namespace GenericScriptableArchitecture
{
    using System;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEngine;

    [Serializable]
    internal abstract class VariableHelperWithHistory
    {
        [SerializeField] public bool ListenersWithHistoryExpanded;
        [NonSerialized] public bool HasPreviousValue;

        public abstract EventHelperWithHistory EventHelper { get; }

        public abstract void SetPreviousValue(object value);

        public abstract void InvokeValueChangedEvents(object currentValue);
    }

    [Serializable]
    internal class VariableHelperWithHistory<T> : VariableHelperWithHistory
    {
        [SerializeField] public T PreviousValue;

        public EventHelperWithHistory<T> Event;

        public override EventHelperWithHistory EventHelper => Event;

        private string _objectName;
        private string _typeName;

        public void Initialize(IVariableWithHistory<T> variable, string objectName, string typeName, Func<T> getCurrentValue)
        {
            _objectName = objectName;
            _typeName = typeName;
            Event = new EventHelperWithHistory<T>(variable, () => HasPreviousValue, () => PreviousValue, getCurrentValue);
        }

        public override void InvokeValueChangedEvents(object currentValue)
        {
            InvokeValueChangedEvents((T) currentValue);
        }

        public void InvokeValueChangedEvents(T currentValue)
        {
            if ( ! BaseEvent.CanBeInvoked(_objectName, _typeName))
                return;

            Event.NotifyListeners(PreviousValue, currentValue);
        }

        public override void SetPreviousValue(object value)
        {
            PreviousValue = (T) value;
        }

        public void SetValue(VariableHelper<T> variableHelper, T value)
        {
            HasPreviousValue = true;
            PreviousValue = variableHelper.Value;
            variableHelper.Value = value;
            variableHelper.StackTrace.AddStackTrace(PreviousValue, variableHelper.Value);
            variableHelper.InvokeValueChangedEvents();
            InvokeValueChangedEvents(value);
        }
    }

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable With History", MenuName = Config.PackageName + "Variable With History")]
    public class VariableWithHistory<T> : Variable<T>, IVariableWithHistory<T>
    {
        [SerializeField] internal VariableHelperWithHistory<T> _variableHelperWithHistory;

        public bool HasPreviousValue => _variableHelperWithHistory.HasPreviousValue;

        [PublicAPI]
        public T PreviousValue => _variableHelperWithHistory.PreviousValue;

        VariableHelperWithHistory IVariableWithHistory.VariableHelperWithHistory => _variableHelperWithHistory;

        protected override void OnDisable()
        {
            base.OnDisable();
            _variableHelperWithHistory.Event?.Dispose();
        }

        protected override void SetValue(T value) => _variableHelperWithHistory.SetValue(_variableHelper, value);

        protected override void InitializeValues()
        {
            base.InitializeValues();

            _variableHelperWithHistory.Initialize(this, name, "variable", () => _variableHelper.Value);
            _variableHelperWithHistory.HasPreviousValue = false;
        }

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

        public static VariableWithHistory<T> operator +(VariableWithHistory<T> variableWithHistory, Action<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(listener);
            return variableWithHistory;
        }

        public static VariableWithHistory<T> operator +(VariableWithHistory<T> variableWithHistory, (Action<T, T> Listener, bool NotifyCurrentValue) args)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(args.Listener, args.NotifyCurrentValue);
            return variableWithHistory;
        }

        public static VariableWithHistory<T> operator -(VariableWithHistory<T> variableWithHistory, Action<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.RemoveListener(listener);
            return variableWithHistory;
        }

        public static VariableWithHistory<T> operator +(VariableWithHistory<T> variableWithHistory, IListener<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(listener);
            return variableWithHistory;
        }

        public static VariableWithHistory<T> operator +(VariableWithHistory<T> variableWithHistory, (IListener<T, T> Listener, bool NotifyCurrentValue) args)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(args.Listener, args.NotifyCurrentValue);
            return variableWithHistory;
        }

        public static VariableWithHistory<T> operator -(VariableWithHistory<T> variableWithHistory, IListener<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.RemoveListener(listener);
            return variableWithHistory;
        }

        #endregion
    }
}