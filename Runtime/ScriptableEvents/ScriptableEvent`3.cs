namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using GenericUnityObjects;
    using SolidUtilities.Helpers;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T1,T2,T3>")]
    public class ScriptableEvent<T1, T2, T3> : BaseScriptableEvent, IEvent<T1, T2, T3>
#if UNIRX
        , IObservable<(T1, T2, T3)>
#endif
    {
        private EventHelper<T1, T2, T3> _eventHelper;

        internal override List<UnityEngine.Object> Listeners => _eventHelper?.Listeners ?? ListHelper.Empty<UnityEngine.Object>();

        private void OnEnable() => _eventHelper = new EventHelper<T1, T2, T3>(this);

        private void OnDisable() => _eventHelper.Dispose();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T1 arg0, T2 arg1, T3 arg2)
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace(arg0, arg1, arg2);
            _eventHelper.NotifyListeners(arg0, arg1, arg2);
        }

        #region Adding Removing Listeners

        public void AddListener(ScriptableEventListener<T1, T2, T3> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(ScriptableEventListener<T1, T2, T3> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IEventListener<T1, T2, T3> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IEventListener<T1, T2, T3> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IMultipleEventsListener<T1, T2, T3> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IMultipleEventsListener<T1, T2, T3> listener) => _eventHelper.RemoveListener(listener);

        public void AddResponse(Action<T1, T2, T3> response) => _eventHelper.AddResponse(response);

        public void RemoveResponse(Action<T1, T2, T3> response) => _eventHelper.RemoveResponse(response);

        public IDisposable Subscribe(IObserver<(T1, T2, T3)> observer) => _eventHelper.Subscribe(observer);

        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T1, T2, T3> operator +(ScriptableEvent<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddResponse(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator -(ScriptableEvent<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveResponse(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator +(ScriptableEvent<T1, T2, T3> scriptableEvent, ScriptableEventListener<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator -(ScriptableEvent<T1, T2, T3> scriptableEvent, ScriptableEventListener<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator +(ScriptableEvent<T1, T2, T3> scriptableEvent, IEventListener<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator -(ScriptableEvent<T1, T2, T3> scriptableEvent, IEventListener<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator +(ScriptableEvent<T1, T2, T3> scriptableEvent, IMultipleEventsListener<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(response);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2, T3> operator -(ScriptableEvent<T1, T2, T3> scriptableEvent, IMultipleEventsListener<T1, T2, T3> response)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(response);
            return scriptableEvent;
        }

        #endregion
    }
}