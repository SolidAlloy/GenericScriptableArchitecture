namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameEvent<T> : GameEventBase
    {
        [SerializeField] internal List<ScriptableEvent<T>> ScriptableEvents = new List<ScriptableEvent<T>>();

        public void Invoke(T arg0)
        {
            foreach (var scriptableEvent in ScriptableEvents)
            {
                scriptableEvent.Invoke(arg0);
            }
        }
    }
}