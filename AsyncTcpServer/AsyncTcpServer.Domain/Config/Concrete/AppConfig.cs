using AsyncTcpServer.Domain.Config.Abstract;
using AsyncTcpServer.Domain.Config.Sections;

namespace AsyncTcpServer.Domain.Config.Concrete
{
    public class AppConfig : IAppConfig
    {
        public ServerConfig ServerConfig { get; private set; }
    }
}