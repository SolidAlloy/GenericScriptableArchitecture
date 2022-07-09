namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using SolidUtilities;

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

        public void Invoke(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace(arg0);
            _eventHelper.NotifyListeners(arg0);
        }

        #region Adding Removing Listeners

        public void AddListener(Action<T> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(Action<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IListener<T> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IListener<T> listener) => _eventHelper.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer) => _eventHelper.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}