namespace GenericScriptableArchitecture
{
    using System;
    using GenericUnityObjects;
    using UnityEngine;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T>")]
    public class ScriptableEvent<T> : BaseScriptableEvent, IScriptableEvent<T>
    {
        [SerializeField] private ScriptableEventHelper<T> _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        // _scriptableEventHelper will be null the first time a scriptable object of this type is created.
        private void OnEnable() => _scriptableEventHelper?.Initialize(this, name, "event");

        private void OnDisable() => _scriptableEventHelper?.Event?.Dispose();

        public void Invoke(T arg0) => _scriptableEventHelper.Invoke(arg0);

        #region Adding Removing Listeners

        public void AddListener(Action<T> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(Action<T> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        public void AddListener(IListener<T> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(IListener<T> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer) => _scriptableEventHelper.Event.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator +(ScriptableEvent<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T> operator -(ScriptableEvent<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}