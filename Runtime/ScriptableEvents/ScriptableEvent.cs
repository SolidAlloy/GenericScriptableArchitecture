namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using EasyButtons;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(menuName = "Events/ScriptableEvent")]
    public class ScriptableEvent : ScriptableEventBase
    {
        private List<ScriptableEventListener> _listeners = new List<ScriptableEventListener>();

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void Invoke()
        {
            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(ScriptableEventListener listener) => _listeners.Add(listener);

        public void UnregisterListener(ScriptableEventListener listener) => _listeners.Remove(listener);
    }
}