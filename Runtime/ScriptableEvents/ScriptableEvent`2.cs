namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = "Events/ScriptableEvent<T1,T2>")]
    public class ScriptableEvent<T1, T2> : ScriptableEventBase
    {
        private List<ScriptableEventListener<T1, T2>> _listeners = new List<ScriptableEventListener<T1, T2>>();

        internal override List<ScriptableEventListenerBase> Listeners => _listeners.ConvertAll(item => (ScriptableEventListenerBase) item);

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T1 arg0, T2 arg1)
        {
            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0, arg1);
            }
        }

        public void RegisterListener(ScriptableEventListener<T1, T2> listener) => _listeners.Add(listener);

        public void UnregisterListener(ScriptableEventListener<T1, T2> listener) => _listeners.Remove(listener);
    }
}