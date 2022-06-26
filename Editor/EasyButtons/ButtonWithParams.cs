namespace EasyButtons.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    internal class ButtonWithParams : Button
    {
        private readonly Parameter[] _parameters;
        private bool _expanded = true;

        public ButtonWithParams(MethodInfo method, (ParameterInfo parameter, Func<string> getArgName)[] parameters)
            : base(method)
        {
            _parameters = parameters.Select(paramData => new Parameter(paramData.parameter, paramData.getArgName)).ToArray();
        }

        protected override void DrawInternal(IEnumerable<object> targets)
        {
            (Rect foldoutRect, Rect buttonRect) = DrawUtility.GetFoldoutAndButtonRects(DisplayName);

            _expanded = DrawUtility.DrawInFoldout(foldoutRect, _expanded, DisplayName, () =>
            {
                foreach (Parameter param in _parameters)
                {
                    param.Draw();
                }
            });

            if ( ! GUI.Button(buttonRect, "Invoke"))
                return;

            var paramValues = _parameters.Select(param => param.Value).ToArray();

            foreach (object obj in targets)
            {
                Method.Invoke(obj, paramValues);
            }
        }

        private readonly struct Parameter
        {
            private readonly FieldInfo _fieldInfo;
            private readonly ScriptableObject _scriptableObj;
            private readonly Editor _editor;
            private readonly SerializedProperty _paramProperty;
            private readonly Func<string> _getCustomLabel;
            private readonly GUIContent _guiContent;

            public Parameter(ParameterInfo paramInfo, Func<string> getCustomLabel)
            {
                Type generatedType = ScriptableObjectCache.GetClass(paramInfo.Name, paramInfo.ParameterType);
                _scriptableObj = ScriptableObject.CreateInstance(generatedType);
                _fieldInfo = generatedType.GetField(paramInfo.Name);
                _editor = Editor.CreateEditor(_scriptableObj);
                _paramProperty = _editor.serializedObject.FindProperty(paramInfo.Name);
                _getCustomLabel = getCustomLabel;
                _guiContent = getCustomLabel != null ? new GUIContent() : null;
            }

            public object Value => _fieldInfo.GetValue(_scriptableObj);

            public void Draw()
            {
                if (_editor.target == null)
                    return;

                _editor.serializedObject.UpdateIfRequiredOrScript();

                if (_getCustomLabel != null)
                {
                    _guiContent.text = _getCustomLabel();
                }

                EditorGUILayout.PropertyField(_paramProperty, _guiContent);
                _editor.serializedObject.ApplyModifiedProperties();

                LogHelper.RemoveLogEntriesByMode(LogModes.NoScriptAssetWarning);
            }
        }
    }
}