﻿namespace GenericScriptableArchitecture.Timeline
{
    using System;
    using GenericUnityObjects;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

#if GSA_TIMELINE
    using Internals;
#endif

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

        [SerializeField, Tooltip("Time for the marker"),
#if GSA_TIMELINE
         TimeField
#endif
        ] private double m_Time;

        /// <inheritdoc/>
        public TrackAsset parent { get; private set; }

        /// <inheritdoc/>
        /// <remarks>
        /// The marker time cannot be negative.
        /// </remarks>
        public double time
        {
            get => m_Time;
            set => m_Time = Math.Max(value, 0);
        }

        /// <inheritdoc/>
        void IMarker.Initialize(TrackAsset parentTrack)
        {
            // We only really want to update the parent when the object is first deserialized
            // If not a cloned track would "steal" the source's markers
            if (parent == null)
            {
                parent = parentTrack;
            }
        }

        #endregion
    }
}