namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using EasyButtons;
    using GenericUnityObjects;
    using SolidUtilities.Helpers;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T>")]
    public class ScriptableEvent<T> : BaseScriptableEvent, IEvent<T>
#if UNIRX
        , IObservable<T>
#endif
    {
        private EventHelper<T> _eventHelper;

        internal override List<UnityEngine.Object> Listeners => _eventHelper?.Listeners ?? ListHelper.Empty<UnityEngine.Object>();

        private void OnEnable() => _eventHelper = new EventHelper<T>(this);

        private void OnDisable() => _eventHelper.Dispose();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace(arg0);
            _eventHelper.NotifyListeners(arg0);
        }

        #region Adding Removing Listeners

        public void AddListener(ScriptableEventListener<T> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(ScriptableEventListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IMultipleEventsListener<T> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IMultipleEventsListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IEventListener<T> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IEventListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddResponse(Action<T> response) => _eventHelper.AddResponse(response);

        public void RemoveResponse(Action<T> response) => _eventHelper.RemoveResponse(response);

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer) => _eventHelper.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, Action<T> response)
        {
            scriptableEvent.AddResponse(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, Action<T> response)
        {
            scriptableEvent.RemoveResponse(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, ScriptableEventListener<T> response)
        {
            scriptableEvent.AddListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, ScriptableEventListener<T> response)
        {
            scriptableEvent.RemoveListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, IEventListener<T> response)
        {
            scriptableEvent.AddListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, IEventListener<T> response)
        {
            scriptableEvent.RemoveListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, IMultipleEventsListener<T> response)
        {
            scriptableEvent.AddListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, IMultipleEventsListener<T> response)
        {
            scriptableEvent.RemoveListener(response);
            return scriptableEvent;
        }

        #endregion
    }
}