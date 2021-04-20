using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
  public class Program
  {
    static async Task Main(string[] args)
    {
      var client = new HttpClient();
      var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
      if (disco.IsError)
      {
        Console.WriteLine(disco.Error);
        Console.ReadLine();
        return;
      }

      var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
      {
        Address = disco.TokenEndpoint,

        ClientId = "client",
        ClientSecret = "secret",

        Scope = "api1 api2"
      });

      if (tokenResponse.IsError)
      {
        Console.WriteLine(tokenResponse.Error);
        Console.ReadLine();
        return;
      }

      Console.WriteLine(tokenResponse.Json);

      // call api
      var apiClient = new HttpClient();
      apiClient.SetBearerToken(tokenResponse.AccessToken);

      var response = await apiClient.GetAsync("https://localhost:6001/identity");
      if (!response.IsSuccessStatusCode)
      {
        Console.WriteLine(response.StatusCode);
      }
      else
      {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JArray.Parse(content));
      }
      Console.ReadLine();
    }
  }
}
