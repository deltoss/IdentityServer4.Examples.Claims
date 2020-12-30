using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace IdentityServer4Example.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            string accessToken = await GetAccessTokenAsync(client, disco);

            Console.WriteLine(accessToken);

            UserInfoResponse userInfo = await GetUserInfoAsync(accessToken, client, disco);

            foreach (Claim claim in userInfo.Claims)
            {
                Console.WriteLine();
                Console.WriteLine($"Type: {claim.Type}");
                Console.WriteLine($"Value: {claim.Value}");
                Console.WriteLine();
            }

            Console.ReadLine();
        }

        public static async Task<string> GetAccessTokenAsync(HttpClient client, DiscoveryDocumentResponse disco)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(disco.TokenEndpoint));

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("scope", "weatherforecastapi.read openid email"),
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

        public static async Task<UserInfoResponse> GetUserInfoAsync(string accessToken, HttpClient client, DiscoveryDocumentResponse disco)
        {
            return await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken,
            });
        }
    }
}
