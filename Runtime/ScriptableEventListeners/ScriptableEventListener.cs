namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener : BaseScriptableEventListener
    {
        [SerializeField] internal ScriptableEvent _event;
        [SerializeField] internal UnityEvent _response;

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

        public void OnEventInvoked()
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace();
            _response.Invoke();
        }
    }
}