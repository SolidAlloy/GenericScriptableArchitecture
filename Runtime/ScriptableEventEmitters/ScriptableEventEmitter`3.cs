namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    /// <inheritdoc/>
    [Serializable]
    public abstract class ScriptableEventEmitter<T1, T2, T3> : ScriptableEventEmitterBase
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private T1 _arg0;
        [SerializeField] private T2 _arg1;
        [SerializeField] private T3 _arg2;

        protected override ScriptableEventBase EventBase => _event;

        public override void InvokeEvent()
        {
            if (_event == null)
            {
                Debug.LogWarning("Marker does not have an assigned event, so it will not trigger anything.");
                return;
            }

            _event.Invoke(_arg0, _arg1, _arg2);
        }
    }
}