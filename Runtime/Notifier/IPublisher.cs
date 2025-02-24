using System;

namespace SisyphusFramework.Notifier
{
    public interface IPublisher<T> where T : System.Enum
    {
        public IDisposable Subscribe(IObserver<T> observer);
        public void Notify(T t);
    }
    /// <summary>
    /// Publisher Interface with Param support
    /// </summary>
    /// <typeparam name="T">Enum T</typeparam>
    /// <typeparam name="TParam">Param (struct)</typeparam>
    public interface IPublisher<T, TParam> 
        where T : System.Enum
        where TParam : struct
    {
        public IDisposable Subscribe(IObserver<T, TParam> observer);
        public void Notify(T t, TParam tParam);
        
    }
}