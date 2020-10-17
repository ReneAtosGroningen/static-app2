using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class HttpTriggerCSharp
    {
        [FunctionName("NewGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request for new game.");

            GalgjeAntwoord antwoord = await GetNewWord();

            return new OkObjectResult(antwoord);
        }

        private static async Task<GalgjeAntwoord> GetNewWord()
        {
            return new GalgjeAntwoord{woord="nieuw woord", hash="kjkjckjc"};
        }
    }

    public class GalgjeAntwoord
    {
        public string woord {get;set;}
        public string hash {get;set;}

    }
}
