namespace GenericScriptableArchitecture.Timeline
{
    using UnityEngine;
    using UnityEngine.Playables;

    /// <summary>
    /// A component that listens for signals from Timeline and invokes appropriate ScriptableEvents.
    /// </summary>
    /// <seealso cref="ScriptableEventEmitter"/>
    public class ScriptableSignalRelayer : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable _, INotification notification, object __)
        {
            if (notification is BaseScriptableEventEmitter eventEmitter)
                eventEmitter.InvokeEvent();
        }
    }
}