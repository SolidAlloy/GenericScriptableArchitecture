namespace GenericScriptableArchitecture
{
    using System;
    using ExtEvents;
    using UnityEngine;

    [Serializable]
    [AddComponentMenu("")]
    public class VoidScriptableEventListener : BaseScriptableEventListener, IListener
    {
        [SerializeField] internal EventHolder _event;
        [SerializeField] internal ExtEvent _response;

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