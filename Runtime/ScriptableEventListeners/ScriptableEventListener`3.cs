namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class ScriptableEventListener<T1, T2, T3> : BaseScriptableEventListener, IListener<T1, T2, T3>
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private UnityEvent<T1, T2, T3> _response;

        public override BaseEvent Event
        {
            get => _event;
            set
            {
                if (value is ScriptableEvent<T1, T2, T3> scriptableEvent)
                    _event = scriptableEvent;
            }
        }

        public override bool DrawObjectField { get; set; }

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