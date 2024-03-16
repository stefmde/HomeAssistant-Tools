using HomeAssistant_Tools.Core.Models.Configuration;
using Newtonsoft.Json;

namespace HomeAssistant_Tools.Core;

public class ConfigurationEditor<T> where T : IConfigurationEditorConfiguration<T>, new()
{
	private readonly string _filePath;

	public T Configuration { get; set; }
	
	public ConfigurationEditor(string filePath = "configuration.json")
	{
		_filePath = filePath;
		Configuration = File.Exists(filePath) ? ReadConfiguration() : CreateEmptyConfiguration();
	}

	private T CreateEmptyConfiguration()
	{
		var emptyConfig = new T();
		var json = JsonConvert.SerializeObject(emptyConfig);
		File.WriteAllText(_filePath, json);
		return emptyConfig;
	}
	
	private T ReadConfiguration()
	{
		var json = File.ReadAllText(_filePath);
		var config = JsonConvert.DeserializeObject<T>(json);
		return config;
	}

	public bool SafeConfiguration()
	{
		if (!File.Exists(_filePath))
		{
			return false;
		}

		var json = JsonConvert.SerializeObject(Configuration, Formatting.Indented);
		File.WriteAllText(_filePath, json);
		return true;
	}
}