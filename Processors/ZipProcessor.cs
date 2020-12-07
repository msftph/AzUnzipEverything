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
    }
}