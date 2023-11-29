using System;
using System.Collections.Generic;

namespace RichCanvasDemo.Services
{
    public sealed class MessengerService
    {
        private static readonly Lazy<MessengerService> _lazy =
        new Lazy<MessengerService>(() => new MessengerService());

        public delegate void MessageHandler(object message);

        private readonly Dictionary<MessageChannel, MessageHandler> _channelsHandlers = new Dictionary<MessageChannel, MessageHandler>();

        public static MessengerService Instance => _lazy.Value;

        private MessengerService() { }

        public void Register(MessageChannel channel, MessageHandler handler)
        {
            _channelsHandlers.Add(channel, handler);
        }

        public void Send<TMessage>(MessageChannel channel, TMessage message)
        {
            _channelsHandlers.TryGetValue(channel, out var handler);
            handler(message);
        }
    }
}
