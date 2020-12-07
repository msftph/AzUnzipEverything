using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using SharpCompress.Archives.Zip;
using Microsoft.WindowsAzure.Storage;

namespace AzUnzipEverything.Processors
{
    public class ZipProcessor : ArchiveProcessor
    {
        public ZipProcessor(ILogger log)
            : base(log,".zip")
        {
        }

        public override async Task ProcessAsync(CloudBlockBlob source, CloudBlobContainer target)
        {
            using(var sourceStream = await source.OpenReadAsync())
            {
                using(var zipArchive = ZipArchive.Open(sourceStream))
                {
                    foreach(var entry in zipArchive.Entries)
                    {                            
                        //Replace all NO digits, letters, or "-" by a "-" Azure storage is specific on valid characters
                        var targetFileName = Regex.Replace(entry.Key, @"[^a-zA-Z0-9\-.]","-").ToLower();                            
                        var targetBlob = target.GetBlockBlobReference(targetFileName);

                        using(var sourceFileStream = entry.OpenEntryStream())
                        {                            
                            Log.LogInformation($"started extracting {entry.Key} ({entry.CompressedSize}) to {targetFileName} ({entry.Size})");
                            targetBlob.StreamWriteSizeInBytes = 104857600;
                            await targetBlob.UploadFromStreamAsync(sourceFileStream);                                                     
                            Log.LogInformation($"completed extracting {entry.Key} to {targetFileName}");                            
                        }
                    }
                }
            }
        }
    }
}