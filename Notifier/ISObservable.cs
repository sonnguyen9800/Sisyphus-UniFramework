using System;

namespace SisyphusLab.Notifier
{
    // Custom ISObservable that accep order of notifier client
    public interface ISObservable<out T>
    {
        IDisposable Subscribe(IObserver<T> observer, byte rank);
    }
}