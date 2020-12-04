using AzUnzipEverything.Processors;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(AzUnzipEverything.Startup))]

namespace AzUnzipEverything
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {                           
            builder.Services.AddSingleton<IArchiveProcessor, ZipProcessor>();
            builder.Services.AddSingleton<IArchiveProcessor, SevenZipProcessor>();
            builder.Services.AddSingleton<IArchiveProcessor, RarProcessor>();   

            builder.Services.AddSingleton<IArchiveProcessorFactory, ArchiveProcessorFactory>();    

            builder.Services.AddLogging();
            builder.Services.AddSingleton<ILogger>(sp=>
            {
                return sp.GetService<ILogger<Unzipthis>>();                
            });
        }        
    }
}