using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIPromptOptimizerExtension
{
    public class GeminiAPI
    {
        public static string GeminiAPIKey { get => ""; }
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetResponseAsync(string prompt)
        {
            string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
            var data = new
            {
                text = prompt
            };
            string content = JsonConvert.SerializeObject(data);
            Console.WriteLine(content);
            HttpRequestMessage httpContent = new HttpRequestMessage(HttpMethod.Post, url);
            httpContent.Headers.Add("x-goog-api-key", GeminiAPIKey);
            //httpContent.Headers.Add("Content-Type", "application/json");
            httpContent.Content = new StringContent(content);

            HttpResponseMessage response = await client.SendAsync(httpContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
