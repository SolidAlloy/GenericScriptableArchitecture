namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameEvent : GameEventBase
    {
        [SerializeField] internal List<ScriptableEvent> ScriptableEvents = new List<ScriptableEvent>();

        public void Invoke()
        {
            foreach (var scriptableEvent in ScriptableEvents)
            {
                scriptableEvent.Invoke();
            }
        }
    }
}