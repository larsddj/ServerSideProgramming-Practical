using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Queues; 
using Azure.Storage.Queues.Models;
using System.Configuration;
using System.Buffers.Text;
using ImageTextFunc.Helper;
using System.Security.Cryptography;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ImageTextFunc
{
    public static class Startup
    {
        [FunctionName("image")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "image/")] HttpRequest req,
            ILogger log)
        {
            IdHelper idHelper = new IdHelper();
            log.LogInformation("C# HTTP trigger function processed a request.");

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string queueName = "ssp-image-queue";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            await queue.CreateIfNotExistsAsync();

            string search = req.Query["search"];

            string guid = idHelper.generateID();
            byte[] preEncode = Encoding.ASCII.GetBytes(search +"-"+ guid);
            string encoded = Convert.ToBase64String(preEncode);

            string responseMessage = string.IsNullOrEmpty(search)
                ? "Please pass a search term using the search parameter in the request body"
                : $"https://ssp608694.blob.core.windows.net/api-images/{search}-{guid}.bmp";
            if (!string.IsNullOrEmpty(search))
                //TO-DO this string is hardcoded right now, search object needs to be base64 encoded first for the queue to handle it properly
                await queue.AddMessageAsync(new CloudQueueMessage(encoded));

            return new OkObjectResult(responseMessage);
        }
    }
}
