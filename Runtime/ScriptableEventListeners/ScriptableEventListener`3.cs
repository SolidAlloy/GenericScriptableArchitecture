namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T1, T2, T3> : BaseScriptableEventListener
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private UnityEvent<T1, T2, T3> _response;

        protected virtual void OnEnable()
        {
            if (_event != null)
                _event.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            if (_event != null)
                _event.RemoveListener(this);
        }

        public void OnEventInvoked(T1 arg0, T2 arg1, T3 arg2)
        {
            if ( ! CanBeInvoked())
                return;

            _response.Invoke(arg0, arg1, arg2);
        }
    }
}