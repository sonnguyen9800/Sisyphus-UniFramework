using System;
using System.Collections.Generic;

namespace SisyphusFramework.Notifier
{
    // Custom ISObservable that accep order of notifier client
    public interface ISortedObservable<T, TParam> where T : IComparable<T>
    {
        public struct PublisherData<T, TParam>
        {
            public T Command;
            public TParam Param;
        }

        protected SortedSet<IObserver<T,TParam>> Observers
        {
            get;
        }
        
        public (IDisposable ,bool) Subscribe(IObserver<T, TParam> newObserver);
        public (IDisposable ,bool) Subscribe(IObserver<T, TParam> newObserver, IEnumerable<PublisherData<T, TParam>> predefinedIInfos);

        public void Notify(TParam param);
        public void Notify();
    }

}