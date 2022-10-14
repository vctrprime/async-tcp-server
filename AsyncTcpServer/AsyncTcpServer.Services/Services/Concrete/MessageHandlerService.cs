using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncTcpServer.Services.Services.Abstract;

namespace AsyncTcpServer.Services.Services.Concrete
{
    public class MessageHandlerService : IMessageHandlerService
    {
        public async Task<string> Handle(string message)
        {
            var messageData = message.Replace("\n", "");
            
            if (messageData.Length < 2) throw new ArgumentException("Ошибка входящего сообщения!");

            var commandNumber = messageData[0];
            messageData = messageData.Substring(1, messageData.Length - 1);
            

            var result = commandNumber switch
            {
                '1' => new string(messageData.Reverse().ToArray()),
                '2' => messageData.ToUpper(),
                '3' => messageData.ToLower(),
                _ => throw new ArgumentException("Такой команды не существует!")
            };

            //имитация долгого выполнения действия
            int delayDuration = Convert.ToInt32(commandNumber.ToString()) * 1000;
            await Task.Delay(delayDuration);

            return result;
        }
    }
}