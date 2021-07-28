#if UNIRX
namespace GenericScriptableArchitecture
{
    internal interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }
}
#endif