namespace GenericScriptableArchitecture.Timeline
{
    using UnityEngine;
    using UnityEngine.Timeline;

    /// <inheritdoc/>
    [ExcludeFromPreset, HideInMenu]
    public class ScriptableEventEmitter : BaseScriptableEventEmitter
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