namespace HomeAssistant_Tools.Core;

public static class UserInteraction
{
	public static string GetDataFromUser(string question, string? defaultAnswer = null, List<string>? additionalAnswers = null)
    {
    	while (true)
    	{
    		if (string.IsNullOrWhiteSpace(defaultAnswer))
    		{
    			Console.Write($"{question} ");
    		}
    		else
    		{
    			Console.Write($"{question} [{defaultAnswer}/{string.Join('/', additionalAnswers)}]: ");
    		}
    		
    		var value = Console.ReadLine();
    
    		if (string.IsNullOrWhiteSpace(value) && defaultAnswer is not null)
    		{
    			return defaultAnswer;
    		}
    		
    		if (string.IsNullOrWhiteSpace(value) && defaultAnswer is null)
    		{
    			continue;
    		}
    		
    		if (!string.IsNullOrWhiteSpace(value) && defaultAnswer is null)
    		{
    			return value;
    		}
    
    		var selectedAdditionalAnswer = additionalAnswers.FirstOrDefault(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));
    		if (!string.IsNullOrWhiteSpace(selectedAdditionalAnswer))
    		{
    			return selectedAdditionalAnswer;
    		}
    	}
    }
    
    public static List<string> GetListFromUser(string question)
    {
    	var data = new List<string>();
    	Console.WriteLine($"{question} (Enter empty to stop adding, minimum one):");
    	while (true)
    	{
    		var value = Console.ReadLine();
    
    		if (string.IsNullOrWhiteSpace(value) && !data.Any())
    		{
    			continue;
    		}
    		
    		if (string.IsNullOrWhiteSpace(value) && data.Any())
    		{
    			return data;
    		}
    		
    		if (!string.IsNullOrWhiteSpace(value))
    		{
    			data.Add(value);
    		}
    	}
    }
}