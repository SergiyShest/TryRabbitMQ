using NLog;
using Rabbit;
using RabbitMQ.Client;
using System.Text;

public class MessageSender : RabbitBase
{


    public MessageSender(ConnectionFactory factory, ILogger log) : base(factory, log)
    { }
       
     public MessageSender(string host, ILogger log = null): base(host,log)
    {
        
    }



    public void SendMessage(string queueName, string message)
    {
        ConfigureQueue(queueName);
        var body = Encoding.UTF8.GetBytes(message);
        Channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        Logger.Info("Message sent synchronously");
    }

    public async Task SendMessageAsync(string queueName, string message)
    {
        await Task.Run(() => SendMessage(queueName, message));
        Logger.Info("Message sent asynchronously");
    }

    private void ConfigureQueue(string queueName)
    {
        Channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
}



