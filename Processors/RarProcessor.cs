using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives.Rar;

namespace AzUnzipEverything.Processors
{
    public class RarProcessor : ArchiveProcessor
    {
        public RarProcessor(ILogger log)
            : base(log, ".rar")
        {
        }

        public override async Task ProcessAsync(CloudBlockBlob source, CloudBlobContainer target)
        {
            using(var sourceStream = await source.OpenReadAsync())
            {
                using(var archive = RarArchive.Open(sourceStream))
                {
                    foreach(var entry in archive.Entries)
                    {                            
                        //Replace all NO digits, letters, or "-" by a "-" Azure storage is specific on valid characters
                        var targetFileName = Regex.Replace(entry.Key, @"[^a-zA-Z0-9\-.]","-").ToLower();
                        var targetBlob = target.GetBlockBlobReference(targetFileName);

                        using(var sourceFileStream = entry.OpenEntryStream())
                        {
                            Log.LogInformation($"started extracting {entry.Key} ({entry.CompressedSize}) to {targetFileName} ({entry.Size})");
                            await targetBlob.UploadFromStreamAsync(sourceFileStream);
                            Log.LogInformation($"completed extracting {entry.Key} to {targetFileName}");
                        }
                    }
                }
            }
        }
    }
}