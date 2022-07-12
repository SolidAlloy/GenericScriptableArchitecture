namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

    public class EventInstancer<T> : BaseEventInstancer, IScriptableEvent<T>
    {
        [SerializeField] internal ScriptableEvent<T> _base;

        // Setter would be of no use here since we only use base event reference for description and arg names, and it doesn't influence runtime execution.
        [PublicAPI] public ScriptableEvent<T> EventReference => _base;

        [SerializeField] private ScriptableEventHelper<T> _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        internal override BaseScriptableEvent Base => _base;

        private void Awake() => _scriptableEventHelper.Initialize(this, name, "event instancer");

        private void OnDisable() => _scriptableEventHelper.Event.Dispose();

        public void Invoke(T arg0) => _scriptableEventHelper.Invoke(arg0);

        #region Adding Removing Listeners

        public void AddListener(Action<T> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(Action<T> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        public void AddListener(IListener<T> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(IListener<T> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer) => _scriptableEventHelper.Event.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static EventInstancer<T> operator +(EventInstancer<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T> operator -(EventInstancer<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T> operator +(EventInstancer<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T> operator -(EventInstancer<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}