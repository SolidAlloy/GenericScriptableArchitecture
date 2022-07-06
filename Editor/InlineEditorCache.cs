namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    internal static class InlineEditorCache
    {
        private static readonly Dictionary<Object, Editor> _editorCache = new Dictionary<Object, Editor>();
        
        public static Editor GetInlineEditor<T>(Object variable) where T : Editor
        {
            if ( ! _editorCache.TryGetValue(variable, out Editor editor))
            {
                editor = EditorHelper.CreateEditor<T>(variable);
                _editorCache.Add(variable, editor);
            }

            return editor;
        }
    }
}