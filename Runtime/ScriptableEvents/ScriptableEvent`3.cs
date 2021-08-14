namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T1,T2,T3>")]
    public class ScriptableEvent<T1, T2, T3> : BaseScriptableEvent, IEvent<T1, T2, T3>
#if UNIRX
        , IObservable<(T1, T2, T3)>, IDisposable
#endif
    {
        private List<ScriptableEventListener<T1, T2, T3>> _listeners = new List<ScriptableEventListener<T1, T2, T3>>();
        private List<IMultipleEventsListener<T1, T2, T3>> _multipleEventsListeners = new List<IMultipleEventsListener<T1, T2, T3>>();
        private List<IEventListener<T1, T2, T3>> _singleEventListeners = new List<IEventListener<T1, T2, T3>>();
        private List<Action<T1, T2, T3>> _responses = new List<Action<T1, T2, T3>>();

        internal override List<UnityEngine.Object> Listeners
            => _responses
                .Select(response => response.Target)
                .Concat(_singleEventListeners)
                .Concat(_multipleEventsListeners)
                .Concat(_listeners)
                .OfType<UnityEngine.Object>()
                .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T1 arg0, T2 arg1, T3 arg2)
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace(arg0, arg1, arg2);

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0, arg1, arg2);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(arg0, arg1, arg2);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventRaised(this, arg0, arg1, arg2);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventRaised(arg0, arg1, arg2);
            }

#if UNIRX
            _observableHelper?.RaiseOnNext((arg0, arg1, arg2));
#endif
        }

        #region Adding Removing Listeners

        public void AddListener(ScriptableEventListener<T1, T2, T3> listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener<T1, T2, T3> listener) => _listeners.Remove(listener);

        public void AddResponse(Action<T1, T2, T3> response) => _responses.Add(response);

        public void RemoveResponse(Action<T1, T2, T3> response) => _responses.Remove(response);

        public void AddListener(IMultipleEventsListener<T1, T2, T3> listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener<T1, T2, T3> listener) => _multipleEventsListeners.Remove(listener);

        public void AddListener(IEventListener<T1, T2, T3> listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener<T1, T2, T3> listener) => _singleEventListeners.Remove(listener);

        #endregion

        #region UniRx
#if UNIRX
        private ObservableHelper<(T1, T2, T3)> _observableHelper;

        public IDisposable Subscribe(IObserver<(T1, T2, T3)> observer)
        {
            _observableHelper ??= new ObservableHelper<(T1, T2, T3)>();
            return _observableHelper.Subscribe(observer);
        }

        public void Dispose() => _observableHelper?.Dispose();
#endif
        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T1, T2, T3> operator +(ScriptableEvent<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> response)
        {
            scriptableEvent.AddResponse(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator -(ScriptableEvent<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> response)
        {
            scriptableEvent.RemoveResponse(response);
            return scriptableEvent;
        }

        #endregion
    }
}