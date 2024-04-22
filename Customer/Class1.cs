using RabbitMQ.Client.Events;

namespace RabbitMQ.Client
{
    /// <summary>
    /// Общая модель AMQP, охватывающая объединение функциональности,
    /// предлагаемой версиями AMQP 0-8, 0-8qpid, 0-9 и 0-9-1.
    /// </summary>
    /// <remarks>
    /// Расширяет интерфейс <see cref="IDisposable"/>, так что оператор "using"
    /// может использоваться для определения времени жизни канала, когда это уместно.
    /// </remarks>
    public interface IModel : IDisposable
    {
        /// <summary>
        /// Номер канала, уникальный для каждого подключения.
        /// </summary>
        int ChannelNumber { get; }

        /// <summary>
        /// Возвращает null, если сессия все еще находится в состоянии, когда ее можно использовать,
        /// или причину ее закрытия в противном случае.
        /// </summary>
        ShutdownEventArgs CloseReason { get; }

        /// <summary>
        /// Указывается, когда доставляется неожиданное сообщение.
        ///
        /// В некоторых обстоятельствах канал может получить доставку сообщения,
        /// которое не соответствует ни одному потребителю, который в настоящее время
        /// настроен через basicConsume(). Это происходит после следующей последовательности событий:
        ///
        /// ctag = basicConsume(queue, consumer); // т.е. с явным подтверждением
        /// // происходит некоторая доставка, но без подтверждения
        /// basicCancel(ctag);
        /// basicRecover(false);
        ///
        /// Поскольку в basicRecover указано, что повторное помещение в очередь не производится, спецификация
        /// гласит, что сообщение должно быть повторно доставлено «оригинальному получателю»
        /// - т.е. тому же каналу / тегу потребителя. Но потребитель больше не активен.
        ///
        /// В этих обстоятельствах вы можете зарегистрировать потребителя по умолчанию для обработки
        /// таких доставок. Если потребитель по умолчанию не зарегистрирован, при поступлении такой доставки
        /// будет выброшено исключение InvalidOperationException.
        ///
        /// Большинству людей это не понадобится.</summary>
        IBasicConsumer DefaultConsumer { get; set; }

        /// <summary>
        /// Возвращает true, если модель больше не находится в состоянии, когда ее можно использовать.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Возвращает true, если модель все еще находится в состоянии, когда ее можно использовать.
        /// То же самое, что и проверка, равно ли <see cref="CloseReason"/> null.</summary>
        bool IsOpen { get; }

        /// <summary>
        /// В режиме подтверждения возвращает порядковый номер следующего сообщения, которое будет опубликовано.
        /// </summary>
        ulong NextPublishSeqNo { get; }

        /// <summary>
        /// Имя последней объявленной в этом канале очереди.
        /// </summary>
        /// <remarks>
        /// Ссылка: https://www.rabbitmq.com/amqp-0-9-1-reference.html#domain.queue-name
        /// </remarks>
        string CurrentQueue { get; }

        /// <summary>
        /// Сигнализируется, когда от брокера приходит команда Basic.Ack.
        /// </summary>
        event EventHandler<BasicAckEventArgs> BasicAcks;

        /// <summary>
        /// Сигнализируется, когда от брокера приходит команда Basic.Nack.
        /// </summary>
        event EventHandler<BasicNackEventArgs> BasicNacks;

        /// <summary>
        /// Все сообщения, полученные до этого события, которые не были подтверждены, будут повторно доставлены.
        /// Все сообщения, полученные после этого, не будут.
        /// </summary>
        /// <remarks>
        /// Обработчики этого события вызываются потоком подключения.
        /// Иногда полезно позволить этому потоку знать, что был получен recover-ok,
        /// а не поток, который вызвал <see cref="BasicRecover"/>.
        /// </remarks>
        event EventHandler<EventArgs> BasicRecoverOk;

        /// <summary>
        /// Сигнализируется, когда от брокера приходит команда Basic.Return.
        /// </summary>
        event EventHandler<BasicReturnEventArgs> BasicReturn;

        /// <summary>
        /// Сигнализируется, когда в обратном вызове, вызванном моделью, происходит исключение.
        ///
        /// Примеры случаев, когда это событие будет сигнализировано,
        /// включают исключения, возникающие в методах <see cref="IBasicConsumer"/>, или
        /// исключения, возникающие в делегатах <see cref="ModelShutdown"/> и т. д.
        /// </summary>
        event EventHandler<CallbackExceptionEventArgs> CallbackException;

        event EventHandler<FlowControlEventArgs> FlowControl;

        /// <summary>
        /// Уведомляет о разрушении модели.
        /// </summary>
        /// <remarks>
        /// Если модель уже разрушена на момент добавления обработчика событий к этому событию,
        /// обработчик событий будет активирован немедленно.
        /// </remarks>
        event EventHandler<ShutdownEventArgs> ModelShutdown;

        /// <summary>
        /// Прервать эту сессию.
        /// </summary>
        /// <remarks>
        /// Если сессия уже закрыта (или закрывается), то этот
        /// метод ничего не делает, кроме как ждет завершения процесса закрытия.
        /// Этот метод не вернется вызывающему абоненту, пока завершение не будет полным.
        /// В отличие от обычного метода <see cref="Close()"/>, метод <see cref="Abort()"/> не будет выбрасывать
        /// <see cref="Exceptions.AlreadyClosedException"/> или <see cref="System.IO.IOException"/> или любое другое <see cref="Exception"/> во время закрытия модели.
        /// </remarks>
        //[AmqpMethodDoNotImplement(null)]
        void Abort();

        /// <summary>
        /// Прервать эту сессию.
        /// </summary>
        /// <remarks>
        /// Метод ведет себя так же, как <see cref="Abort()"/>, с той лишь разницей, что модель закрывается с данным кодом закрытия модели и сообщением.
        /// <para>
        /// Код закрытия (см. раздел "Коды ответов" в спецификации AMQP)
        /// </para>
        /// <para>
        /// Сообщение, указывающее причину закрытия модели
        /// </para>
        /// </remarks>
       // [AmqpMethodDoNotImplement(null)]
        void Abort(ushort replyCode, string replyText);
    }
}
