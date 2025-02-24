using System;

namespace SisyphusFramework.Notifier
{
    /// <summary>
    /// IObserver with Param support. Command is supplied
    /// as an enum
    /// </summary>
    /// <typeparam name="T">Command sent</typeparam>
    /// <typeparam name="TParam">Param</typeparam>
    public interface IObserver<in T, TParam>
    {
        void OnCompleted();

        void OnError(Exception error);

        void OnNext(T value, TParam param);
        
        void Unsubscribe();
    }
}