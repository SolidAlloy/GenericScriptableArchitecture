namespace ScriptableEvents
{
    using UnityEngine;
    using UnityEngine.Events;

    public class ScriptableEventListener<T1, T2> : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent<T1, T2> _event;
        [SerializeField] private UnityEvent<T1, T2> _response;

        protected virtual void OnEnable()
        {
            _event.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            _event.UnregisterListener(this);
        }

        public void OnEventRaised(T1 arg0, T2 arg1)
        {
            _response.Invoke(arg0, arg1);
        }
    }
}