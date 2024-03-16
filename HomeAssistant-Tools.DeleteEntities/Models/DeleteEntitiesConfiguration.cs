using HomeAssistant_Tools.Core.Models.Configuration;

namespace HomeAssistant_Tools.DeleteEntities.Models;

public class DeleteEntitiesConfiguration : IConfigurationEditorConfiguration<DeleteEntitiesConfiguration>
{
	public ConfigurationProperty<bool> SkipDisclaimer { get; set; } = new();
	
	public ConfigurationProperty<bool> GlobalSaveEnabled { get; set; } = new();
	
	// public ConfigurationProperty<bool> GlobalAskForChanges { get; set; } = new();
	//
	// public ConfigurationProperty<bool> GlobalSavedAsDefaultAnswer { get; set; } = new();
	
	
	public ConfigurationProperty<string> ApiToken { get; set; } = new();
	
	public ConfigurationProperty<string> BaseUrl { get; set; } = new();
	
	public ConfigurationProperty<List<string>> Entities { get; set; } = new();
	
	public ConfigurationProperty<bool> PurgeOrphaned { get; set; } = new();
}