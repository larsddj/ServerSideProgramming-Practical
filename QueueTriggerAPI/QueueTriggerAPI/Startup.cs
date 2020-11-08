using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using QueueTriggerAPI.Model;
using QueueTriggerAPI.Helper;
using System.Text;

namespace QueueTriggerAPI
{
    public static class Startup
    {

        [FunctionName("Function1")]
        public static void Run([QueueTrigger("ssp-image-queue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            ImageTextHelper imageTextHelper = new ImageTextHelper();
            BlobHelper blobHelper = new BlobHelper();
            MessageSeperator messageSeperator = new MessageSeperator();

            //surround queue process in trycatch in case a queue item is not correctly set
            try
            {
                byte[] queueItemByteArray = Convert.FromBase64String(myQueueItem);
                string decodedString = Encoding.UTF8.GetString(queueItemByteArray);
                QueueItem queueItem = messageSeperator.Seperate(decodedString);
                string searchResult = GetTextFromApi(queueItem.message_).Result;
                Image imageResult =  GetImageFromApi(queueItem.message_).Result;
                Image mergedImage = imageTextHelper.Merge(searchResult, imageResult).Result;
                blobHelper.ExecuteBlobLogic(mergedImage, queueItem);
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }


        }

        static async Task<string> GetTextFromApi(string search)
        {
            HttpClient client = new HttpClient();
            string url = "https://api.datamuse.com";
            string urlParam = $"/words?ml={search}";

            HttpResponseMessage response = client.GetAsync(url+urlParam).Result;
            if (response.IsSuccessStatusCode)
            {
                // parse the response body.
                var jObjects = response.Content.ReadAsAsync<IEnumerable<JObject>>().Result;
                if (jObjects != null)
                {
                    // retrieve the first result from the JSON list and get the word from it
                    JObject firstResult = jObjects.ElementAt(0);
                    JToken value;
                    if (firstResult.TryGetValue("word", out value))
                    {
                        return value.ToString();
                    }
                }
            }
            return "No valid JSON returned";
        }

        
        static async Task<Image> GetImageFromApi(string search)
        {
            ImageHelper imageHelper = new ImageHelper();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"https://api.unsplash.com/");
            client.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"photos/random/?query={search}&per_page=1&client_id" +
                "=rf_6-RapB2gSfjfL_T-zE6XrPSfSPyZZ4o3Z3R4N5o8");

            HttpResponseMessage response = client.SendAsync(request).Result;
            var imageJson = await response.Content.ReadAsStringAsync();
            UnsplashJsonImage deserializedImageJson = JsonConvert.DeserializeObject<UnsplashJsonImage>(imageJson);

            Image image = imageHelper.GetImage(deserializedImageJson.urls.small);

            return image;
        }
        
    }
}
