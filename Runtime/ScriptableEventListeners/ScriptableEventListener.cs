namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener : ScriptableEventListenerBase
    {
        [SerializeField] private ScriptableEvent _event;
        [SerializeField] private UnityEvent _response;

        private void OnEnable()
        {
            if (_event != null)
                _event.AddListener(this);
        }

        private void OnDisable()
        {
            if (_event != null)
                _event.RemoveListener(this);
        }

        public void OnEventRaised()
        {
            AddStackTrace();
            _response.Invoke();
        }
    }
}