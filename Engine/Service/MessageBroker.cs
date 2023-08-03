using System;
using Engine.EventArgs;

namespace Engine.Service
{
    public class MessageBroker
    {
        protected static readonly MessageBroker _messageBroker = new MessageBroker();

        private MessageBroker()
        {

        }

        public event EventHandler<GameMessageEventArgs> OnMessageRaised;
        public static MessageBroker GetInstance()
        {
            return _messageBroker;
        }

        internal void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }
    }
}
