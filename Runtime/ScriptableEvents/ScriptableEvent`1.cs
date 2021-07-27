namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T>")]
    public class ScriptableEvent<T> : BaseScriptableEvent, IEvent<T>
    {
        private List<ScriptableEventListener<T>> _listeners = new List<ScriptableEventListener<T>>();
        private List<IMultipleEventsListener<T>> _multipleEventsListeners = new List<IMultipleEventsListener<T>>();
        private List<IEventListener<T>> _singleEventListeners = new List<IEventListener<T>>();
        private List<Action<T>> _responses = new List<Action<T>>();

        internal override List<UnityEngine.Object> Listeners
            => _responses
                .Select(response => response.Target)
                .Concat(_singleEventListeners)
                .Concat(_multipleEventsListeners)
                .Concat(_listeners)
                .OfType<UnityEngine.Object>()
                .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace(arg0);

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(arg0);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventRaised(this, arg0);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventRaised(arg0);
            }
        }

        public void AddListener(ScriptableEventListener<T> listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener<T> listener) => _listeners.Remove(listener);

        public void AddListener(IMultipleEventsListener<T> listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener<T> listener) => _multipleEventsListeners.Remove(listener);

        public void AddListener(IEventListener<T> listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener<T> listener) => _singleEventListeners.Remove(listener);

        public void AddResponse(Action<T> response) => _responses.Add(response);

        public void RemoveResponse(Action<T> response) => _responses.Remove(response);
    }
}