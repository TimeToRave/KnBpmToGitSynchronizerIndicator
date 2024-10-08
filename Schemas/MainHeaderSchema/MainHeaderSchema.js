 define("MainHeaderSchema", ["ServiceHelper", "css!KnMainHeaderSchemaCss"], 
function(ServiceHelper){
	return {
		attributes: {
			"LastGitSyncSessionLabel": {
				dataValueType: this.BPMSoft.DataValueType.DateTime,
			},
			"GitSyncSessionStatusLabel": {
				dataValueType: this.BPMSoft.DataValueType.Text,
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
				this.set("GitSyncSessionStatusLabel", "Git status checking");
				this.callParent(arguments);
				this.updateGitSyncLabel();	
			},

			/**
			 * Инициализирует состояние индикатора
			 * синхронизации с git-репозиторием
			 */
			updateGitSyncLabel: function () {
				BPMSoft.SysSettings.querySysSettings(
					["KnBpmToGitSyncStatus", "KnLastBpmToGitSyncSessionDate", "KnBpmToGitSyncMessage", "KnGitSyncPeriod"],
					function (values) {
						if (values.KnLastBpmToGitSyncSessionDate) {
							var diffInHours = ((new Date() - values.KnLastBpmToGitSyncSessionDate) / 3600000).toFixed(1);
							this.set(
								"GitSyncSessionStatusLabel",
								Ext.String.format(this.get("Resources.Strings.LastGitSyncSessionTemplateLabel"), diffInHours)
							);

							if((diffInHours * 60) > values.KnGitSyncPeriod) {
								$("#sync-status-label")
									.removeClass("last-git-sync-session-date-label")
									.addClass("git-autocommiter-is-stopped");
							}

							if (values.KnBpmToGitSyncStatus) {
							   var isError = values.KnBpmToGitSyncStatus.toLowerCase() === "error";
							   this.set("IsErrorLogoVisible", isError);									
							}
						}
					}, this);
			},

			/**
			 * Обработка нажатия на кнопку "git commit"
			 */
            onInitCommitButtonClicked: function() {
                var commitData = {
					message: "Manual commit from user"
				};

            	ServiceHelper.callService(
					"BpmToGitSynchronizerIndicatorService", 
					"CreateCommit",
                    function () {
						console.log("Commit created");
					}, 
					commitData, 
					this
				);  
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
					"id": "sync-status-label",
					"itemType": BPMSoft.ViewItemType.LABEL,
					"labelClass": ["git-sync-label last-git-sync-session-date-label"],
					"caption": {"bindTo": "GitSyncSessionStatusLabel"},
					"visible": {"bindTo": "IsErrorLogoVisible", "bindConfig": {"converter": "invertBooleanValue"}},
				}
			},
			{
				"operation": "insert",
				"name": "GitSyncErrorStatusContainer",
				"parentName": "LastGitSyncSessionContainer",
				"propertyName": "items",
				"values": {
					"id": "sync-error-label",
					"itemType": BPMSoft.ViewItemType.LABEL,
					"labelClass": ["git-sync-label git-sync-error-label"],
					"caption": {"bindTo": "GitSyncSessionStatusLabel"},
					"visible": {"bindTo": "IsErrorLogoVisible"}
				}
			},
			{
				"operation": "insert",
				"name": "LastGitSyncInitCommitButton",
				"parentName": "LastGitSyncSessionContainer",
				"propertyName": "items",
				"values": {
                    "id": "InitCommitButton",
                    "itemType": BPMSoft.ViewItemType.COMPONENT,
                    "className": "BPMSoft.Button",
                    "caption": "Зафиксировать изменения",
                    "click": {
                        "bindTo": "onInitCommitButtonClicked"
                    }
				}
			},
		]
	};
});
