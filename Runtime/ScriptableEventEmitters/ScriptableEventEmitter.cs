namespace GenericScriptableArchitecture
{
    using UnityEngine;

    /// <inheritdoc/>
    [ExcludeFromPreset]
    public class ScriptableEventEmitter : ScriptableEventEmitterBase
    {
        [SerializeField] private ScriptableEvent _event;

        protected override BaseScriptableEvent BaseEvent => _event;

        public override void InvokeEvent()
        {
            if (_event == null)
            {
                Debug.LogWarning("Marker does not have an assigned event, so it will not trigger anything.");
                return;
            }

            _event.Invoke();
        }
    }
}