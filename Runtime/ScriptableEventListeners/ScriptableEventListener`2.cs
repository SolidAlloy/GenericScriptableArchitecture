namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T1, T2> : BaseScriptableEventListener
    {
        [SerializeField] private EventHolder<T1, T2> _event;
        [SerializeField] private UnityEvent<T1, T2> _response;

        protected virtual void OnEnable()
        {
            _event.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            _event.RemoveListener(this);
        }

        public void OnEventInvoked(T1 arg0, T2 arg1)
        {
            if ( ! CanBeInvoked())
                return;

            _response.Invoke(arg0, arg1);
        }
    }
}