using System.Threading.Tasks;

namespace AsyncTcpServer.Services.Services.Abstract
{
    public interface IMessageHandlerService
    {
        Task<string> Handle(string message);
    }
}