namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities;
    using UnityEngine;

    using Object = UnityEngine.Object;

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable With History", MenuName = Config.PackageName + "Variable With History")]
    public class VariableWithHistory<T> : Variable<T>, IEvent<T, T>
#if UNIRX
        , IReactivePropertyWithHistory<T>
#endif
    {
        [SerializeField] internal T _previousValue;
        [SerializeField] internal bool ListenersWithHistoryExpanded;

        private EventHelperWithHistory<T> _eventHelper;

        public bool HasPreviousValue => HasPreviousValueInternal;

        [PublicAPI]
        public T PreviousValue => _previousValue;

        internal override List<Object> ListenersWithHistory => _eventHelper?.Listeners ?? ListHelper.Empty<Object>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _eventHelper = new EventHelperWithHistory<T>(this, () => HasPreviousValue, () => _previousValue, () => _value);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _eventHelper.Dispose();
        }

        #region Adding Removing Listeners

        public void AddListener(IListener<T, T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(IListener<T, T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(Action<T, T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(Action<T, T> listener) => _eventHelper.RemoveListener(listener);

        #endregion

        protected override void SetValue(T value)
        {
            HasPreviousValueInternal = true;
            _previousValue = _value;
            _value = value;
            AddStackTrace(_previousValue, _value);
            InvokeValueChangedEvents();
        }

        protected override void InitializeValues()
        {
            base.InitializeValues();
            HasPreviousValueInternal = false;
        }

        internal override void InvokeValueChangedEvents()
        {
            if ( ! CanBeInvoked())
                return;

            base.InvokeValueChangedEvents();
            _eventHelper.NotifyListeners(_previousValue, _value);
        }

#if UNIRX
        bool IReadOnlyReactivePropertyWithHistory<T>.HasValue => HasPreviousValue;

        public IDisposable Subscribe(Action<T, T> onNext) => _eventHelper.Subscribe(onNext);

        public IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError) => _eventHelper.Subscribe(onNext, onError);

        public IDisposable Subscribe(Action<T, T> onNext, Action onCompleted) => _eventHelper.Subscribe(onNext, onCompleted);

        public IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError, Action onCompleted) => _eventHelper.Subscribe(onNext, onError, onCompleted);

        public IDisposable Subscribe(IObserver<(T Previous, T Current)> observer) => _eventHelper.Subscribe(observer);
#endif

        #region Operator Overloads

        public override string ToString() => $"VariableWithHistory{{{Value}}}";

        public static VariableWithHistory<T> operator +(VariableWithHistory<T> variableWithHistory, Action<T, T> listener)
        {
            if (variableWithHistory == null)
                return null;

            variableWithHistory.AddListener(listener);
            return variableWithHistory;
        }

        public static Variable<T> operator +(VariableWithHistory<T> variableWithHistory, (Action<T, T> Listener, bool NotifyCurrentValue) args)
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

        public static Variable<T> operator +(VariableWithHistory<T> variableWithHistory, (IListener<T, T> Listener, bool NotifyCurrentValue) args)
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