namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(menuName = Config.PackageName + Config.Events + "ScriptableEvent")]
    public class ScriptableEvent : BaseScriptableEvent
    {
        private List<ScriptableEventListener> _listeners = new List<ScriptableEventListener>();
        private List<Action> _responses = new List<Action>();

        internal override List<BaseScriptableEventListener> Listeners
            => _listeners.ConvertAll(item => (BaseScriptableEventListener) item);

        internal override List<UnityEngine.Object> ResponseTargets
            => _responses.Select(response => response.Target)
                            .OfType<UnityEngine.Object>()
                            .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void Invoke()
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace();

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised();
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke();
            }
        }

        public void AddListener(ScriptableEventListener listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener listener) => _listeners.Remove(listener);

        public void AddResponse(Action response) => _responses.Add(response);

        public void RemoveResponse(Action response) => _responses.Remove(response);
    }
}