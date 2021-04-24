namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T> : ScriptableEventListenerBase
    {
        [SerializeField] private ScriptableEvent<T> _event;
        [SerializeField] private UnityEvent<T> _response;

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

        public void OnEventRaised(T arg0)
        {
            _response.Invoke(arg0);
        }
    }
}