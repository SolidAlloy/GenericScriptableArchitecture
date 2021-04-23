namespace GenericScriptableArchitecture
{
    using UnityEngine;
    using UnityEngine.Playables;

#if USE_TIMELINE
    using UnityEngine.Timeline;
#endif

    /// <summary>
    /// Marker that invokes an attached event when triggered. A game object that has PlayableDirector component must
    /// also have a <see cref="ScriptableSignalRelayer"/> component for events to be triggered.
    /// </summary>
    public abstract class ScriptableEventEmitterBase :
#if USE_TIMELINE
        Marker,
#endif
        INotification
#if USE_TIMELINE
        , INotificationOptionProvider
#endif
    {
        [SerializeField] private bool _retroactive;
        [SerializeField] private bool _emitOnce;

        protected abstract ScriptableEventBase EventBase { get; }

        public abstract void InvokeEvent();

        PropertyName INotification.id => EventBase == null ? new PropertyName(string.Empty) : new PropertyName(EventBase.name);

#if USE_TIMELINE
        NotificationFlags INotificationOptionProvider.flags =>
            (_retroactive ? NotificationFlags.Retroactive : default)
            | (_emitOnce ? NotificationFlags.TriggerOnce : default);
#endif
    }
}