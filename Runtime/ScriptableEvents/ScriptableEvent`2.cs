namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using SolidUtilities;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T1,T2>")]
    public class ScriptableEvent<T1, T2> : BaseScriptableEvent, IEvent<T1, T2>
#if UNIRX
        , IObservable<(T1, T2)>
#endif
    {
        private EventHelper<T1, T2> _eventHelper;

        internal override List<UnityEngine.Object> Listeners => _eventHelper?.Listeners ?? ListHelper.Empty<UnityEngine.Object>();

        private void OnEnable() => _eventHelper = new EventHelper<T1, T2>(this);

        private void OnDisable() => _eventHelper.Dispose();

        public void Invoke(T1 arg0, T2 arg1)
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace(arg0, arg1);

            _eventHelper.NotifyListeners(arg0, arg1);
        }

        #region Adding Removing Listeners

        public void AddListener(IListener<T1, T2> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IListener<T1, T2> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(Action<T1, T2> listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(Action<T1, T2> listener) => _eventHelper.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<(T1, T2)> observer) => _eventHelper.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T1, T2> operator +(ScriptableEvent<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2> operator -(ScriptableEvent<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2> operator +(ScriptableEvent<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2> operator -(ScriptableEvent<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}