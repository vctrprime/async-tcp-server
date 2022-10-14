using System;
using System.Threading.Tasks;
using AsyncTcpServer.Domain;
using AsyncTcpServer.Services.Services.Abstract;
using AsyncTcpServer.Services.Services.Concrete;
using Xunit;

namespace AsyncTcpServer.UnitTests.Services
{
    public class MessageHandlerServiceTest
    {
        private readonly IMessageHandlerService _service;

        public MessageHandlerServiceTest()
        {
            _service = new MessageHandlerService();
        }

        [Theory]
        [InlineData("1test", "tset")]
        [InlineData("2test", "TEST")]
        [InlineData("3TEST", "test")]
        public async Task HandleReturnExpectedResult(string message, string expected)
        {
            var result = await _service.Handle(message);

            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("t")]
        public async Task HandleRaiseArgumentExceptionForIncorrectMessage(string message)
        {
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _service.Handle(message));
            Assert.Equal(Errors.INCORECT_MESSAGE, exception.Message);
        }
        
        [Fact]
        public async Task HandleRaiseArgumentExceptionForIncorrectCommand()
        {
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _service.Handle("4q"));
            Assert.Equal(Errors.INCORRECT_COMMAND, exception.Message);
        }
    }
}