using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncTcpServer.App;
using AsyncTcpServer.Domain.Config.Abstract;
using AsyncTcpServer.Domain.Config.Concrete;
using AsyncTcpServer.Services.Services.Abstract;
using AsyncTcpServer.Services.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace AsyncTcpServer.IntegrationTests
{
    public class ClientServerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IAppConfig _appConfig;
        private readonly IMessageHandlerService _messageHandlerService;
        private readonly ILogger<Startup> _logger;

        private const int Port = 4991;
        private const string Host = "127.0.0.1";

        public ClientServerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _appConfig = GetConfig();
            _messageHandlerService = new MessageHandlerService();
            _logger = Mock.Of<ILogger<Startup>>();
            
            
        }

        [Theory]
        [InlineData("1test", "tset")]
        [InlineData("2test", "TEST")]
        [InlineData("3TEST", "test")]
        public void ConnectAndSendMessageReturnRightResponse(string message, string expected)
        {
            var server = new Startup(_appConfig, _logger, _messageHandlerService);
            server.Run();
            
            var response = string.Empty;
            var task = Task.Factory.StartNew(() =>
            {
                response = ConnectAndGetResponse(message);
            });

            task.Wait();
            
            Assert.Equal(expected, response);
            server.Stop();
        }
        
        [Fact]
        public void ExecuteTwoClientsCalls()
        {
            var server = new Startup(_appConfig, _logger, _messageHandlerService);
            server.Run();
            
            var tasks = new List<Task>
            {
                Task.Factory.StartNew( () =>
                {
                    Thread.Sleep(500);
                    ParallelMessage("1test", "Клиент 1");
                }),
                Task.Factory.StartNew( () =>
                {
                    ParallelMessage("3TEST", "Клиент 2");
                }),
                Task.Factory.StartNew( () =>
                {
                    ParallelMessage("2test", "Клиент 3");
                }),
                Task.Factory.StartNew( () =>
                {
                    ParallelMessage("2test", "Клиент 4");
                }),
            }.ToArray();

            Task.WaitAll(tasks);
            
            server.Stop();
        }
        
        private IAppConfig GetConfig()
        {
            var services = new ServiceCollection();
            var json = "{\"serverConfig\": {\"port\": " + Port +", \"host\": \"" + Host +"\"}}";
            
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(json)))
                .Build();
            services.AddSingleton<IAppConfig>
                (configuration.Get<AppConfig>(options => options.BindNonPublicProperties = true));
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetService<IAppConfig>();
        }


        private void ParallelMessage(string message, string clientName)
        {
            _testOutputHelper.WriteLine($"{clientName}: {message}");
            var response = ConnectAndGetResponse(message);
            _testOutputHelper.WriteLine($"{clientName}: ответ на {message} - {response}");
             
        }
        
        private string ConnectAndGetResponse(string message)
        {
            using var client = new TcpClient();
            
            client.Connect(Host, Port);
            
            var stream = client.GetStream();
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);

            sw.AutoFlush = true;
            sw.WriteLine(message);
            string response = sr.ReadLine();
            
            stream.Close();
            client.Close();
            
            return response;
        }
    }
}