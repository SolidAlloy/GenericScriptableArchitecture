namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    internal interface IInlineDrawer
    {
        bool HasContent { get; }

        void OnInlineGUI();
    }

    internal static class InlineEditorCache
    {
        // for some reason using UnityEngine.Object as dictionary key doesn't work and we have to use instance ids instead.
        private static readonly Dictionary<int, IInlineDrawer> _editorCache = new Dictionary<int, IInlineDrawer>();

        public static IInlineDrawer GetInlineDrawer(Object variable)
        {
            if ( ! _editorCache.TryGetValue(variable.GetInstanceID(), out IInlineDrawer drawer))
            {
                drawer = CreateInlineDrawer(variable);
                _editorCache.Add(variable.GetInstanceID(), drawer);
            }

            return drawer;
        }

        [InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            // The previously created serialized objects become broken after exiting play mode,
            // so we need to discard the cache and create new serialized objects if we enter play mode again without a domain reload
            _editorCache.Clear();
        }

        private static IInlineDrawer CreateInlineDrawer(Object value)
        {
            var type = value.GetType();

            if (value is BaseScriptableEvent)
                return EditorHelper.CreateEditor<ScriptableEventEditor>(value);

            if (value is BaseEventInstancer)
                return EditorHelper.CreateEditor<EventInstancerEditor>(value);

            var genericType = type.BaseType;

            if (!genericType.IsGenericType)
                return null;

            var genericTypeDefinition = genericType.GetGenericTypeDefinition();

            if (genericTypeDefinition == typeof(VariableWithHistory<>))
                return new VariableWithHistoryEditor(new SerializedObject(value), (IVariableWithHistory) value, null);

            if (genericTypeDefinition == typeof(Variable<>))
                return new GenericVariableEditor(new SerializedObject(value), (IVariable) value, null);

            if (genericTypeDefinition == typeof(Constant<>))
                return EditorHelper.CreateEditor<ConstantEditor>(value);

            if (genericTypeDefinition == typeof(VariableInstancerWithHistory<>))
                return new VariableInstancerWithHistoryEditor((IVariableWithHistory) value, new SerializedObject(value), null, null);

            if (genericTypeDefinition == typeof(VariableInstancer<>))
                return new GenericVariableInstancerEditor((BaseVariableInstancer) value, new SerializedObject(value), null, null);

            return null;
        }
    }
}