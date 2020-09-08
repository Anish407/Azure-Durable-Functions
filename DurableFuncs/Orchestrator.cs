using DurableTask.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DurableFuncs.MyActivities;

namespace DurableFuncs
{
    public class Orchestrator
    {

        [FunctionName("O_MyOrchestrator")]
        public static async Task<object> O_MyOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
            ILogger logger
            )
        {
            //Start the activities, the name of the blob file is passed ,
            // we take the name of the file and pass it to different activities 

            string name = orchestrationContext.GetInput<string>();

            if (!orchestrationContext.IsReplaying)
                logger.LogInformation("Log started");

            //Await here works differently. 
            //Read the file from the blob container 'democont' and write it to another container called demo2
            var activity1 = await orchestrationContext.CallActivityAsync<string>("A_WriteToBlobB", name);

            //Read from the blob container 'democont' and write its contents to a queue 
            var activity2 = await orchestrationContext.CallActivityAsync<string>("A_WriteToQueue", name);

            // read the contents from the blob container 'democont' and write it to table storage
            var activity3 = await orchestrationContext.CallActivityAsync<MyClass>("A_WriteToTableStrg", name);

            return new
            {
                One = activity1,
                Two=activity2,
                Three=activity3
            };
        }
    }
}
