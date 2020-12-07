using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives.SevenZip;
using System.IO;
using System;
using Microsoft.WindowsAzure.Storage;

namespace AzUnzipEverything.Processors
{
    public class SevenZipProcessor : ArchiveProcessor
    {
        public SevenZipProcessor(ILogger log)
            : base(log, ".7z")
        {
        }

        public override async Task ProcessAsync(CloudBlockBlob source, CloudBlobContainer target)
        {
            using(var sourceStream = await source.OpenReadAsync())
            {
                using(var archive = SevenZipArchive.Open(sourceStream))
                {
                    foreach(var entry in archive.Entries)
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