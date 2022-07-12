namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [AddComponentMenu("")]
    public class ScriptableEventListener<T1, T2, T3> : BaseScriptableEventListener, IListener<T1, T2, T3>
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private EventHolder<T1, T2, T3> _eventHolder;
        [SerializeField] private ExtEvent<T1, T2, T3> _response;

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

        public void OnEventInvoked(T1 arg0, T2 arg1, T3 arg2)
        {
            if ( ! CanBeInvoked())
                return;

            _stackTrace.AddStackTrace(arg0, arg1, arg2);
            _response.Invoke(arg0, arg1, arg2);
        }
    }
}