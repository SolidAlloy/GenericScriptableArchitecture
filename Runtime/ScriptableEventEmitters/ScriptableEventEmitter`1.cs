namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;


    /// <inheritdoc/>
    [Serializable]
    public abstract class ScriptableEventEmitter<T> : ScriptableEventEmitterBase
    {
        [SerializeField] private ScriptableEvent<T> _event;
        [SerializeField] private T _arg0;

        protected override BaseScriptableEvent BaseEvent => _event;

        public override void InvokeEvent()
        {
            if (_event == null)
            {
                Debug.LogWarning("Marker does not have an assigned event, so it will not trigger anything.");
                return;
            }

            _event.Invoke(_arg0);
        }
    }
}