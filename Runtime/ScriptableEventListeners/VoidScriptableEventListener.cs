namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [AddComponentMenu("")]
    public class VoidScriptableEventListener : BaseScriptableEventListener, IListener
    {
        [SerializeField] internal ScriptableEvent _event;
        [SerializeField] internal ExtEvent _response;

        internal override IBaseEvent Event
        {
            get => _event;
            set
            {
                if (value is ScriptableEvent scriptableEvent)
                    _event = scriptableEvent;
            }
        }

        internal override bool DrawObjectField { get; set; }

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

            _stackTrace.AddStackTrace();
            _response.Invoke();
        }
    }
}