namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T>")]
    public class ScriptableEvent<T> : ScriptableEventBase
    {
        private List<ScriptableEventListener<T>> _listeners = new List<ScriptableEventListener<T>>();
        private List<Action<T>> _responses = new List<Action<T>>();

        internal override List<ScriptableEventListenerBase> Listeners => _listeners.ConvertAll(item => (ScriptableEventListenerBase) item);

        internal override List<UnityEngine.Object> ResponseTargets
            => _responses
                .Select(response => response.Target)
                .OfType<UnityEngine.Object>()
                .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T arg0)
        {
            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(arg0);
            }
        }

        public void RegisterListener(ScriptableEventListener<T> listener) => _listeners.Add(listener);

        public void UnregisterListener(ScriptableEventListener<T> listener) => _listeners.Remove(listener);

        public void AddResponse(Action<T> response) => _responses.Add(response);

        public void RemoveResponse(Action<T> response) => _responses.Remove(response);
    }
}