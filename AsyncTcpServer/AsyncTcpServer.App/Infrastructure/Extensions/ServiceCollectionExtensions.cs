using System;
using AsyncTcpServer.Domain.Config.Abstract;
using AsyncTcpServer.Domain.Config.Concrete;
using AsyncTcpServer.Services.Services.Abstract;
using AsyncTcpServer.Services.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AsyncTcpServer.App.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрация необходимых зависимостей
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection RegisterModules(this IServiceCollection services, IConfiguration config)
        {
            services
                .RegisterConfiguration(config)
                .RegisterServices();

            RegisterLogger();
                

            return services;
        }
        
        /// <summary>
        /// Регистрация провайдера конфигурации приложения.
        /// </summary>
        private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config);
            services.AddSingleton<IAppConfig>
                (config.Get<AppConfig>(options => options.BindNonPublicProperties = true));

            return services;
        }
        
        /// <summary>
        /// Регистрация сервисов
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<Startup>();
            
            services.AddTransient<IMessageHandlerService, MessageHandlerService>();
            
            return services;
        }
        
        private static void RegisterLogger()
        {
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"Logging Process Error: {msg}"));
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}