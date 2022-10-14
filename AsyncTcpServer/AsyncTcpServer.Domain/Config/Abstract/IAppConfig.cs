using AsyncTcpServer.Domain.Config.Sections;

namespace AsyncTcpServer.Domain.Config.Abstract
{
    public interface IAppConfig
    {
        ServerConfig ServerConfig { get; }
    }
}