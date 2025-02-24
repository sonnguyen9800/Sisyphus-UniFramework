using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;

// Original reference: https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern


namespace SisyphusFramework.Notifier
{
    /// <summary>
    /// This Publisher had Observable to be sorted
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public class SortedPublisher<T, TParam> : ISortedObservable<T, TParam>
        where T : IComparable<T>
    {

        private SortedSet<IObserver<T, TParam>> _observers = new();

        SortedSet<IObserver<T,TParam>> ISortedObservable<T, TParam>.Observers
        {
            get => _observers;
        }

        public SortedPublisher()
        {
            _observers = new SortedSet<IObserver<T,TParam>>();
        }

        public (IDisposable, bool) Subscribe(IObserver<T,TParam> newObserver)
        {
            if (_observers.Add(newObserver))
                return (new Unsubscriber<T,TParam>(_observers, newObserver), true);
            return (null, false);
        }
        

        public (IDisposable, bool) Subscribe(
            IObserver<T,TParam> newObserver, IEnumerable<
                ISortedObservable<T,TParam>.PublisherData<T,TParam>> predefinedIInfos)
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
    


}