using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AsyncTcpServer.Domain.Config.Abstract;
using AsyncTcpServer.Domain.Config.Sections;
using AsyncTcpServer.Services.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace AsyncTcpServer.App
{
    public class Startup
    {
        private readonly ServerConfig _config;
        private readonly ILogger _logger;
        private readonly IMessageHandlerService _messageHandlerService;
        
        static TcpListener _listener;
        readonly List<Client> _clients = new ();

        public Startup(IAppConfig appConfig, ILogger<Startup> logger,
            IMessageHandlerService messageHandlerService)
        {
            _config = appConfig.ServerConfig;
            _logger = logger;
            _messageHandlerService = messageHandlerService;
        }

        public async Task Run()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse(_config.Host), _config.Port);
                _listener.Start();

                _logger.LogInformation("Сервер запущен");

                _logger.LogInformation("Ожидание подключений...");
                
                while (true)
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    Client client = new Client(_logger, tcpClient, _messageHandlerService)
                        .SetClientName($"Клиент {_clients.Count+1}");

                    _clients.Add(client);
                    _logger.LogWarning($"Активные клиенты:\n{string.Join("\n", _clients.Where(c => c.IsActive).Select(c => c.ClientName))}");
                    
                    Task clientTask = new Task(client.Process);
                    clientTask.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Startup));
            }
            finally
            {
                _listener?.Stop();
            }
        }
    }
}