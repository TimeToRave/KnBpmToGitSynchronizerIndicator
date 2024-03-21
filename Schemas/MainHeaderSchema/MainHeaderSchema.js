 define("MainHeaderSchema", ["css!KnMainHeaderSchemaCss"], function(){
	return {
		attributes: {
			"LastGitSyncSessinLabel": {
				dataValueType: this.BPMSoft.DataValueType.DateTime,
			},
		},
		messages: { },
		methods: {
			init: function() {
				this.callParent(arguments);
				
				BPMSoft.SysSettings.querySysSettingsItem("KnLastBpmToGitSyncSessionDate", function(value) {
					if (value) {
						var diffInHours = ((new Date() - value) / 3600000).toFixed(1);
						this.set(
							"LastGitSyncSessinLabel", 
							Ext.String.format(this.get("Resources.Strings.LastGitSyncSessionTemplateLabel"), diffInHours)
						);
					}
				}, this);
				
			}
		},
		diff: [
			//main header menu logo
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
				"name": "LastGitSyncSessionDateTimeContainer",
				"parentName": "LastGitSyncSessionContainer",
				"propertyName": "items",
				"values": {
					"itemType": BPMSoft.ViewItemType.LABEL,
					"labelClass": ["last-git-sync-session-label"],
					"caption": {"bindTo": "LastGitSyncSessinLabel"},
				}
			},
		]
	};
});
