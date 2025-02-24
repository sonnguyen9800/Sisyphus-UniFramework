using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisyphusFramework.Notifier
{
    /// <summary>
    /// Basic publisher. Has no support for param when Notify()
    /// </summary>
    /// <typeparam name="T">Enum for Notify</typeparam>
    public class Publisher<T> where T : System.Enum
    {
        private readonly HashSet<IObserver<T>> _observers = new();
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_observers.Add(observer))
                return new Unsubscriber<T>(_observers, observer);
            return null;
        }

        public void NotifyParallel(T command)
        {
            // Use parallel programing to boots performance
            Parallel.ForEach(_observers, (observer) =>
            {
                observer.OnNext(command);
            });
        }
        public void NotifySequence(T command)
        {
            foreach (var iter in _observers.ToList())
            {
                iter.OnNext(command);
            }
        }
    }
    internal sealed class Unsubscriber<T> : IDisposable
    {
        private readonly ISet<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        internal Unsubscriber(ISet<IObserver<T>> observers,
            IObserver<T> observer) => (_observers, _observer) = (observers, observer);

        public void Dispose() => _observers.Remove(_observer);
    }
}