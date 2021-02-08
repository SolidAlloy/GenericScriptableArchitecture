namespace ScriptableEvents
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameEvent<T1, T2> : GameEventBase
    {
        [SerializeField] internal List<ScriptableEvent<T1, T2>> ScriptableEvents = new List<ScriptableEvent<T1, T2>>();

        public void Invoke(T1 arg0, T2 arg1)
        {
            foreach (var scriptableEvent in ScriptableEvents)
            {
                scriptableEvent.Invoke(arg0, arg1);
            }
        }
    }
}