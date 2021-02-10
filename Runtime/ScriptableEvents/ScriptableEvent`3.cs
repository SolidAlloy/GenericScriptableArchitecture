namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = "Events/ScriptableEvent<T1,T2,T3>")]
    public class ScriptableEvent<T1, T2, T3> : ScriptableEventBase
    {
        private List<ScriptableEventListener<T1, T2, T3>> _listeners = new List<ScriptableEventListener<T1, T2, T3>>();

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void Invoke(T1 arg0, T2 arg1, T3 arg2)
        {
            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0, arg1, arg2);
            }
        }

        public void RegisterListener(ScriptableEventListener<T1, T2, T3> listener) => _listeners.Add(listener);

        public void UnregisterListener(ScriptableEventListener<T1, T2, T3> listener) => _listeners.Remove(listener);
    }
}