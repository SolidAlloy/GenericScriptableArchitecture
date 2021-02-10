namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GameEvent<T1, T2, T3> : GameEventBase
    {
        [SerializeField] internal List<ScriptableEvent<T1, T2, T3>> ScriptableEvents = new List<ScriptableEvent<T1, T2, T3>>();

        public void Invoke(T1 arg0, T2 arg1, T3 arg2)
        {
            foreach (var scriptableEvent in ScriptableEvents)
            {
                scriptableEvent.Invoke(arg0, arg1, arg2);
            }
        }
    }
}