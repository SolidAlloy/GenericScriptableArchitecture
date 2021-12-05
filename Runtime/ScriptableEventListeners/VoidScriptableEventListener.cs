﻿namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    [AddComponentMenu("")]
    public class VoidScriptableEventListener : BaseScriptableEventListener, IListener
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

        public override bool DrawObjectField { get; set; }

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