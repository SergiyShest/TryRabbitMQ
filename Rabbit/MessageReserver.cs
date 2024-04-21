using NLog;
using Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;

public class MessageReceiver : RabbitBase
{
  

    public ConcurrentBag<RabbitMessage> Messages { get; } = new ConcurrentBag<RabbitMessage>();

    public MessageReceiver(ConnectionFactory factory, ILogger log = null) : base(factory,log)
    {
       
    }

    public MessageReceiver(string host, ILogger log = null) : base(host,log)
    {
       
    }


    public void AddListener(string queue)
    {
        Channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new EventingBasicConsumer(Channel);
        consumer.Received += Consumer_Received;
        Channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
    }

    public async Task<uint> GetMessageCountAsync(string queue)
    {
        try
        {
            var result = await Task.Run(() => Channel.BasicGet(queue, false));
            if (result != null)
            {
                Channel.BasicNack(result.DeliveryTag, false, true);
                return result.MessageCount;
            }
            return 0;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return 0;
        }
    }

    private void Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Messages.Add(new RabbitMessage() { Message = message });
        Logger.Info($"Received: {message}");
    }

    public class RabbitMessage
    {
        public string Message { get; set; }
    }
}
