using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Sprache;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AIPromptOptimizerExtension
{
    /// <summary>
    /// Represents the returned gemini structure.
    /// </summary>
    public class GeminiPromptEntry
    {
        public string type = "Object";
        public ExpectedProperties properties = new ExpectedProperties();

        public class ExpectedProperties
        {
            public ObjectDesriptor score = new ObjectDesriptor("integer");
            public ObjectDesriptor clarityScore = new ObjectDesriptor("integer");
            public ObjectDesriptor concicenessScore = new ObjectDesriptor("integer");
            public ObjectDesriptor contextualityScore = new ObjectDesriptor("integer");
            public ObjectDesriptor clarityExplanation = new ObjectDesriptor("string");
            public ObjectDesriptor concicenessExplanation = new ObjectDesriptor("string");
            public ObjectDesriptor contextualityExplanation = new ObjectDesriptor("string");
            public ObjectDesriptor benefits = new ObjectDesriptor("string");
            public ObjectDesriptor praise = new ObjectDesriptor("string");
        }

        public class ObjectDesriptor
        {
            public string type;

            public ObjectDesriptor(string type)
            {
                this.type = type;
            }
        }
    }

    public class GeminiResponse
    {
        public List<Candidate> candidates { get; set; }
        public UsageMetadata usageMetadata { get; set; }
        public string modelVersion { get; set; }
        public string responseId { get; set; }

        public AnalysisResults ParseResponse() => JsonConvert.DeserializeObject<AnalysisResults>(candidates[0].content.parts[0].text);

        public class Candidate
        {
            public Content content { get; set; }
            public string finishReason { get; set; }
            public int index { get; set; }
        }

        public class Content
        {
            public List<Part> parts { get; set; }
            public string role { get; set; }
        }

        public class Part
        {
            public string text { get; set; }
        }

        public class PromptTokensDetail
        {
            public string modality { get; set; }
            public int tokenCount { get; set; }
        }

        public class UsageMetadata
        {
            public int promptTokenCount { get; set; }
            public int candidatesTokenCount { get; set; }
            public int totalTokenCount { get; set; }
            public List<PromptTokensDetail> promptTokensDetails { get; set; }
            public int thoughtsTokenCount { get; set; }
        }
    }

    public class AnalysisResults
    {
        public string benefits { get; set; }
        public string clarityExplanation { get; set; }
        public int clarityScore { get; set; }
        public string concicenessExplanation { get; set; }
        public int concicenessScore { get; set; }
        public string contextualityExplanation { get; set; }
        public int contextualityScore { get; set; }
        public string praise { get; set; }
        public int score { get; set; }
    }

    public class GeminiAPI
    {
        public static string GeminiAPIKey { get => AIPromptOptimizerExtensionPackage.APIKey[(byte)AIPromptOptimizerExtensionPackage.Keys.Gemini]; }
        private static readonly HttpClient client = new HttpClient();

        private const string SystemPretensePrompt = @"
            You are Prompty, an expert prompt engineer.
            Users will provide you their prompts.

            Instructions:
            1. Score the prompt from 0% to 100% based on clarity, conciceness, and contextuality.
            2. If there is an issue with clarity, explain the parts of the prompt that are not clear.
            3. If there is an issue with conciceness, explain where the prompt is not concise.
            4. If there is an issue with contextuality, explain where the prompt lacks context.
            5. Praise the good qualities of the prompt.
            6. Explain the benefits of adjusting the input in terms of Token efficiency and generation speed.
            ";

      

        private static async Task<string> PromptAsync(string prompt)
        {
            if (!client.DefaultRequestHeaders.Contains("x-goog-api-key"))
                client.DefaultRequestHeaders.Add("x-goog-api-key", GeminiAPIKey);

            string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
            var payload = new
            {
                system_instruction = new        //For some reason, system_instruction stucture is different from prompt structure.
                {
                    parts = new { text = SystemPretensePrompt }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
               generationConfig = new
               {
                    responseMimeType = "application/json",
                    responseSchema = new
                    {
                        type = "OBJECT",
                        properties = new GeminiPromptEntry.ExpectedProperties()
                    }
                }
            };
            GeminiPromptEntry g = new GeminiPromptEntry();
            string result = JsonConvert.SerializeObject(payload);
            result = result.Trim();
            result = result.Replace("\\r", " ");
            result = result.Replace("\\n", " ");
            StringContent content = new StringContent(result, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Performs a Gemini API call.
        /// </summary>
        /// <param name="prompt">The prompt to request from Gemini.</param>
        /// <returns>An object holding all data on the return response.</returns>
        public static AnalysisResults GetResponse(string prompt)
        {
            try
            {
                string result = ThreadHelper.JoinableTaskFactory.Run(async () => await PromptAsync(prompt));
                GeminiResponse returnedContent = JsonConvert.DeserializeObject<GeminiResponse>(result);
                return returnedContent.ParseResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Performs an asynchronous Gemini API call.
        /// </summary>
        /// <param name="prompt">The prompt to request from Gemini.</param>
        /// <returns>An object holding all data on the return response.</returns>
        public async static Task<AnalysisResults> GetResponseAsync(string prompt)
        {
            try
            {
                string result = await PromptAsync(prompt);
                GeminiResponse returnedContent = JsonConvert.DeserializeObject<GeminiResponse>(result);
                return returnedContent.ParseResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
