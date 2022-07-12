namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    [AddComponentMenu("")]
    public class ScriptableEventListener<T> : BaseScriptableEventListener, IListener<T>
    {
        [SerializeField, FormerlySerializedAs("_event")] private EventHolder<T> _eventHolder = new EventHolder<T>();
        [SerializeField] private ExtEvent<T> _response;

        private void OnEnable() => _eventHolder?.AddListener(this);

        private void OnDisable() => _eventHolder?.RemoveListener(this);

        public void OnEventInvoked(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace(arg0);
            _response.Invoke(arg0);
        }
    }
}