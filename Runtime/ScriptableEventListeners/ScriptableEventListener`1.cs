namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T> : BaseScriptableEventListener, IListener<T>
    {
        [SerializeField] private EventHolder<T> _event;
        [SerializeField] private UnityEvent<T> _response;

        protected virtual void OnEnable() => _event?.AddListener(this);

        protected virtual void OnDisable() => _event?.RemoveListener(this);

        public void OnEventInvoked(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace(arg0);
            _response.Invoke(arg0);
        }
    }
}