using System;
using DevExpress.Mvvm;

namespace Sidebar.Messages
{
    public class Mediator
    {
        public static void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Messenger.Default.Register(recipient, action);
        }

        public static void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            Messenger.Default.Unregister(recipient, action);
        }

        public static void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Messenger.Default.Register(recipient, token, action);
        }

        public static void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Messenger.Default.Unregister(recipient, token, action);
        }

        public static void Send<TMessage>(TMessage message)
        {
            Messenger.Default.Send(message);
        }

        public static void Send<TMessage>(TMessage message, object token)
        {
            Messenger.Default.Send(message, token);
        }
    }
}
