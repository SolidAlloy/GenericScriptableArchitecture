namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public class GeneratedComponentsDebugDrawer
    {
        private static readonly List<GameObject> _debuggedGameObjects = new List<GameObject>();

        private GUIContent _showGeneratedLabel;
        private bool _debugFoldoutOpen;
        private SerializedProperty _componentProperty;
        private GameObject _targetGameObject;
        private Func<IReadOnlyCollection<Component>> _getWildcardComponents;
        private Func<IReadOnlyCollection<Component>> _getBaseComponents;

        public bool ShowGeneratedComponents { get; private set; }

        public GeneratedComponentsDebugDrawer(SerializedProperty componentProperty, GameObject targetGameObject, Func<IReadOnlyCollection<Component>> getWildcardComponents, Func<IReadOnlyCollection<Component>> getBaseComponents)
        {
            _showGeneratedLabel = new GUIContent("Show generated components",
                "Show auto-generated components attached to this game object that are hidden by default. Use this if the component goes missing for some reason.");

            _targetGameObject = targetGameObject;
            ShowGeneratedComponents = _debuggedGameObjects.Contains(_targetGameObject);
            _debugFoldoutOpen = ShowGeneratedComponents;
            _componentProperty = componentProperty;
            _getWildcardComponents = getWildcardComponents;
            _getBaseComponents = getBaseComponents;
        }

        public void DrawComponentProperty()
        {
            if (ShowGeneratedComponents)
                EditorGUILayout.PropertyField(_componentProperty);
        }

        public void DrawDebugFlag()
        {
            var wildcardComponentsCount = _getWildcardComponents().Count;
            var genericComponentsCount = _getBaseComponents().Count;

            if (genericComponentsCount > wildcardComponentsCount)
                EditorGUILayout.HelpBox("The game object has more generated listeners than wildcard listener components (the visible ones). Tick 'Show Generated Components' checkbox in Debug to resolve the issue and remove invalid generated components.", MessageType.Warning);

            _debugFoldoutOpen = EditorGUILayout.Foldout(_debugFoldoutOpen, "Debug", true);

            if (!_debugFoldoutOpen)
                return;

            bool newValue = EditorGUILayout.Toggle(_showGeneratedLabel, ShowGeneratedComponents);

            if (ShowGeneratedComponents == newValue)
                return;

            if (ShowGeneratedComponents && !newValue) // hide components
            {
                SetHideFlagsForComponents(_getBaseComponents(), HideFlags.HideInInspector);
                _debuggedGameObjects.Remove(_targetGameObject);
            }
            else if (!ShowGeneratedComponents && newValue) // show components
            {
                SetHideFlagsForComponents(_getBaseComponents(), HideFlags.None);
                _debuggedGameObjects.Add(_targetGameObject);
            }

            ShowGeneratedComponents = newValue;
        }

        private static void SetHideFlagsForComponents(IEnumerable<Component> components, HideFlags flags)
        {
            foreach (var component in components)
            {
                var editor = Editor.CreateEditor(component);
                editor.serializedObject.SetHideFlagsPersistently(flags);
            }
        }
    }
}