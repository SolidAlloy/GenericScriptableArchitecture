namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

    public class EventInstancer<T1, T2> : BaseEventInstancer, IScriptableEvent<T1, T2>
    {
        [SerializeField] internal ScriptableEvent<T1, T2> _base;

        // Setter would be of no use here since we only use base event reference for description and arg names, and it doesn't influence runtime execution.
        [PublicAPI] public ScriptableEvent<T1, T2> EventReference => _base;

        [SerializeField] private ScriptableEventHelper<T1, T2> _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        internal override BaseScriptableEvent Base => _base;

        private void Awake() => _scriptableEventHelper.Initialize(this, name, "event instancer");

        private void OnDisable() => _scriptableEventHelper.Event.Dispose();

        public void Invoke(T1 arg0, T2 arg1) => _scriptableEventHelper.Invoke(arg0, arg1);

        #region Adding Removing Listeners

        public void AddListener(IListener<T1, T2> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(IListener<T1, T2> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        public void AddListener(Action<T1, T2> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(Action<T1, T2> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<(T1, T2)> observer) => _scriptableEventHelper.Event.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static EventInstancer<T1, T2> operator +(EventInstancer<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T1, T2> operator -(EventInstancer<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T1, T2> operator +(EventInstancer<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T1, T2> operator -(EventInstancer<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}