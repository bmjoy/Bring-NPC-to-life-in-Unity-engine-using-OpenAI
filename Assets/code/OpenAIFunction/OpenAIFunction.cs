using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace OpenAIFunction
{
    public static class OpenAIFunction
    {

        [FunctionName("OpenAIFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null),] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var apiKey = "XXXXX-XXXXXXXXXXXXXXX";
            var httpClient = new HttpClient();
            var openAIClient = new OpenAIClient(apiKey, httpClient);

            // SupportedEndpoints
            Console.WriteLine(JsonConvert.SerializeObject(openAIClient.SupportedEndpoints));

            // List models
            var models = await openAIClient.ListModels();
            //Console.WriteLine(models);

            // Generate Answer
            var modelParams = new OpenAIClient.ModelParameters()
            {
                model = "text-davinci-002",
                prompt = "Who are you?",
                temperature = 0.2f,

            };
            var response = await openAIClient.GenerateAnswer(JsonConvert.SerializeObject(modelParams));
            Console.WriteLine(response);
            Console.WriteLine(JsonConvert.SerializeObject(response));

            return new OkObjectResult("OK");
        }
    }

    public class OpenAIClient
    {
        private string AuthKey;

        public HttpClient httpClient;

        public Dictionary<string, Uri> SupportedEndpoints = new Dictionary<string, Uri>()
        {
            { "list", new Uri("https://api.openai.com/v1/models")},
            { "completion", new Uri("https://api.openai.com/v1/completions")},
        };

        public OpenAIClient(string userApiKey, HttpClient httpClient)
        {
            this.AuthKey = userApiKey;
            this.httpClient = httpClient;
        }

        public async Task<dynamic> ListModels()
        {
            var request = new HttpRequestMessage();
            createHttpRequest(request, this.SupportedEndpoints["list"], "GET");
            var response = await this.httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<dynamic> GenerateAnswer(string body = null, string model = "text-davinci-001", string prompt = "test")
        {
            if (body == null)
            {
                var modelParams = new ModelParameters() { model = model, prompt = prompt };
                body = JsonConvert.SerializeObject(modelParams);
            }

            var request = new HttpRequestMessage();
            createHttpRequest(request, this.SupportedEndpoints["completion"], "POST", body);
            var response = await this.httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public HttpRequestMessage createHttpRequest(HttpRequestMessage request, Uri Uri, string methodType = "GET", string body = null)
        {
            request.RequestUri = Uri;
            request.Method = methodType.Equals("GET") ? HttpMethod.Get : HttpMethod.Post;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AuthKey);

            if (methodType == "POST" && body != null)
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            }

            return request;
        }

        public class ModelParameters
        {
            public string model;

            public string prompt;

            // Sampling temperature. Higher values = more risk/creativity 
            public float temperature;

            // Unique identifier representing your end-user, possibly a player
            public string user = "";

            // Should the model stream back partial progress 
            public bool stream;

            // Echo back prompt
            public bool echo;

        }
    }
}
