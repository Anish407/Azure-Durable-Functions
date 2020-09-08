using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DurableFuncs
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blobpath")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
           

            log.LogInformation("Call Orchestrator from here");

             string name = req.Query["name"];

            //call the orchestrator function
             var orchestrId = await starter.StartNewAsync("O_MyOrchestrator", input: name);

            //takes the HTTP request object and the orchestration instanceID and it uses
            //those to work out the URLs that we'll need to call, in order to check on the progress of our orchestration 
            return starter.CreateCheckStatusResponse(req, orchestrId);
        }
    }
}
