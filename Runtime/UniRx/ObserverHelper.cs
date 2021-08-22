#if UNIRX
namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UniRx;

    public static class ObserverHelper
    {
        private static readonly Type _subscribeType;

        private static Dictionary<Type, FieldInfo> _onNextFields = new Dictionary<Type, FieldInfo>();

        static ObserverHelper()
        {
            _subscribeType = typeof(Observer).GetNestedType("Subscribe`1", BindingFlags.NonPublic);
        }

        public static object GetTarget<T>(IObserver<T> observer)
        {
            if (observer is ObserverUtil.SubscribeWithHistory subscribeWithHistory)
                return subscribeWithHistory.Target;

            var observerType = observer.GetType();

            if (observerType.GetGenericTypeDefinition() != _subscribeType)
                return null;

            if (!_onNextFields.TryGetValue(observerType, out var onNextField))
            {
                onNextField = observerType.GetField("onNext", BindingFlags.NonPublic | BindingFlags.Instance);
                _onNextFields.Add(observerType, onNextField);
            }

            var action = (Action<T>) onNextField.GetValue(observer);
            return action?.Target;
        }
    }
}
#endif