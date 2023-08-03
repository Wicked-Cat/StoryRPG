using System;
using Engine.Models;
using Engine.Service;

namespace Engine.Actions
{
    public abstract class BaseAction
    {
        protected readonly Item _itemInUse;
        public event EventHandler<string> OnActionPerformed;
        protected readonly MessageBroker _messageBroker = MessageBroker.GetInstance();

        protected BaseAction(
            Item itemInUse)
        {
            _itemInUse = itemInUse;
        }
    }
}
