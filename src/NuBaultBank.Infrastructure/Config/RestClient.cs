using System.Net.Http.Headers;
using System.Text;
using Ardalis.GuardClauses;
using NuBaultBank.Core.Interfaces;
using NuBaultBank.Infrastructure.Config.RestClientExtensions;
using Microsoft.Extensions.Configuration;

namespace NuBaultBank.Infrastructure.Config;

public class RestClient : HttpClient, IRestClient
{
  private readonly string _baseUri;
  private readonly string _token;
  private readonly ApiAccess _apiAccess;
  
  public RestClient(IConfiguration configuration)
  {
    _apiAccess = configuration.GetSection("BaseApiAccess").Get<ApiAccess>();
    Guard.Against.Null(_apiAccess.BaseUri);
    Guard.Against.Null(_apiAccess.BaseToken);

    _baseUri = _apiAccess.BaseUri;
    _token = _apiAccess.BaseToken;
  }
  
  public async Task<T> GetAsync<T>(string url, Dictionary<string, string> query)
  {
    DefaultRequestHeaders.Clear();
    DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    using (HttpResponseMessage response = await GetAsync(Utilities.AddQueryString(_baseUri + url, query)))
    {
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsAsync<T>();
      }
      throw new Exception(response.ReasonPhrase);
    }
  }
  
  public async Task<T> GetAsync<T>(string url)
  {
    DefaultRequestHeaders.Clear();
    DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    using (HttpResponseMessage response = await GetAsync(_baseUri + url))
    {
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsAsync<T>();
      }
      throw new Exception(response.ReasonPhrase);
    }
  }
  
  public async Task<T> PostAsync<T>(string url, string data)
  {
    DefaultRequestHeaders.Clear();
    DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    var content = new StringContent(data, Encoding.UTF8, "application/json");
    using (HttpResponseMessage response = await PostAsync(_baseUri + url, content))
    {
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsAsync<T>();
      }
      throw new Exception(response.ReasonPhrase);
    }
  }
  
  public async Task<T> PutAsync<T>(string url, FileStream fs)
  {
    DefaultRequestHeaders.Clear();
    DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    using (HttpResponseMessage response = await PutAsync(_baseUri + url, new StreamContent(fs)))
    {
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsAsync<T>();
      }
      throw new Exception(response.ReasonPhrase);
    }
  }
}
