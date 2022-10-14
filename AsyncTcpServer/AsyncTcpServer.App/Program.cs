using System;
using System.Threading.Tasks;
using AsyncTcpServer.App.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AsyncTcpServer.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)
                    .Build();

                
                var host = Host.CreateDefaultBuilder() 
                    .ConfigureServices((context, services) => { 
                        services
                            .RegisterModules(configuration)
                            .BuildServiceProvider(true);
                    })
                    .UseSerilog() 
                    .Build(); 

                var scope = host.Services.CreateScope();

                await scope.ServiceProvider.GetRequiredService<Startup>().Run();
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error starting program: {e}");
            }
        }
    }
}