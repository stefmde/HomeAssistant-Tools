using System.Text;
using HomeAssistant_Tools.Core;
using HomeAssistant_Tools.DeleteEntities.Models;

Console.WriteLine(" Home Assistant Tools - Delete Entities");
Console.WriteLine("########################################");
Console.WriteLine();
var configEditor = new ConfigurationEditor<DeleteEntitiesConfiguration>();

if (!configEditor.Configuration.SkipDisclaimer.IsSet || !configEditor.Configuration.SkipDisclaimer.Value || !configEditor.Configuration.GlobalSaveEnabled.Value)
{
	Console.WriteLine("DISCLAIMER (Read carefully!):");
	Console.WriteLine("This application executes DELETE statements against your Home Assistant instance.");
	Console.WriteLine("The deleted data is NOT recoverable if you don't do routinely backups.");
	Console.WriteLine("If you or this App is making an misstake/bug and you don't have a Backup YOR DATA IS GONE FOREVER!");
	Console.WriteLine("So if you want to use this App CREATE A BACKUP before using it and be sure what you do.");
	Console.WriteLine("DON'T USE THIS APP if you don't have a current Backup or your aren't sure what you or this App will do!");
	Console.WriteLine("I will not be liable for any potential damage this app or you might do. For this also see the LICENSE-file.");
	Console.WriteLine();
}

// GlobalSaveEnabled
if (!configEditor.Configuration.GlobalSaveEnabled.IsSet)
{
	var setting = UserInteraction.GetDataFromUser("Do you want to enable saving of settings?", "Y", ["n"]);
	configEditor.Configuration.GlobalSaveEnabled.IsSet = true;
	// configEditor.Configuration.GlobalSaveEnabled.DoSave = true;
	configEditor.Configuration.GlobalSaveEnabled.Value = setting == "Y";
	configEditor.SafeConfiguration();
	Console.WriteLine();
}

// SkipDisclaimer
if (!configEditor.Configuration.SkipDisclaimer.IsSet && configEditor.Configuration.GlobalSaveEnabled.Value)
{
	var setting = UserInteraction.GetDataFromUser("Do you want to skip the disclaimer next time? (Still be careful)", "N", ["y"]);
	configEditor.Configuration.SkipDisclaimer.IsSet = true;
	// configEditor.Configuration.SkipDisclaimer.DoSave = true;
	configEditor.Configuration.SkipDisclaimer.Value = setting == "Y";
	configEditor.SafeConfiguration();
	Console.WriteLine();
}

// if (!configEditor.Configuration.GlobalSavedAsDefaultAnswer.IsSet)
// {
// 	if (configEditor.Configuration.GlobalSaveEnabled.Value)
// 	{
// 		var setting = GetDataFromUser("Do you want to show saved configurations as default settings?", "Y", ["n"]);
// 		configEditor.Configuration.GlobalSavedAsDefaultAnswer.IsSet = true;
// 		configEditor.Configuration.GlobalSavedAsDefaultAnswer.DoSave = true;
// 		configEditor.Configuration.GlobalSavedAsDefaultAnswer.Value = setting == "Y";
// 		configEditor.SafeConfiguration();
// 	}
// 	else
// 	{
// 		configEditor.Configuration.GlobalSavedAsDefaultAnswer.IsSet = true;
// 		configEditor.Configuration.GlobalSavedAsDefaultAnswer.DoSave = false;
// 		configEditor.Configuration.GlobalSavedAsDefaultAnswer.Value = false;
// 	}
// Console.WriteLine();
// }
HaCommunication haCommunication = null;
var apiTokenAndBaseUrlValid = false;
while (!apiTokenAndBaseUrlValid)
{
	// BaseUrl
	if (!configEditor.Configuration.BaseUrl.IsSet || !configEditor.Configuration.GlobalSaveEnabled.Value)
	{
		var setting = UserInteraction.GetDataFromUser("Home Assistant Url:");
		if (!setting.EndsWith("/"))
		{
			setting += "/";
		}
		configEditor.Configuration.BaseUrl.IsSet = true;
		configEditor.Configuration.BaseUrl.Value = setting;
		
		if (configEditor.Configuration.GlobalSaveEnabled.Value)
		{
			// configEditor.Configuration.BaseUrl.DoSave = true;
			configEditor.SafeConfiguration();
		}
		// else
		// {
		// 	configEditor.Configuration.BaseUrl.DoSave = false;
		// }
		Console.WriteLine();
	}

	// ApiToken
	var urlToCreateApiToken = $"{configEditor.Configuration.BaseUrl.Value}profile";
	var createApiTokenMessage = $"If you don't have an ApiToken or need to check if yours still active, you can do so, on the bottom, here: {urlToCreateApiToken}";
	if (!configEditor.Configuration.ApiToken.IsSet || !configEditor.Configuration.GlobalSaveEnabled.Value)
	{
		Console.WriteLine(createApiTokenMessage);
		var setting = UserInteraction.GetDataFromUser("Home Assistant Api Token:");
		configEditor.Configuration.ApiToken.IsSet = true;
		configEditor.Configuration.ApiToken.Value = setting;
		
		if (configEditor.Configuration.GlobalSaveEnabled.Value)
		{
			// configEditor.Configuration.ApiToken.DoSave = true;
			configEditor.SafeConfiguration();
		}
		// else
		// {
		// 	configEditor.Configuration.ApiToken.DoSave = false;
		// }
		Console.WriteLine();
	}

	// Check ApiToken and BaseUrl
	haCommunication = new HaCommunication(configEditor.Configuration.BaseUrl.Value, configEditor.Configuration.ApiToken.Value);
	if (haCommunication.IsValid)
	{
		Console.WriteLine("ApiToken and BaseUrl tested successfully");
		apiTokenAndBaseUrlValid = true;
	}
	if (!haCommunication.IsBaseUrlValid)
	{
		Console.WriteLine($"BaseUrl NOT tested successfully. BaseUrl '{configEditor.Configuration.BaseUrl.Value}' not correct or Home Assistant not running?");
		configEditor.Configuration.BaseUrl.Value = null;
		configEditor.Configuration.BaseUrl.IsSet = false;
	}
	if (haCommunication.IsApiTokenValid is not null && !haCommunication.IsApiTokenValid.Value)
	{
		Console.WriteLine($"ApiToken NOT tested successfully. {createApiTokenMessage}");
		configEditor.Configuration.ApiToken.Value = null;
		configEditor.Configuration.ApiToken.IsSet = false;
	}
	if (!haCommunication.IsValid)
	{
		if (configEditor.Configuration.GlobalSaveEnabled.Value)
		{
			configEditor.SafeConfiguration();
		}
		Console.WriteLine();
	}
}
Console.WriteLine();

// Entities
if (!configEditor.Configuration.Entities.IsSet || !configEditor.Configuration.GlobalSaveEnabled.Value)
{
	var setting = UserInteraction.GetListFromUser("Entities to delete:");
	configEditor.Configuration.Entities.IsSet = true;
	configEditor.Configuration.Entities.Value = setting;
	
	if (configEditor.Configuration.GlobalSaveEnabled.Value)
	{
		// configEditor.Configuration.Entities.DoSave = true;
		configEditor.SafeConfiguration();
	}
	// else
	// {
	// 	configEditor.Configuration.Entities.DoSave = false;
	// }
	Console.WriteLine();
}

// Orphaned
var orphanedSetting = UserInteraction.GetDataFromUser("Delete orphaned entities? (May also delete temporary disabled)", "N", ["y"]);
configEditor.Configuration.PurgeOrphaned.IsSet = true;
configEditor.Configuration.PurgeOrphaned.Value = orphanedSetting == "y";

if (configEditor.Configuration.GlobalSaveEnabled.Value)
{
	// configEditor.Configuration.PurgeOrphaned.DoSave = true;
	configEditor.SafeConfiguration();
}
// else
// {
// 	configEditor.Configuration.PurgeOrphaned.DoSave = false;
// }
Console.WriteLine();

// Start?
var start = UserInteraction.GetDataFromUser("Start or exit?", "EXIT", ["start"]);
if (start == "EXIT")
{
	return;
}
Console.WriteLine();

// Execute
Console.WriteLine("Executing...");
Console.WriteLine("'DeleteState' Could be 'False' if Entity does not exist");
foreach (var entity in configEditor.Configuration.Entities.Value)
{
	Console.WriteLine(await DeleteEntity(entity));
}
Console.WriteLine();
Console.WriteLine("Done.");


// HELPERS
async Task<string> DeleteEntity(string entityId)
{
	// Check Client
	if (haCommunication is null || !haCommunication.IsValid)
	{
		return "Client has not been initialized successfully.";
	}
	
	// Delete Entity
	var resultDeleteState = (await haCommunication.Client.DeleteAsync($"api/states/{entityId}")).IsSuccessStatusCode;

	// Purge Entities
	var serviceData = new StringContent($"{{\"entity_id\": \"{entityId}\"}}", Encoding.UTF8, "application/json");
	var resultPurgeEntitiesService = (await haCommunication.Client.PostAsync("api/services/Recorder/purge_entities", serviceData)).IsSuccessStatusCode;

	// Purge
	bool? resultPurgeService = null;
	if (configEditor.Configuration.PurgeOrphaned.Value)
	{
		resultPurgeService = (await haCommunication.Client.PostAsync("api/services/Recorder/purge", null)).IsSuccessStatusCode;
	}

	// Result
	var result = $"DeleteState: {resultDeleteState} | PurgeEntities: {resultPurgeEntitiesService}";
	if (resultPurgeService is not null)
	{
		result += $" | Purge: {resultPurgeService.Value}";
	}

	result += $" | EntityId: {entityId}";
	
	return result;
}
