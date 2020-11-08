using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using QueueTriggerAPI.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QueueTriggerAPI.Helper
{
    class BlobHelper
    {
        public void ExecuteBlobLogic(Image image, QueueItem search)
        {
            UploadToBlob(image, search);
        }

        private async Task UploadToBlob(Image image, QueueItem search)
        {
            string fileName = search.message_ +"-"+ search.id_+".bmp";
            string containerName = "api-images";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            
            CloudBlobClient blobClient = BlobAccountExtensions.CreateCloudBlobClient(storageAccount);
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = "image/jpg";
            using (var filestream = new System.IO.MemoryStream())
            {
                image.Save(filestream, ImageFormat.Bmp);
                filestream.Position = 0;

                blockBlob.UploadFromStream(filestream);
            }

        }
       
    }
}
