#if UNIRX
namespace GenericScriptableArchitecture
{
    using System;
    using System.Threading;
    using UniRx;

    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        private readonly IObserver<T> _observer;
        private object _observerTarget;
        private bool _checkedTargetOnce;

        private IObserverLinkedList<T> _list;

        public ObserverNode<T> Previous { get; internal set; }

        public ObserverNode<T> Next { get; internal set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            _list = list;
            _observer = observer;
        }

        public object GetTarget()
        {
            if (_checkedTargetOnce)
                return _observerTarget;

            _checkedTargetOnce = true;
            _observerTarget = ObserverHelper.GetTarget(_observer);
            return _observerTarget;
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref _list, null);
            if (sourceList == null)
            {
                return;
            }

            sourceList.UnsubscribeNode(this);
        }
    }
}
#endif