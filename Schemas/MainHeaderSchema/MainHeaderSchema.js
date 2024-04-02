 define("MainHeaderSchema", ["css!KnMainHeaderSchemaCss"], function(){
	return {
		attributes: {
			"LastGitSyncSessionLabel": {
				dataValueType: this.BPMSoft.DataValueType.DateTime,
			},
			"GitSyncSessionStatusLabel": {
				dataValueType: this.BPMSoft.DataValueType.DateTime,
			},
			"IsErrorLogoVisible": {
				dataValueType: this.BPMSoft.DataValueType.Boolean,
				value: false
			}
		},
		methods: {
			/**
			 * Инициализация модуля
			 */
			init: function() {
				this.callParent(arguments);
				this.updateGitSyncLabel();	
			},

			/**
			 * Инициализирует состояние индикатора
			 * синхронизации с git-репозиторием
			 */
			updateGitSyncLabel: function() {
				BPMSoft.SysSettings.querySysSettings(
					["KnBpmToGitSyncStatus", "KnLastBpmToGitSyncSessionDate", "KnBpmToGitSyncMessage"], 
					function(values) {

						if (values.KnLastBpmToGitSyncSessionDate) {
							var diffInHours = ((new Date() - values.KnLastBpmToGitSyncSessionDate) / 3600000).toFixed(1);
							this.set(
								"LastGitSyncSessionLabel", 
								Ext.String.format(this.get("Resources.Strings.LastGitSyncSessionTemplateLabel"), diffInHours)
							);
						
							if (values.KnBpmToGitSyncStatus) {
								this.set(
									"GitSyncSessionStatusLabel", 
									Ext.String.format(this.get("Resources.Strings.GitSyncSessionStatusTemplateLabel"), values.KnBpmToGitSyncMessage , diffInHours)
								);

								this.set("IsErrorLogoVisible", values.KnBpmToGitSyncStatus.toLowerCase() === "error");
							}
						}	
				}, this);
			}
		},
		diff: [
			{
				"operation": "insert",
				"name": "LastGitSyncSessionContainer",
				"parentName": "RightHeaderContainer",
				"propertyName": "items",
				"values": {
					"id": "main-header-last-git-sync-container",
					"itemType": BPMSoft.ViewItemType.CONTAINER,
					"wrapClass": [],
					"items": []
				}
			},
			{
				"operation": "insert",
				"name": "LastGitSyncSessionStatusContainer",
				"parentName": "LastGitSyncSessionContainer",
				"propertyName": "items",
				"values": {
					"itemType": BPMSoft.ViewItemType.LABEL,
					"labelClass": ["git-sync-label git-sync-error-label"],
					"caption": {"bindTo": "GitSyncSessionStatusLabel"},
					"visible": {"bindTo": "IsErrorLogoVisible"}
				}
			},
			{
				"operation": "insert",
				"name": "LastGitSyncSessionDateTimeContainer",
				"parentName": "LastGitSyncSessionContainer",
				"propertyName": "items",
				"values": {
					"itemType": BPMSoft.ViewItemType.LABEL,
					"labelClass": ["git-sync-label last-git-sync-session-date-label"],
					"caption": {"bindTo": "LastGitSyncSessionLabel"},
					"visible": {"bindTo": "IsErrorLogoVisible", "bindConfig": {"converter": "invertBooleanValue"}}
				}
			}
		]
	};
});
