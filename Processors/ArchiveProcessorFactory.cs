using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzUnzipEverything.Processors
{
    /// <summary>
    /// ArchiveProcessorFactory creates an archive processor from the given format
    /// </summary>
    public class ArchiveProcessorFactory : IArchiveProcessorFactory
    {
        private IDictionary<string, IArchiveProcessor> processors;
        private ILogger log;

        private ArchiveProcessorFactory(ILogger log)
        {
            this.processors = new Dictionary<string, IArchiveProcessor>();
            this.log = log;
        }

        public ArchiveProcessorFactory(ILogger log, IEnumerable<IArchiveProcessor> processors)
            : this(log)
        {
            foreach(var processor in processors)
                Register(processor);
        }
        
        public void Register(IArchiveProcessor processor)
        {            
            this.processors.Add(processor.Extension, processor);
        }

        public bool Contains(string extension)
        {
            return processors.ContainsKey(extension);
        }

        public IArchiveProcessor Create(string extension)
        {
            return processors[extension];
        }
    }
}