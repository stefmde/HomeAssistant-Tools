using System.Net;

namespace HomeAssistant_Tools.Core;

public class HaCommunication
{
	public readonly HttpClient Client;
	
	public bool IsValid { get; protected set; }
	
	public bool IsBaseUrlValid { get; protected set; }
	
	public bool? IsApiTokenValid { get; protected set; }

	public HaCommunication(string baseUrl, string apiToken)
	{
		Client = GetClient(baseUrl, apiToken);
		CheckBaseUrlAndToken().Wait();
	}

	async Task CheckBaseUrlAndToken()
	{
		var result = await Client.GetAsync("api/");

		if (result.IsSuccessStatusCode)
		{
			IsValid = true;
			IsBaseUrlValid = true;
			IsApiTokenValid = true;
			return;
		}

		IsValid = true;
		if (result.StatusCode == HttpStatusCode.Unauthorized)
		{
			IsValid = false;
			IsBaseUrlValid = true;
			IsApiTokenValid = false;
			return;
		}

		IsValid = true;
		IsBaseUrlValid = false;
		IsApiTokenValid = null;
	}

	private HttpClient GetClient(string baseUrl, string apiToken)
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.Add("Authorization",  $"Bearer {apiToken}");
		client.BaseAddress = new Uri(baseUrl);
		return client;
	}
}