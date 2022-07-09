namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using EasyButtons;
    using SolidUtilities;
    using UnityEngine;
#if UNIRX
    using UniRx;
#endif

    [Serializable]
    [CreateAssetMenu(menuName = Config.PackageName + Config.Events + "ScriptableEvent")]
    public class ScriptableEvent : BaseScriptableEvent, IEvent
#if UNIRX
        , IObservable<Unit>
#endif
    {
        private EventHelper _eventHelper;

        internal override List<UnityEngine.Object> Listeners => _eventHelper?.Listeners ?? ListHelper.Empty<UnityEngine.Object>();

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void Invoke()
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace();
            _eventHelper.NotifyListeners();
        }

        private void OnEnable() => _eventHelper = new EventHelper(this);

        private void OnDisable() => _eventHelper.Dispose();

        #region Adding Removing Listeners

        public void AddListener(IListener listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(IListener listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(Action listener) => _eventHelper.AddListener(listener);

        public void RemoveListener(Action listener) => _eventHelper.RemoveListener(listener);

        #endregion

#if UNIRX
        public IDisposable Subscribe(IObserver<Unit> observer) => _eventHelper.Subscribe(observer);
#endif

        #region Operator Overloads

        public static ScriptableEvent operator +(ScriptableEvent scriptableEvent, Action listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent operator -(ScriptableEvent scriptableEvent, Action listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent operator +(ScriptableEvent scriptableEvent, IListener listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent operator -(ScriptableEvent scriptableEvent, IListener listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}