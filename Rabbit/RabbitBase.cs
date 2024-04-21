using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Rabbit
{
    public abstract class RabbitBase : IDisposable
    {
        protected readonly ConnectionFactory factory;
        private IConnection _connection;
        private IModel _channel;
        private bool _disposed = false;
        ILogger _logger;
        protected ILogger Logger { get { if (_logger == null) { 
                _logger = LogManager.GetCurrentClassLogger();
                }
                return _logger;
            } }

        protected IConnection Connection => _connection ??= factory.CreateConnection();

        protected IModel Channel => _channel ??= Connection.CreateModel();

        public RabbitBase(ConnectionFactory factory,ILogger log = null)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        public RabbitBase(string host,ILogger log = null)
        {
            this.factory = new ConnectionFactory() { HostName = host }; 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _channel?.Dispose();
                    _connection?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

 
}
