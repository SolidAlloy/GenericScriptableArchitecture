namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

#if UNIRX
    using UniRx;
#endif

    [Serializable]
    public class BaseEventReference
    {
        [SerializeField] internal EventType _eventType;

        [PublicAPI] public EventType Type => _eventType;

        public enum EventType { ScriptableEvent, EventInstancer }
    }

    [Serializable]
    public class EventReference : BaseEventReference, IScriptableEvent
    {
        [SerializeField] internal ScriptableEvent _event;
        [SerializeField] internal EventInstancer _eventInstancer;

        private IScriptableEvent Event => _eventType == EventType.ScriptableEvent ? _event : _eventInstancer;

#if UNIRX
        public IDisposable Subscribe(IObserver<Unit> observer) => Event?.Subscribe(observer) ?? Disposable.Empty;
#endif

        public void Invoke() => Event?.Invoke();

        public void AddListener(Action listener) => Event?.AddListener(listener);

        public void RemoveListener(Action listener) => Event?.RemoveListener(listener);

        public void AddListener(IListener listener) => Event?.AddListener(listener);

        public void RemoveListener(IListener listener) => Event?.AddListener(listener);

        #region Operator Overloads

        public static EventReference operator +(EventReference scriptableEvent, Action listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference operator -(EventReference scriptableEvent, Action listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventReference operator +(EventReference scriptableEvent, IListener listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference operator -(EventReference scriptableEvent, IListener listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }

    [Serializable]
    public class EventReference<T> : BaseEventReference, IScriptableEvent<T>
    {
        [SerializeField] internal ScriptableEvent<T> _event;
        [SerializeField] internal EventInstancer<T> _eventInstancer;

        private IScriptableEvent<T> Event => _eventType == EventType.ScriptableEvent ? _event : _eventInstancer;

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer) => Event?.Subscribe(observer) ?? Disposable.Empty;
#endif

        public void Invoke(T arg0) => Event?.Invoke(arg0);

        public void AddListener(Action<T> listener) => Event?.AddListener(listener);

        public void RemoveListener(Action<T> listener) => Event?.RemoveListener(listener);

        public void AddListener(IListener<T> listener) => Event?.AddListener(listener);

        public void RemoveListener(IListener<T> listener) => Event?.AddListener(listener);

        #region Operator Overloads

        public static EventReference<T> operator +(EventReference<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T> operator -(EventReference<T> scriptableEvent, Action<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T> operator +(EventReference<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T> operator -(EventReference<T> scriptableEvent, IListener<T> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }

    [Serializable]
    public class EventReference<T1, T2> : BaseEventReference, IScriptableEvent<T1, T2>
    {
        [SerializeField] internal ScriptableEvent<T1, T2> _event;
        [SerializeField] internal EventInstancer<T1, T2> _eventInstancer;

        private IScriptableEvent<T1, T2> Event => _eventType == EventType.ScriptableEvent ? _event : _eventInstancer;

#if UNIRX
        public IDisposable Subscribe(IObserver<(T1, T2)> observer) => Event?.Subscribe(observer) ?? Disposable.Empty;
#endif

        public void Invoke(T1 arg0, T2 arg1) => Event?.Invoke(arg0, arg1);

        public void AddListener(Action<T1, T2> listener) => Event?.AddListener(listener);

        public void RemoveListener(Action<T1, T2> listener) => Event?.RemoveListener(listener);

        public void AddListener(IListener<T1, T2> listener) => Event?.AddListener(listener);

        public void RemoveListener(IListener<T1, T2> listener) => Event?.AddListener(listener);

        #region Operator Overloads

        public static EventReference<T1, T2> operator +(EventReference<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T1, T2> operator -(EventReference<T1, T2> scriptableEvent, Action<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T1, T2> operator +(EventReference<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T1, T2> operator -(EventReference<T1, T2> scriptableEvent, IListener<T1, T2> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }

    [Serializable]
    public class EventReference<T1, T2, T3> : BaseEventReference, IScriptableEvent<T1, T2, T3>
    {
        [SerializeField] internal ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] internal EventInstancer<T1, T2, T3> _eventInstancer;

        private IScriptableEvent<T1, T2, T3> Event => _eventType == EventType.ScriptableEvent ? _event : _eventInstancer;

#if UNIRX
        public IDisposable Subscribe(IObserver<(T1, T2, T3)> observer) => Event?.Subscribe(observer) ?? Disposable.Empty;
#endif

        public void Invoke(T1 arg0, T2 arg1, T3 arg2) => Event?.Invoke(arg0, arg1, arg2);

        public void AddListener(Action<T1, T2, T3> listener) => Event?.AddListener(listener);

        public void RemoveListener(Action<T1, T2, T3> listener) => Event?.RemoveListener(listener);

        public void AddListener(IListener<T1, T2, T3> listener) => Event?.AddListener(listener);

        public void RemoveListener(IListener<T1, T2, T3> listener) => Event?.AddListener(listener);

        #region Operator Overloads

        public static EventReference<T1, T2, T3> operator +(EventReference<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T1, T2, T3> operator -(EventReference<T1, T2, T3> scriptableEvent, Action<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T1, T2, T3> operator +(EventReference<T1, T2, T3> scriptableEvent, IListener<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.AddListener(listener);
            return scriptableEvent;
        }

        public static EventReference<T1, T2, T3> operator -(EventReference<T1, T2, T3> scriptableEvent, IListener<T1, T2, T3> listener)
        {
            if (scriptableEvent == null)
                return null;

            scriptableEvent.RemoveListener(listener);
            return scriptableEvent;
        }

        #endregion
    }
}