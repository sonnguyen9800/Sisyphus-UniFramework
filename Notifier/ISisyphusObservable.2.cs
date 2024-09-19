using System;
using System.Collections.Generic;

namespace SisyphusLab.Notifier
{
    /// <summary>
    /// Custom ISObservable that accept param
    /// </summary>
    /// <typeparam name="T">Enum, Type for command</typeparam>
    /// <typeparam name="TParam">Extra param (struct</typeparam>
    // 
    public interface IObservable<T, TParam>
        where T: System.Enum
        where TParam : struct
    {
        public struct PublisherData<T, TParam>
        {
            public T Command;
            public TParam Param;
        }

        protected ISet<IObserver<T,TParam>> Observers
        {
            get;
        }
        
        public (IDisposable ,bool) TrySubscribe(IObserver<T, TParam> newObserver);
        public (IDisposable ,bool) Subscribe(IObserver<T, TParam> newObserver, IEnumerable<PublisherData<T, TParam>> predefinedIInfos);

        public void Notify(TParam param);
        public void Notify();
    }

}