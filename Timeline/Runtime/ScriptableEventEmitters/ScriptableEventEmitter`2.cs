﻿namespace GenericScriptableArchitecture.Timeline
{
    using System;
    using GenericUnityObjects;
    using UnityEngine;
    using UnityEngine.Timeline;

    /// <inheritdoc/>
    [Serializable, ApplyToChildren(typeof(HideInMenuAttribute))]
    public class ScriptableEventEmitter<T1, T2> : BaseScriptableEventEmitter
    {
        [SerializeField] private ScriptableEvent<T1, T2> _event;
        [SerializeField] private T1 _arg0;
        [SerializeField] private T2 _arg1;

        protected override BaseScriptableEvent BaseEvent => _event;

        public override void InvokeEvent()
        {
            if (_event == null)
            {
                Debug.LogWarning("Marker does not have an assigned event, so it will not trigger anything.");
                return;
            }

            _event.Invoke(_arg0, _arg1);
        }
    }
}