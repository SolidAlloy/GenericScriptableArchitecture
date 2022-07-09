namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [AddComponentMenu("")]
    public class ScriptableEventListener<T> : BaseScriptableEventListener, IListener<T>
    {
        [SerializeField] private EventHolder<T> _event = new EventHolder<T>();
        [SerializeField] private ExtEvent<T> _response;

        internal override IBaseEvent Event
        {
            get => _event.Event;
            set => _event.Event = value;
        }

        internal override bool DrawObjectField { get => _event.DrawObjectField; set => _event.DrawObjectField = value; }

        protected virtual void OnEnable() => _event?.AddListener(this);

        protected virtual void OnDisable() => _event?.RemoveListener(this);

        public void OnEventInvoked(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace(arg0);
            _response.Invoke(arg0);
        }
    }
}