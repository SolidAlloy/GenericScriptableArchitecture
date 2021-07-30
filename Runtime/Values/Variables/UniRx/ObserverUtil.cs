#if UNIRX
namespace GenericScriptableArchitecture
{
    using System;
    using System.Threading;

    internal static class ObserverUtil
    {
        internal static IObserver<(T Previous, T Current)> CreateSubscribeObserver<T>(Action<T, T> onNext, Action<Exception> onError, Action onCompleted)
        {
            // need compare for avoid iOS AOT
            if (onNext == Stubs<T, T>.Ignore)
            {
                return new Subscribe<(T, T)>(onError, onCompleted);
            }

            return new SubscribeWithHistory<T>(onNext, onError, onCompleted);
        }

        private class SubscribeWithHistory<T> : IObserver<(T, T)>
        {
            private readonly Action<T, T> _onNext;
            private readonly Action<Exception> _onError;
            private readonly Action _onCompleted;

            private int _isStopped;

            public SubscribeWithHistory(Action<T, T> onNext, Action<Exception> onError, Action onCompleted)
            {
                _onNext = onNext;
                _onError = onError;
                _onCompleted = onCompleted;
            }

            public void OnNext((T, T) value)
            {
                if (_isStopped == 0)
                {
                    _onNext(value.Item1, value.Item2);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onError(error);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onCompleted();
                }
            }
        }

        private class Subscribe<T> : IObserver<T>
        {
            private readonly Action<Exception> _onError;
            private readonly Action _onCompleted;

            private int _isStopped;

            public Subscribe(Action<Exception> onError, Action onCompleted)
            {
                _onError = onError;
                _onCompleted = onCompleted;
            }

            public void OnNext(T value) { }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onError(error);
                }
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onCompleted();
                }
            }
        }
    }
}
#endif