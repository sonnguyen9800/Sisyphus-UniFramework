using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SisyphusLab.Notifier
{
    /// <summary>
    /// This Publisher had support for param. If you want Publisher to
    /// have sorting observable by any order then use <see cref="SortedPublisher"/> instead
    /// </summary>
    /// <typeparam name="T">Command type to be notified (enum)</typeparam>
    /// <typeparam name="TParam">Param for each signal (struct)</typeparam>
    public class Publisher<T, TParam> : IObservable<T, TParam>
        where T: System.Enum
        where TParam : struct
    {
        private readonly HashSet<IObserver<T, TParam>> _observers = new();

        ISet<IObserver<T,TParam>> IObservable<T, TParam>.Observers => _observers;

        public (IDisposable, bool) TrySubscribe(IObserver<T,TParam> newObserver)
        {
            if (_observers.Add(newObserver))
                return (new Unsubscriber<T,TParam>(_observers, newObserver), true);
            return (null, false);
        }

        public (IDisposable, bool) Subscribe(IObserver<T, TParam> newObserver, IEnumerable<PublisherData<T, TParam>> predefinedIInfos)
        {
            if (_observers.Add(newObserver))
            {
                foreach (var priorInfo in predefinedIInfos)    
                {
                    newObserver.OnNext(priorInfo.Command, priorInfo.Param);
                }
                return (new Unsubscriber<T,TParam>(_observers, newObserver), true);
            }
            return (null, false);
            
        }

        public IDisposable Subscribe(IObserver<T,TParam> newObserver)
        {
            if (_observers.Add(newObserver))
                return new Unsubscriber<T,TParam>(_observers, newObserver);
            return null;
        }

        public virtual void Notify(TParam param)
        {
            
        }

        public virtual void Notify()
        {
            
        }
        public void NotifyParallel(T command, TParam param)
        {
            // Use parallel programing to boots performance
            Parallel.ForEach(_observers, (observer) =>
            {
                observer.OnNext(command, param );
            });
        }
        public void NotifySequence(T command, TParam param)
        {
            foreach (var iter in _observers.ToList())
            {
                iter.OnNext(command, param);
            }
        }
    }
    
    internal sealed class Unsubscriber<T, TParam> : IDisposable
    {
        private readonly ISet<IObserver<T,TParam>> _observers;
        private readonly IObserver<T, TParam> _observer;

        internal Unsubscriber(ISet<IObserver<T,TParam>> observers,
            IObserver<T,TParam> observer) => (_observers, _observer) = (observers, observer);

        public void Dispose() => _observers.Remove(_observer);
    }

}