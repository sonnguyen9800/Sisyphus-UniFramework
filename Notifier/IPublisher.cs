using System;

namespace SisyphusLab.Notifier
{
    public interface IPublisher<T> where T : System.Enum
    {
        public IDisposable Subscribe(IObserver<T> observer);
        public void Notify(T t);
    }
}