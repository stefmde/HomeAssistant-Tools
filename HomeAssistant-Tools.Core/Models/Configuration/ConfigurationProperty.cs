using System.Collections;

namespace HomeAssistant_Tools.Core.Models.Configuration;

public class ConfigurationProperty<T>
{
	public T Value { get; set; }
	
	public bool IsSet { get; set; }
	
	// public bool DoSave { get; set; }
}