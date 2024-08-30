using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FAW.Core;

namespace SisyphusLab.Notifier
{
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
}