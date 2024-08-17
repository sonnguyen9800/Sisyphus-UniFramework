using System;
using System.Collections.Generic;

namespace SisyphusLab.Notifier
{
    // Custom ISObservable that accep order of notifier client
    public interface ISisyphusObservable<T, in TParam> where T : IComparable<T>
    {
        protected SortedSet<IObserver<T>> Observers
        {
            get;
        }
        
        public (IDisposable ,bool) Subscribe(IObserver<T> newObserver);
        public (IDisposable ,bool) Subscribe(IObserver<T> newObserver, IEnumerable<T> predefinedIInfos);

        public void Notify(TParam param);
        public void Notify();
    }

}