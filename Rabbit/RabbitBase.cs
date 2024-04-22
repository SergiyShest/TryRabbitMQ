using NLog;
using RabbitMQ.Client;
namespace Rabbit
{
    public abstract class RabbitBase : IDisposable
    {
        protected readonly IAsyncConnectionFactory factory;
        private IConnection _connection;
        private IModel _channel;
        private bool _disposed = false;
        ILogger? _logger;
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetCurrentClassLogger();
                }
                return _logger;
            }
        }

        protected IConnection Connection => _connection ??= factory.CreateConnection();

        protected IModel Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = Connection.CreateModel();
                }
                return _channel;
            }
            set
            {
                if (_channel != null)
                {
                    _channel.Dispose();
                }
                _channel = value;
            }
        }

        public RabbitBase(IAsyncConnectionFactory factory, ILogger? log = null)
        {
            _logger = log;
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        public RabbitBase(string host, ILogger? log = null)
        {
            _logger = log;
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
