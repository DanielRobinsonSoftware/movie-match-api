using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public static class HttpClientExtensions
{
	public static async Task<HttpResponseMessage> GetWithHeadersAsync(this HttpClient httpClient, string requestUri, Dictionary<string, string> headers)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
		
		foreach(var header in headers)
		{
			request.Headers.Add(header.Key, header.Value);
		}

		return await httpClient.SendAsync(request);
	}
}