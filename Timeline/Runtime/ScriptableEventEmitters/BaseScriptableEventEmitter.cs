namespace GenericScriptableArchitecture.Timeline
{
    using System;
    using GenericUnityObjects;
    using Internals;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    /// <summary>
    /// Marker that invokes an attached event when triggered. A game object that has PlayableDirector component must
    /// also have a <see cref="ScriptableSignalRelayer"/> component for events to be triggered.
    /// </summary>
    [Serializable]
    public abstract class BaseScriptableEventEmitter : GenericScriptableObject, IMarker, INotificationOptionProvider, INotification
    {
        [SerializeField] private bool _retroactive;
        [SerializeField] private bool _emitOnce;

        protected abstract BaseScriptableEvent BaseEvent { get; }

        public abstract void InvokeEvent();

        PropertyName INotification.id => BaseEvent == null ? new PropertyName(string.Empty) : new PropertyName(BaseEvent.name);

        NotificationFlags INotificationOptionProvider.flags =>
            (_retroactive ? NotificationFlags.Retroactive : default)
            | (_emitOnce ? NotificationFlags.TriggerOnce : default);

        #region Marker Code

        [SerializeField, TimeField, Tooltip("Time for the marker")] double m_Time;

        /// <inheritdoc/>
        public TrackAsset parent { get; private set; }

        /// <inheritdoc/>
        /// <remarks>
        /// The marker time cannot be negative.
        /// </remarks>
        public double time
        {
            get { return m_Time; }
            set { m_Time = Math.Max(value, 0); }
        }

        /// <inheritdoc/>
        void IMarker.Initialize(TrackAsset parentTrack)
        {
            // We only really want to update the parent when the object is first deserialized
            // If not a cloned track would "steal" the source's markers
            if (parent == null)
            {
                parent = parentTrack;
                try
                {
                    OnInitialize(parentTrack);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message, this);
                }
            }
        }

        /// <summary>
        /// Override this method to receive a callback when the marker is initialized.
        /// </summary>
        /// <param name="aPent">The track that contains the marker.</param>
        public virtual void OnInitialize(TrackAsset aPent)
        {
        }

        #endregion
    }
}