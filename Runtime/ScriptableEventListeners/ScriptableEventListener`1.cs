namespace GenericScriptableArchitecture
{
    using UnityEngine;
    using UnityEngine.Events;

    public class ScriptableEventListener<T> : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent<T> _event;
        [SerializeField] private UnityEvent<T> _response;

        protected virtual void OnEnable()
        {
            if (_event != null)
                _event.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            if (_event != null)
                _event.UnregisterListener(this);
        }

        public void OnEventRaised(T arg0)
        {
            _response.Invoke(arg0);
        }
    }
}