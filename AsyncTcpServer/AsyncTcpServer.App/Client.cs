using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AsyncTcpServer.Services.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace AsyncTcpServer.App
{
    public class Client
    {
        private readonly ILogger _logger;
        private readonly TcpClient _client;
        private readonly IMessageHandlerService _messageHandlerService;
        
        public string ClientName { get; private set; }

        public bool IsActive { get; private set; }
        
        private NetworkStream _stream;

        public Client(ILogger logger, TcpClient client, IMessageHandlerService messageHandlerService)
        {
            _logger = logger;
            _client = client;
            _messageHandlerService = messageHandlerService;
            
            IsActive = true;
        }
        
        public Client SetClientName(string clientName)
        {
            ClientName = clientName;
            return this;
        }


        public void Process()
        {
            _logger.LogWarning($"Подключился клиент: {ClientName}");
            try
            {

                _stream = _client.GetStream();

                while (true)
                {
                    byte[] buffer = new byte[200]; 
                    byte[] data;
                    
                    do
                    {
                        int bytes = _stream.Read(buffer, 0, buffer.Length);

                        if (bytes == 0) throw new Exception();
                        
                        data = new byte[bytes];
                        Array.Copy(buffer, data, bytes);
                        
                    } while (_stream.DataAvailable);

                    
                    async void HandleAction() => await Handle(data);

                    Task clientTask = new Task(HandleAction);
                    clientTask.Start();
                }
            }
            catch
            {
                _stream?.Close();
                _client?.Close();
                IsActive = false;
                _logger.LogWarning($"{ClientName} отключился");
            }



        }
        
        private async Task Handle(byte[] data)
        {
            string message = string.Empty,
                response = string.Empty;
            try
            {
                message = Encoding.UTF8.GetString(data);
                _logger.LogInformation($"{ClientName}: {message}");
                response = await _messageHandlerService.Handle(message);

            }
            catch (Exception ex)
            {
                response = $"Error: {ex.Message}";
            }
            finally
            {
                var responseData = Encoding.UTF8.GetBytes(response + "\n");
                _logger.LogInformation($"Подготовлен ответ на сообщение \"{message}\" для {ClientName}: {response}");
                _stream?.Write(responseData, 0, responseData.Length);
                
            }
        }
    }
}