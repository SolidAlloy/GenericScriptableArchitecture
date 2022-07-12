namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;

    [Serializable]
    [AddComponentMenu("")]
    public class VoidScriptableEventListener : BaseScriptableEventListener, IListener
    {
        [SerializeField] internal ScriptableEvent _event;
        [SerializeField] internal EventHolder _eventHolder;
        [SerializeField] internal ExtEvent _response;

        private void OnEnable()
        {
            if (_event != null) // backwards compatibility
            {
                _event.AddListener(this);
                return;
            }

            _eventHolder?.AddListener(this);
        }

        private void OnDisable()
        {
            if (_event != null) // backwards compatibility
            {
                _event.RemoveListener(this);
                return;
            }

            _eventHolder?.RemoveListener(this);
        }

        public void OnEventInvoked()
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace();
            _response.Invoke();
        }
    }
}