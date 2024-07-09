using System.ServiceModel.Web;
using System.ServiceModel;
using System;
using BPMSoft.Web.Common;
using System.ServiceModel.Activation;
using BPMSoft.Core;

namespace BPMSoft.Configuration
{
    /// <summary>
    /// Сервис для обновления даты последней синхронизации с Git-репозитрием
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BpmToGitSynchronizerIndicatorService : BaseService
    {
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
}