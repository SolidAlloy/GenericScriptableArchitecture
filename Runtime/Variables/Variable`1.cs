namespace ExtendedScriptableObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using ScriptableEvents;
    using SolidUtilities.Attributes;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
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
            // OnEnable is called after domain reload in edit mode too. If _initialValue is assigned to other values
            // in edit mode, it causes all kinds of weird behaviour.
            // For example, _initialValue and _value become impossible to edit in inspector if T is a reference type.
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
#endif
            {
                _value = _initialValue;
                _previousValue = _initialValue;
            }
        }

        public static implicit operator T(Variable<T> variable) => variable.Value;

        public override string ToString() => $"Variable{{{Value}}}";
    }
}