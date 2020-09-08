using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DurableFuncs
{
    public static class MyActivities
    {
        /// <summary>
        /// Read the blob name passed to the activity trigger and save it to
        /// another container in a new file
        /// </summary>
        /// <param name="path">name of the blob passed by the orchestrator</param>
        /// <param name="stream"></param>
        /// <param name="textWriter"></param>
        /// <param name="binder"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        [FunctionName("A_WriteToBlobB")]
        public static async Task<string> A_WriteToBlobB(
            [ActivityTrigger] string path,
            [Blob("democont/{path}",FileAccess.Read)]Stream stream,
            [Blob("demo2/{path}.txt", FileAccess.Write)]TextWriter textWriter,
            IBinder binder,
            ILogger logger)
        {
            var data =await new StreamReader(stream).ReadToEndAsync();
            await textWriter.WriteAsync(data);
            return data;
        }

        //Get the blob name from the orchestrator
        //use a blob input binding to read the contents of the blob and
        // write the contents to a queue
        [FunctionName("A_WriteToQueue")]
        [return: Queue("demoq")]
        public static async Task<string> A_WriteToQueue(
          [ActivityTrigger] string path,
          [Blob("democont/{path}", FileAccess.Read)] Stream stream,
          ILogger logger)
        {
            return await new StreamReader(stream).ReadToEndAsync();
        }

        //Get the blob name from the orchestrator
        //use a blob input binding to read the contents of the blob and
        // write the contents to a table storage
        [FunctionName("A_WriteToTableStrg")]
        [return: Table("demotable")]
        public static async Task<MyClass> A_WriteToTableStrg(
          [ActivityTrigger] string path,
          [Blob("democont/{path}", FileAccess.Read)] Stream stream,
          ILogger logger)
        {
            var data = await new StreamReader(stream).ReadToEndAsync();
            return new MyClass { Data = data, RowKey = Guid.NewGuid().ToString(), PartitionKey = "demo" };
        }

        public class MyClass : TableEntity
        {
            public string Data { get; set; }
        }

    }
}
