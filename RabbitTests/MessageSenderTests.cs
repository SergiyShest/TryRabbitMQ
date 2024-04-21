
using Moq;
using NLog;
using NUnit.Framework;
using RabbitMQ.Client;
using Assert = NUnit.Framework.Assert;

namespace Tests
{
    [TestFixture]
    public class RabbitTests
    {
        string QueueName = "testQueue";
        private readonly MessageSender sender;
        private readonly MessageReceiver receiver;

        public RabbitTests()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Должно быть настроено или мокировано
            var logger = LogManager.GetCurrentClassLogger(); 
            sender = new MessageSender(factory, logger);
            receiver = new MessageReceiver(factory, logger);
        }

        [TestCase("testMessage")]
        public async Task SendMessageTest(string message)
        {
            var prevMessCount = await getMessageCount();
            await SendMessage(message);
            await Task.Delay(1000); // Сократить задержку после улучшения механизма ожидания
            var curMessCount = await getMessageCount();
            Assert.That(prevMessCount + 1== curMessCount, $"Message count did not increase as expected. {prevMessCount + 1} == {curMessCount} ");
        }

        private async Task SendMessage(string message)
        {
            await sender.SendMessageAsync(QueueName, message);
        }

        private async Task<uint> getMessageCount()
        {
            return await receiver.GetMessageCountAsync(QueueName);
        }

        [Test]
        public async Task ReceiveMessageTest()
        {
            var message = Guid.NewGuid().ToString();
            receiver.AddListener(QueueName);
            await SendMessage(message);
            await Task.Delay(1000); // Уменьшить время ожидания
            var exists = receiver.Messages.Any(m => m.Message == message);
            Assert.That(exists, "Message was not received.");
        }
    }

}