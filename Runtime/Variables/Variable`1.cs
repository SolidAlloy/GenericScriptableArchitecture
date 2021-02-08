namespace ExtendedScriptableObjects
{
    using System;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;
    using UnityEngine.Assertions;

    [CreateGenericAssetMenu(FileName = "New Variable")]
    [Serializable]
    public class Variable<T> : PlayModeUnchangedScriptableObject
    {
        [ResizableTextArea, UsedImplicitly]
        [SerializeField] private string _description;

        [PublicAPI] public T Value;

        public static implicit operator T(Variable<T> variable) => variable.Value;

        public static implicit operator Variable<T>(T value)
        {
             var variable = CreateInstance<Variable<T>>();
             Assert.IsNotNull(variable);
             variable.Value = value;
             return variable;
        }

        public override string ToString() => Value.ToString();
    }
}