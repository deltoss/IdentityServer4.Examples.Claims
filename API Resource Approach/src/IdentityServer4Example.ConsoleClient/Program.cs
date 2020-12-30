using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IdentityServer4Example.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            string accessToken = await GetAccessTokenAsync(client);

            Console.WriteLine(accessToken);

            Console.ReadLine();
        }

        public static async Task<string> GetAccessTokenAsync(HttpClient client)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost:5001/connect/token"));

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("scope", "weatherforecastapi.read"),
                new KeyValuePair<string, string>("client_id", "client"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("username", "scott"),
                new KeyValuePair<string, string>("password", "password"),
            };
            request.Content = new FormUrlEncodedContent(formData);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            JsonElement jsonResp = JsonSerializer.Deserialize<JsonElement>(data);
            return jsonResp.GetProperty("access_token").GetString();
        }
    }
}
