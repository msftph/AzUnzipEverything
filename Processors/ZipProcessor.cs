using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System.IO;
using System;

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
                using(var zipArchive = new ZipArchive(sourceStream))
                {
                    foreach(var zipArchiveEntry in zipArchive.Entries)
                    {                            
                        //Replace all NO digits, letters, or "-" by a "-" Azure storage is specific on valid characters
                        var targetFileName = Regex.Replace(zipArchiveEntry.Name, @"[^a-zA-Z0-9\-.]","-").ToLower();                            
                        var targetBlob = target.GetBlockBlobReference(targetFileName);

                        using(var sourceFileStream = zipArchiveEntry.Open())
                        {
                            Log.LogInformation($"started extracting {zipArchiveEntry.Name} ({zipArchiveEntry.CompressedLength}) to {targetFileName} ({zipArchiveEntry.Length})");
                            // await targetBlob.UploadFromStreamAsync(sourceFileStream);
                            using(var targetStream = await targetBlob.OpenWriteAsync())
                            {
                                await CopyWithProgressAsync(sourceFileStream, targetStream, zipArchiveEntry.Length, onCopyProgress=>
                                {
                                    Log.LogInformation($"{zipArchiveEntry.Name} extracting {targetFileName} progress {onCopyProgress.Current} of {onCopyProgress.Total} bytes");
                                });
                            }                            
                            Log.LogInformation($"completed extracting {zipArchiveEntry.Name} to {targetFileName}");                            
                        }
                    }
                }
            }
        }

        private async Task CopyWithProgressAsync(Stream source, Stream target, long uncompressedLength, Action<CopyProgressEvent> onProgress)
        {     
            var current = 0L;
            var read = 0;
            var buffer = new byte[16 * 1024];

            do{
                read = await source.ReadAsync(buffer, 0, buffer.Length);
                current += read;
                await target.WriteAsync(buffer, 0, read);
                onProgress(new CopyProgressEvent
                {
                    Total = uncompressedLength,
                    Current = current
                });
            }while(read == buffer.Length);
        }
    }
}