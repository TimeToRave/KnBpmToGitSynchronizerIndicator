using System.ServiceModel.Web;
using System.ServiceModel;
using System;
using BPMSoft.Web.Common;
using System.ServiceModel.Activation;
using BPMSoft.Core;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using BPMSoft.Core.DB;
using Newtonsoft.Json;

namespace BPMSoft.Configuration
{
    /// <summary>
    /// Сервис для обновления даты последней синхронизации с Git-репозитрием
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BpmToGitSynchronizerIndicatorService : BaseService
    {

        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private CancellationTokenSource _cancellationTokenSource;

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = nameof(UpdateLastGitSyncDate), BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        public string UpdateLastGitSyncDate(string dateTime)
        {
            var logic = new BpmToGitSynchronizerIndicatorLogic(UserConnection);
            return logic.UpdateLastGitSyncDate(dateTime);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = nameof(UpdateGitSyncStatus), BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        public string UpdateGitSyncStatus(string status, string message)
        {
            var logic = new BpmToGitSynchronizerIndicatorLogic(UserConnection);
            return logic.UpdateGitSyncStatus(status, message);
        }


        private static readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        private static readonly ConcurrentDictionary<Guid, TaskCompletionSource<string>> _clients = new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = nameof(CommitMessagePolling), BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        public async Task<CommitMessage> CommitMessagePolling()
        {
            var tcs = new TaskCompletionSource<string>();

            var clientId = Guid.NewGuid();
            _clients.TryAdd(clientId, tcs);

            string message;
            try
            {
                message = await tcs.Task.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                return null;
            }

            return new CommitMessage() { Message = message };
        }


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = nameof(CreateCommit), BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        public void CreateCommit(string message)
        {
            _messages.Enqueue(message);

            foreach (var client in _clients)
            {
                if (_clients.TryRemove(client.Key, out var tcs))
                {
                    tcs.TrySetResult(message);
                }
            }
        }
    }

    /// <summary>
    /// Утилитный класс для работы с данными
    /// </summary>
    public class BpmToGitSynchronizerIndicatorLogic
    {
        public UserConnection UserConnection { get; }
        public BpmToGitSynchronizerIndicatorLogic(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }

        /// <summary>
        /// Обновляет дату последней синхроинзации с git
        /// </summary>
        /// <param name="date">Дата последней синхронизации в текстовом формате</param>
        /// <returns>Успешный результат либо текст ошибки</returns>
        public string UpdateLastGitSyncDate(string date)
        {
            try
            {
                new KnBpmToGitSyncUtilities(UserConnection).SetSyncLastDate(date);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message + "  StackTrace: " + ex.StackTrace;
            }
        }


        /// <summary>
        /// Устанавливает статус синхронизации с git-репозиторием
        /// </summary>
        /// <returns>Результат обработки запроса</returns>
        public string UpdateGitSyncStatus(string status, string message)
        {
            try
            {
                var utils = new KnBpmToGitSyncUtilities(UserConnection);
                utils.SetSyncStatus(status);
                utils.SetSyncStatusMessage(message);

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message + "  StackTrace: " + ex.StackTrace;
            }
        }

    }

    public class CommitMessage 
    {
        [JsonProperty("message")]
        public string Message;
    }

}