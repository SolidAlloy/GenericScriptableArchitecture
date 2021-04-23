namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T1,T2>")]
    public class ScriptableEvent<T1, T2> : ScriptableEventBase
    {
        private List<ScriptableEventListener<T1, T2>> _listeners = new List<ScriptableEventListener<T1, T2>>();
        private List<Action<T1, T2>> _responses = new List<Action<T1, T2>>();

        internal override List<ScriptableEventListenerBase> Listeners
            => _listeners.ConvertAll(item => (ScriptableEventListenerBase) item);

        internal override List<UnityEngine.Object> ResponseTargets
            => _responses
                .Select(response => response.Target)
                .OfType<UnityEngine.Object>()
                .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T1 arg0, T2 arg1)
        {
            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0, arg1);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(arg0, arg1);
            }
        }

        public void RegisterListener(ScriptableEventListener<T1, T2> listener) => _listeners.Add(listener);

        public void UnregisterListener(ScriptableEventListener<T1, T2> listener) => _listeners.Remove(listener);

        public void AddResponse(Action<T1, T2> response) => _responses.Add(response);

        public void RemoveResponse(Action<T1, T2> response) => _responses.Remove(response);
    }
}