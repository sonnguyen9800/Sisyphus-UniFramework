using System;
using System.Collections.Generic;
// Original reference: https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern

namespace SisyphusLab.Notifier
{
    public class Publisher<T> : IObservable<T>
    {

        private readonly HashSet<IObserver<T>> _observers = new();
        private readonly HashSet<T> _observer = new();
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_observers.Add(observer))
            {
                // Provide observer with existing data.
                foreach (T item in _observer)
                {
                    observer.OnNext(item);
                }
            }
            return new Unsubscriber<T>(_observers, observer);
        }
    }
    
    internal sealed class Unsubscriber<T> : IDisposable
    {
        private readonly ISet<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        internal Unsubscriber(
            ISet<IObserver<T>> observers,
            IObserver<T> observer) => (_observers, _observer) = (observers, observer);

        public void Dispose() => _observers.Remove(_observer);
    }
    
    public class Subscriber<T> : IObservable<T>
    {
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }
    }
}