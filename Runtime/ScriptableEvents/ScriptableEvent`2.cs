namespace GenericScriptableArchitecture
{
    using System;
    using GenericUnityObjects;
    using UnityEngine;

    [Serializable]
    [CreateGenericAssetMenu(MenuName = Config.PackageName + Config.Events + "ScriptableEvent<T1,T2>")]
    public class ScriptableEvent<T1, T2> : BaseScriptableEvent, IScriptableEvent<T1, T2>
    {
        [SerializeField] private ScriptableEventHelper<T1, T2> _scriptableEventHelper;

        internal override ScriptableEventHelper ScriptableEventHelper => _scriptableEventHelper;

        private void OnEnable() => _scriptableEventHelper?.Initialize(this, name, "event");

        private void OnDisable() => _scriptableEventHelper?.Event?.Dispose();

        public void Invoke(T1 arg0, T2 arg1) => _scriptableEventHelper.Invoke(arg0, arg1);

        #region Adding Removing Listeners

        public void AddListener(IListener<T1, T2> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(IListener<T1, T2> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

        public void AddListener(Action<T1, T2> listener) => _scriptableEventHelper.Event.AddListener(listener);

        public void RemoveListener(Action<T1, T2> listener) => _scriptableEventHelper.Event.RemoveListener(listener);

#if UNIRX
        public IDisposable Subscribe(IObserver<(T1, T2)> observer) => _scriptableEventHelper.Event.Subscribe(observer);
#endif

        #endregion

        #region Operator Overloads

        public static ScriptableEvent<T1, T2> operator +(ScriptableEvent<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2> operator -(ScriptableEvent<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2> operator +(ScriptableEvent<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static ScriptableEvent<T1, T2> operator -(ScriptableEvent<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}