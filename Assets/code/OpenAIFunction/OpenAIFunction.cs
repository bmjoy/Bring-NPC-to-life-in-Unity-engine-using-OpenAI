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
}
