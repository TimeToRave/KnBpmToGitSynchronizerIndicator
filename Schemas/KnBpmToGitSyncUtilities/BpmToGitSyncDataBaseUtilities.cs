using System;
using BPMSoft.Core;

namespace BPMSoft.Configuration
{
    public class BpmToGitSyncDataBaseUtilities
    {
        UserConnection UserConnection { get; set; }

        public BpmToGitSyncDataBaseUtilities(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }

        internal void SetSyncLastDate(string date)
        {
            var recievedDateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            recievedDateTime += UserConnection.CurrentUser.GetTimeZoneOffset();

            BPMSoft.Core.Configuration.SysSettings.SetValue(
                UserConnection,
                "KnLastBpmToGitSyncSessionDate",
                recievedDateTime
            );
        }

        internal void SetSyncStatus(string status)
        {
            BPMSoft.Core.Configuration.SysSettings.SetValue(
                UserConnection,
                "KnBpmToGitSyncStatus",
                status
            );
        }

        internal void SetSyncStatusMessage(string message)
        {
            BPMSoft.Core.Configuration.SysSettings.SetValue(
                UserConnection,
                "KnBpmToGitSyncMessage",
                message
            );
        }
    }
}