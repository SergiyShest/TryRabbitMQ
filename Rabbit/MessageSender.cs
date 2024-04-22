using NLog;
using Rabbit;
using RabbitMQ.Client;
using System.Text;

public class MessageSender : RabbitBase
{


    public MessageSender(IAsyncConnectionFactory factory, ILogger? log) : base(factory, log)
    { }

    public MessageSender(string host, ILogger? log = null) : base(host, log)
    {

    }



    public void SendMessage(string queueName, string message, string exchange = "")
    {
        if (!string.IsNullOrEmpty(queueName)) { ConfigureQueue(queueName); }

        var body = Encoding.UTF8.GetBytes(message);
        try
        {

            Channel.BasicPublish(exchange: exchange, routingKey: queueName, basicProperties: null, body: body);
            Logger.Info($"Message {message} sent!");

        }
        catch (Exception ex)
        {
            Logger.Info($"{ex}");
        }
    }

    public async Task SendMessageAsync(string queueName, string message)
    {
        await Task.Run(() => SendMessage(queueName, message));

    }

    private void ConfigureQueue(string queueName)
    {
        Channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
}



