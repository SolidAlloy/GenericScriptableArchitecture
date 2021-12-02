namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener : BaseScriptableEventListener, IListener
    {
        [SerializeField] internal ScriptableEvent _event;
        [SerializeField] internal UnityEvent _response;

        public override BaseEvent Event
        {
            get => _event;
            set
            {
                if (value is ScriptableEvent scriptableEvent)
                    _event = scriptableEvent;
            }
        }

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