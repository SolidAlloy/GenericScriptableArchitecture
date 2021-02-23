namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T1, T2, T3> : ScriptableEventListenerBase
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private UnityEvent<T1, T2, T3> _response;

        protected virtual void OnEnable()
        {
            _event.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            _event.UnregisterListener(this);
        }

        public void OnEventRaised(T1 arg0, T2 arg1, T3 arg2)
        {
            _response.Invoke(arg0, arg1, arg2);
        }
    }
}