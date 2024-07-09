using System;
using BPMSoft.Core;
using BPMSoft.Core.DB;

namespace BPMSoft.Configuration
{
    public class KnBpmToGitSyncUtilities
    {
        UserConnection UserConnection { get; set; }

        public KnBpmToGitSyncUtilities(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }

        /**
         * Устанавливает дату последней синхонизации с 
         * git-репозиторием
         */
        internal void SetSyncLastDate(string date)
        {
            var recievedDateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            SetSysSettingValue("KnLastBpmToGitSyncSessionDate", recievedDateTime);
        }

        /**
         * Устанавливает результат последней синхронизации
         */
        internal void SetSyncStatus(string status)
        {
                SetSysSettingValue("KnBpmToGitSyncStatus", status);
        }

        /**
         * Устанавливает сообщение последней синхронизации
         */
        internal void SetSyncStatusMessage(string message)
        {
                SetSysSettingValue("KnBpmToGitSyncMessage", message);
        }

        /**
         * Получает идентификатор системной настройки по ее коду
         */
        internal Guid GetSysSettingId(string sysSettingCode) {
            return (
                    new Select(UserConnection)
                    .Column("Id")
                    .From("SysSettings")
                    .Where("Code")
                        .IsEqual(Column.Const(sysSettingCode)) as Select)
                    .ExecuteScalar<Guid>();
        }

        /**
         * Универсальный метод.
         * Устанавливает значение системной настройки
         * текстовое значение
         */
        internal void SetSysSettingValue(string sysSettingCode, string value) {
            Guid sysSettingId = GetSysSettingId(sysSettingCode);

            if (!(UserConnection is object))
            {
                throw new NullReferenceException("UserConnection is not an object");
            }

            if(!sysSettingId.Equals(Guid.Empty)) {
                (new Update(UserConnection)
                    .Set("TextValue", Column.Const(value))
                    .Where("SysSettingsId")
                        .IsEqual(Column.Const(sysSettingId))).Execute();
            }
        }

        /**
         * Универсальный метод.
         * Устанавливает значение системной настройки
         * Значение типа: Дата и время
         */
        internal void SetSysSettingValue(string sysSettingCode, DateTime value) {
            Guid sysSettingId = GetSysSettingId(sysSettingCode);

            if (!(UserConnection is object))
            {
                throw new NullReferenceException("UserConnection is not an object");
            }

            if(!sysSettingId.Equals(Guid.Empty)) {
                (new Update(UserConnection)
                    .Set("DateTimeValue", Column.Const(value))
                    .Where("SysSettingsId")
                        .IsEqual(Column.Const(sysSettingId))).Execute();
            }
        }
    }
}
