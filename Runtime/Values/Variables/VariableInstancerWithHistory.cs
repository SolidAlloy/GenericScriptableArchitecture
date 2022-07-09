namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

    public class VariableInstancerWithHistory<T> : VariableInstancer<T>, IVariableWithHistory<T>
    {
        [SerializeField] internal VariableHelperWithHistory<T> _variableHelperWithHistory;

        public bool HasPreviousValue => _variableHelperWithHistory.HasPreviousValue;

        [PublicAPI]
        public T PreviousValue => _variableHelperWithHistory.PreviousValue;

        VariableHelperWithHistory IVariableWithHistory.VariableHelperWithHistory => _variableHelperWithHistory;

        protected override void Awake()
        {
            base.Awake();
            _variableHelperWithHistory.Initialize(this, name, "variable instancer", () => _variableHelper.Value);
            _variableHelperWithHistory.HasPreviousValue = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _variableHelperWithHistory.Event.Dispose();
        }

        protected override void SetValue(T value) => _variableHelperWithHistory.SetValue(_variableHelper, value);

        #region Adding Removing Listeners

        public void AddListener(IListener<T, T> listener, bool notifyCurrentValue = false) => _variableHelperWithHistory.Event.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(IListener<T, T> listener) => _variableHelperWithHistory.Event.RemoveListener(listener);

        public void AddListener(Action<T, T> listener, bool notifyCurrentValue = false) => _variableHelperWithHistory.Event.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(Action<T, T> listener) => _variableHelperWithHistory.Event.RemoveListener(listener);

        #endregion

#if UNIRX
        bool IReadOnlyReactivePropertyWithHistory<T>.HasValue => HasPreviousValue;

        public IDisposable Subscribe(Action<T, T> onNext) => _variableHelperWithHistory.Event.Subscribe(onNext);

        public IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError) => _variableHelperWithHistory.Event.Subscribe(onNext, onError);

        public IDisposable Subscribe(Action<T, T> onNext, Action onCompleted) => _variableHelperWithHistory.Event.Subscribe(onNext, onCompleted);

        public IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError, Action onCompleted) => _variableHelperWithHistory.Event.Subscribe(onNext, onError, onCompleted);

        public IDisposable Subscribe(IObserver<(T Previous, T Current)> observer) => _variableHelperWithHistory.Event.Subscribe(observer);
#endif

        #region Operator Overloads

        public override string ToString() => $"VariableWithHistory '{name}' {{{Value}}}";

        public static VariableInstancerWithHistory<T> operator +(VariableInstancerWithHistory<T> variableWithHistory, Action<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(listener);
            return variableWithHistory;
        }

        public static VariableInstancerWithHistory<T> operator +(VariableInstancerWithHistory<T> variableWithHistory, (Action<T, T> Listener, bool NotifyCurrentValue) args)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(args.Listener, args.NotifyCurrentValue);
            return variableWithHistory;
        }

        public static VariableInstancerWithHistory<T> operator -(VariableInstancerWithHistory<T> variableWithHistory, Action<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.RemoveListener(listener);
            return variableWithHistory;
        }

        public static VariableInstancerWithHistory<T> operator +(VariableInstancerWithHistory<T> variableWithHistory, IListener<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(listener);
            return variableWithHistory;
        }

        public static VariableInstancerWithHistory<T> operator +(VariableInstancerWithHistory<T> variableWithHistory, (IListener<T, T> Listener, bool NotifyCurrentValue) args)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(args.Listener, args.NotifyCurrentValue);
            return variableWithHistory;
        }

        public static VariableInstancerWithHistory<T> operator -(VariableInstancerWithHistory<T> variableWithHistory, IListener<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.RemoveListener(listener);
            return variableWithHistory;
        }

        #endregion
    }
}