namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [AddComponentMenu("")]
    public class ScriptableEventListener<T1, T2> : BaseScriptableEventListener, IListener<T1, T2>
    {
        [SerializeField] private EventHolder<T1, T2> _event = new EventHolder<T1, T2>();
        [SerializeField] private ExtEvent<T1, T2> _response;

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

            _stackTrace.AddStackTrace(arg0, arg1);
            _response.Invoke(arg0, arg1);
        }
    }
}