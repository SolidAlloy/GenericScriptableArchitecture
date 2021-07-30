#if UNIRX
namespace GenericScriptableArchitecture
{
    using System;

    public interface IReactivePropertyWithHistory<T> : IReadonlyReactivePropertyWithHistory<T>
    {
        new T Value { get; set; }
    }

    public interface IReadonlyReactivePropertyWithHistory<T> : IObservableWithHistory<T>
    {
        T Value { get; }

        T PreviousValue { get; }

        bool HasValue { get; }
    }

    public interface IObservableWithHistory<T> : IObservable<(T Previous, T Current)>
    {
        IDisposable Subscribe(Action<T, T> onNext);

        IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError);

        IDisposable Subscribe(Action<T, T> onNext, Action onCompleted);

        IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError, Action onCompleted);
    }
}
#endif