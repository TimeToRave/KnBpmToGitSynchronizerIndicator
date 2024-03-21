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
                var recievedDateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
                recievedDateTime += UserConnection.CurrentUser.GetTimeZoneOffset();

                BPMSoft.Core.Configuration.SysSettings.SetValue(
                    UserConnection,
                    "KnLastBpmToGitSyncSessionDate",
                    recievedDateTime
                );

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}