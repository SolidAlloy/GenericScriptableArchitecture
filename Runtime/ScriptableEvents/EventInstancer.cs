namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

#if UNIRX
    using UniRx;
#endif

    public class EventInstancer : BaseEventInstancer, IScriptableEvent
    {
        [SerializeField] internal ScriptableEvent _base;

        // Setter would be of no use here since we only use base event reference for description and arg names, and it doesn't influence runtime execution.
        [PublicAPI] public ScriptableEvent EventReference => _base;

        [SerializeField] private ScriptableEventHelperVoid _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        private void Awake() => _scriptableEventHelper.Initialize(this, name, "event instancer");

        private void OnDisable() => _scriptableEventHelper.Event.Dispose();

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

        public static EventInstancer operator +(EventInstancer scriptableEvent, Action listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer operator -(EventInstancer scriptableEvent, Action listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer operator +(EventInstancer scriptableEvent, IListener listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer operator -(EventInstancer scriptableEvent, IListener listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}