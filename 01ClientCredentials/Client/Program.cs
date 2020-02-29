using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "louieclient",
                Scope = "api1"
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            //得到token后请求api接口
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);//设置header

            var response = await apiClient.GetAsync("https://localhost:5001/api/identity/get");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
                Console.WriteLine("\n\n");
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine(response.StatusCode);
            }
            Console.ReadLine();
        }
    }
}
