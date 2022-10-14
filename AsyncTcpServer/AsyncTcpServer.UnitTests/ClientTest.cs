using System.Net.Sockets;
using AsyncTcpServer.App;
using AsyncTcpServer.Services.Services.Abstract;
using AsyncTcpServer.Services.Services.Concrete;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AsyncTcpServer.UnitTests
{
    public class ClientTest
    {
        private readonly ILogger _logger;
        private readonly TcpClient _tcpClient;
        private readonly IMessageHandlerService _messageHandlerService;

        public ClientTest()
        {
            _logger = Mock.Of<ILogger<Startup>>();
            _tcpClient = new TcpClient();
            _messageHandlerService = new MessageHandlerService();
        }

        [Fact]
        public void SetNameRight()
        {
            var client = new Client(_logger, _tcpClient, _messageHandlerService).SetClientName("test");
            
            Assert.Equal("test", client.ClientName);
        }
        
        [Fact]
        public void NewClientIsActive()
        {
            var client = new Client(_logger, _tcpClient, _messageHandlerService);
            
            Assert.True(client.IsActive);
        }
    }
}