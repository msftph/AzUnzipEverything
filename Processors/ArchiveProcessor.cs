using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SharpCompress.Archives;

namespace AzUnzipEverything.Processors
{

    /// <summary>
    /// ArchiveProcessor takes in a blob and returns many blobs
    /// </summary>
    public abstract class ArchiveProcessor : IArchiveProcessor
    {
        public string Extension { get; }

        public ILogger Log { get; }

        protected ArchiveProcessor(ILogger log, string extension)
        {
            this.Log = log;
            this.Extension = extension;
        }

        public virtual async Task ProcessAsync(CloudBlockBlob source, CloudBlobContainer target)
        {
            using(var sourceStream = await source.OpenReadAsync())
            {
                using(var archive = ArchiveFactory.Open(sourceStream))
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