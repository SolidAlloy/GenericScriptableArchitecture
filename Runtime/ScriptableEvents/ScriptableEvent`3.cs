namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using GenericUnityObjects;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T1,T2,T3>")]
    public class ScriptableEvent<T1, T2, T3> : BaseScriptableEvent
    {
        private List<ScriptableEventListener<T1, T2, T3>> _listeners = new List<ScriptableEventListener<T1, T2, T3>>();
        private List<Action<T1, T2, T3>> _responses = new List<Action<T1, T2, T3>>();

        internal override List<BaseScriptableEventListener> Listeners
            => _listeners.ConvertAll(item => (BaseScriptableEventListener) item);

        internal override List<UnityEngine.Object> ResponseTargets
            => _responses
                .Select(response => response.Target)
                .OfType<UnityEngine.Object>()
                .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode, Expanded = true)]
        public void Invoke(T1 arg0, T2 arg1, T3 arg2)
        {
            AddStackTrace(arg0, arg1, arg2);

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(arg0, arg1, arg2);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(arg0, arg1, arg2);
            }
        }

        public void AddListener(ScriptableEventListener<T1, T2, T3> listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener<T1, T2, T3> listener) => _listeners.Remove(listener);

        public void AddResponse(Action<T1, T2, T3> response) => _responses.Add(response);

        public void RemoveResponse(Action<T1, T2, T3> response) => _responses.Remove(response);
    }
}