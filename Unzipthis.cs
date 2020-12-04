using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzUnzipEverything.Processors;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzUnzipEverything
{
    public class Unzipthis
    {
        private readonly IArchiveProcessorFactory factory;
        private readonly ILogger log;

        public Unzipthis(ILogger<Unzipthis> log, IArchiveProcessorFactory factory)
        {
            this.log = log;
            this.factory = factory;
        }
        
        [FunctionName("Unzipthis")]
        public async Task Run(
            [BlobTrigger("input-files/{name}", Connection = "cloud5mins_storage")]CloudBlockBlob blob,
            string name)
        {
            log.LogInformation($"C# Blob trigger function Processed blob Name: {name}");

            // exit if not supported format
            var extension = Path.GetExtension(name).ToLower();
            if(!factory.Contains(extension))
            {
                log.LogError($"{extension} is not a supported extension");
                return;           
            }

            var destinationStorage = Environment.GetEnvironmentVariable("destinationStorage");
            var destinationContainer = Environment.GetEnvironmentVariable("destinationContainer");
         
            try
            {  
                var storageAccount = CloudStorageAccount.Parse(destinationStorage);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(destinationContainer);

                var processor = factory.Create(extension);
                await processor.ProcessAsync(blob, container);                
            }
            catch(Exception ex){                
                log.LogError(ex, ex.Message);
            }            
        }
    }
}