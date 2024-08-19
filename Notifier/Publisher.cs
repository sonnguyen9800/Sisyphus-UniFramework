using System;
using System.Collections.Generic;
using Unity.VisualScripting;

// Original reference: https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern
// TODO: improve, test, verify
namespace SisyphusLab.Notifier
{
    public class Publisher<T, TParam> : ISisyphusObservable<T, TParam> where T : IComparable<T>
    {
        private SortedSet<IObserver<T>> _observers = new();

        SortedSet<IObserver<T>> ISisyphusObservable<T, TParam>.Observers
        {
            get => _observers;
        }

        public Publisher()
        {
            _observers = new SortedSet<IObserver<T>>();
        }

        public (IDisposable, bool) Subscribe(IObserver<T> newObserver)
        {
            if (_observers.Add(newObserver))
                return (new Unsubscriber<T>(_observers, newObserver), true);
            return (null, false);
        }

        public (IDisposable, bool) Subscribe(IObserver<T> newObserver, IEnumerable<T> predefinedIInfos)
        {
            if (_observers.Add(newObserver))
            {
                foreach (var priorInfo in predefinedIInfos)    
                {
                    newObserver.OnNext(priorInfo);
                }
                return (new Unsubscriber<T>(_observers, newObserver), true);
            }
            return (null, false);
            
        }

        public virtual void Notify(TParam param)
        {
            
        }

        public virtual void Notify()
        {
            
        }
    }
    
    internal sealed class Unsubscriber<T> : IDisposable
    {
        private readonly System.Collections.Generic.ISet<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        internal Unsubscriber(System.Collections.Generic.ISet<IObserver<T>> observers,
            IObserver<T> observer) => (_observers, _observer) = (observers, observer);

        public void Dispose() => _observers.Remove(_observer);
    }

}