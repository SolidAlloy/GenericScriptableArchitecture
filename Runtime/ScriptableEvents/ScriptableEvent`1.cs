namespace ScriptableEvents
{
    using System;
    using System.Collections.Generic;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = "Events/ScriptableEvent<T>")]
    public class ScriptableEvent<T> : ScriptableEventBase
    {
        private List<ScriptableEventListener<T>> _listeners = new List<ScriptableEventListener<T>>();

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void Invoke(T arg0)
        {
            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0);
            }
        }

        public void RegisterListener(ScriptableEventListener<T> listener) => _listeners.Add(listener);

        public void UnregisterListener(ScriptableEventListener<T> listener) => _listeners.Remove(listener);
    }
}