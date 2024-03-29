﻿namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using SolidUtilities;
    using UnityEngine;
#if UNIRX
    using UniRx;
#endif

    [Serializable]
    [CreateAssetMenu(menuName = Config.PackageName + Config.Events + "ScriptableEvent")]
    public class ScriptableEvent : BaseScriptableEvent, IScriptableEvent
    {
        [SerializeField] internal ScriptableEventHelperVoid _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        private void OnEnable() => _scriptableEventHelper?.Initialize(this, name, "event");

        private void OnDisable() => _scriptableEventHelper?.Event.Dispose();

        public void Invoke() => _scriptableEventHelper.Invoke();

        #region Adding Removing Listeners

        public void AddListener(IListener listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(IListener listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        public void AddListener(Action listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(Action listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        #endregion

#if UNIRX
        public IDisposable Subscribe(IObserver<Unit> observer) => _scriptableEventHelper.Event.Subscribe(observer);
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