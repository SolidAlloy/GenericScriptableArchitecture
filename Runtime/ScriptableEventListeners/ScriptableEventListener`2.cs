namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T1, T2> : BaseScriptableEventListener, IListener<T1, T2>
    {
        [SerializeField] private EventHolder<T1, T2> _event = new EventHolder<T1, T2>();
        [SerializeField] private UnityEvent<T1, T2> _response;

        public override BaseEvent Event
        {
            get => _event.Event;
            set => _event.Event = value;
        }

        protected virtual void OnEnable()
        {
            _event?.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            _event?.RemoveListener(this);
        }

        public void OnEventInvoked(T1 arg0, T2 arg1)
        {
            if ( ! CanBeInvoked())
                return;

            _response.Invoke(arg0, arg1);
        }
    }
}