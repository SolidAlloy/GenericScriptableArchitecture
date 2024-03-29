﻿namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    [AddComponentMenu("")]
    public class ScriptableEventListener<T1, T2> : BaseScriptableEventListener, IListener<T1, T2>
    {
        [SerializeField, FormerlySerializedAs("_event")] private EventHolder<T1, T2> _eventHolder = new EventHolder<T1, T2>();
        [SerializeField] private ExtEvent<T1, T2> _response;

        private void OnEnable() => _eventHolder?.AddListener(this);

        private void OnDisable() => _eventHolder?.RemoveListener(this);

        public void OnEventInvoked(T1 arg0, T2 arg1)
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace(arg0, arg1);
            _response.Invoke(arg0, arg1);
        }
    }
}