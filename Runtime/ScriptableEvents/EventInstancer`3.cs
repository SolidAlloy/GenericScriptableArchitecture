namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

    public class EventInstancer<T1, T2, T3> : BaseEventInstancer, IScriptableEvent<T1, T2, T3>
    {
        [SerializeField] internal ScriptableEvent<T1, T2, T3> _base;

        // Setter would be of no use here since we only use base event reference for description and arg names, and it doesn't influence runtime execution.
        [PublicAPI] public ScriptableEvent<T1, T2, T3> EventReference => _base;

        [SerializeField] private ScriptableEventHelper<T1, T2, T3> _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        private void Awake() => _scriptableEventHelper.Initialize(this, name, "event instancer");

        private void OnDisable() => _scriptableEventHelper.Event.Dispose();

        public void Invoke(T1 arg0, T2 arg1, T3 arg2) => _scriptableEventHelper.Invoke(arg0, arg1, arg2);
        
        #region Adding Removing Listeners

        public void AddListener(IListener<T1, T2, T3> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(IListener<T1, T2, T3> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        public void AddListener(Action<T1, T2, T3> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(Action<T1, T2, T3> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<(T1, T2, T3)> observer) => _scriptableEventHelper.Event.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static EventInstancer<T1, T2, T3> operator +(EventInstancer<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T1, T2, T3> operator -(EventInstancer<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T1, T2, T3> operator +(EventInstancer<T1, T2, T3> scriptableEvent, IListener<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventInstancer<T1, T2, T3> operator -(EventInstancer<T1, T2, T3> scriptableEvent, IListener<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}