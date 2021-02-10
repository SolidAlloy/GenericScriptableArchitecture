namespace GenericScriptableArchitecture
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using SolidUtilities.UnityEngineInternals;
    using UnityEditor;
    using UnityEngine;
#if UNITY_EDITOR

#endif

    [CreateGenericAssetMenu(FileName = "New Variable")]
    [Serializable]
    public class Variable<T> : VariableBase
    {
        [ResizableTextArea, UsedImplicitly]
        [SerializeField] private string _description;

        [SerializeField] private T _initialValue;
        [SerializeField] private T _value;
        [SerializeField] private T _previousValue;

        [SerializeField] private ScriptableEvent<T> _changed;
        [SerializeField] private ScriptableEvent<T, T> _changedWithHistory;

        [SuppressMessage("ReSharper", "RCS1146",
            Justification = "Conditional access on ScriptableEvent bypasses overriden equality operator")]
        public T Value
        {
            get => _value;
            set
            {
                _previousValue = _value;
                _value = value;

                if (_changed != null) _changed.Invoke(_value);
                if (_changedWithHistory != null) _changedWithHistory.Invoke(_previousValue, _value);
            }
        }

        [PublicAPI]
        public T PreviousValue => _previousValue;

        private void OnEnable()
        {
            // DeepCopy() is not very performant, so execute it only in Play Mode.
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
#endif
            {
                _value = _initialValue.DeepCopy();
                _previousValue = _initialValue.DeepCopy();
            }
        }

        public static implicit operator T(Variable<T> variable) => variable.Value;

        public override string ToString() => $"Variable{{{Value}}}";
    }
}