#if UNIRX
namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;

    public class ObservableHelper<T> : IDisposable, IObserverLinkedList<T>
    {
        private bool _disposed;
        private ObserverNode<T> _root;
        private ObserverNode<T> _last;

        private readonly List<ObserverNode<T>> _observers = new List<ObserverNode<T>>();

        public IEnumerable<object> Targets => _observers.Select(node => node.GetTarget());

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_disposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);

            _observers.Add(next);

            if (_root == null)
            {
                _root = _last = next;
            }
            else
            {
                _last.Next = next;
                next.Previous = _last;
                _last = next;
            }

            return next;
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            _observers.Remove(node);

            if (node == _root)
                _root = node.Next;

            if (node == _last)
                _last = node.Previous;

            if (node.Previous != null)
                node.Previous.Next = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            var node = _root;
            _root = _last = null;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        public void RaiseOnNext(T value)
        {
            if (_disposed)
                return;

            var node = _root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }
    }
}
#endif