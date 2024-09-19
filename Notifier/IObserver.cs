using System;

namespace SisyphusLab.Notifier
{
    public interface IObserver<in T, TParam>
    {
        void OnCompleted();

        void OnError(Exception error);

        void OnNext(T value, TParam param);
    }
}