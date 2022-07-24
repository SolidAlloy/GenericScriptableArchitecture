namespace GenericScriptableArchitecture.Editor
{
    using System;
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.Editor.Util;
    using SolidUtilities.Editor;
    using TypeReferences;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(ScriptableEventListener))]
    public class WildcardScriptableEventListenerEditor : Editor
    {
        private const string WildcardListenerKey = "ListenerEditor_WildcardListener";
        private const string ListenerTypeKey = "ListenerEditor_ListenerType";
        private const string EventKey = "ListenerEditor_EventReference";

        private GameObject _targetGameObject;
        private GeneratedComponentsDebugDrawer _debugDrawer;
        private SerializedProperty _componentProperty;
        private ScriptableEventListenerEditorHelper _componentEditor;

        private void OnEnable()
        {
            if (target == null)
                return;

            _componentProperty = GetComponentProperty(serializedObject);

            if (_componentProperty.objectReferenceValue != null)
            {
                _componentEditor = CreateComponentEditor(_componentProperty.objectReferenceValue, Repaint);
            }

            _targetGameObject = ((Component)target).gameObject;

            _debugDrawer = new GeneratedComponentsDebugDrawer(_componentProperty, _targetGameObject,
                () => _targetGameObject.GetComponents<ScriptableEventListener>(),
                () => _targetGameObject.GetComponents<BaseScriptableEventListener>());

            if (!_debugDrawer.ShowGeneratedComponents && _componentEditor != null)
                _componentEditor.SerializedObject.SetHideFlagsPersistently(HideFlags.HideInInspector);
        }

        private static SerializedProperty GetComponentProperty(SerializedObject serializedObject) => serializedObject.FindProperty(nameof(ScriptableEventListener._component));

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                var scriptProperty = serializedObject.FindProperty("m_Script");
                if (scriptProperty != null) EditorGUILayout.PropertyField(scriptProperty);
                return;
            }

            serializedObject.UpdateIfRequiredOrScript();

            _debugDrawer.DrawComponentProperty();

            if (_componentProperty.objectReferenceValue != null && _componentEditor == null)
                _componentEditor = CreateComponentEditor(_componentProperty.objectReferenceValue, Repaint);

            DrawObjectField();

            _componentEditor?.DrawInlineGUI();

            _debugDrawer.DrawDebugFlag();
            serializedObject.ApplyModifiedProperties();
        }

        private static ScriptableEventListenerEditorHelper CreateComponentEditor(Object componentObject, Action repaint)
        {
            var componentSerializedObject = new SerializedObject(componentObject);
            return new ScriptableEventListenerEditorHelper(componentSerializedObject, (BaseScriptableEventListener) componentObject, repaint);
        }

        private void DrawObjectField()
        {
            var oldEvent = _componentEditor?.GetCurrentEventValue();

            EditorGUI.BeginChangeCheck();
            var newEvent = GenericObjectDrawer.ObjectField("Event", oldEvent, typeof(IBaseEvent), true);

            if (EditorGUI.EndChangeCheck() && oldEvent != newEvent)
                ChangeEvent(newEvent);
        }

        private void ChangeEvent(Object newEvent)
        {
            Type newListenerType = GetListenerComponentType(newEvent);

            if (newListenerType == null)
            {
                RemoveComponent(_componentProperty.objectReferenceValue);
                _componentEditor = null;
                return;
            }

            // No need to replace component, if it's already of a matching type. Just replace the event reference in it.
            if (newListenerType.IsInstanceOfType(_componentProperty.objectReferenceValue))
            {
                _componentEditor.SetEventValue(newEvent);
                _componentEditor.SerializedObject.ApplyModifiedProperties();
                return;
            }

            RemoveComponent(_componentProperty.objectReferenceValue);
            _componentEditor = null;

            var component = AddComponentHelper.AddComponent(_targetGameObject, newListenerType, out bool reloadRequired) as BaseScriptableEventListener;

            if (reloadRequired)
            {
                PersistentStorage.SaveData(WildcardListenerKey, (Component) target);
                PersistentStorage.SaveData(ListenerTypeKey, new TypeReference(newListenerType));
                PersistentStorage.SaveData(EventKey, newEvent);
                PersistentStorage.ExecuteOnScriptsReload(OnAfterComponentAdded);
                AssetDatabase.Refresh();
            }
            else
            {
                _componentProperty.objectReferenceValue = component;
                _componentEditor = new ScriptableEventListenerEditorHelper(new SerializedObject(component), component, Repaint);
                _componentEditor.SetEventValue(newEvent);
                _componentEditor.SerializedObject.SetHideFlagsPersistently(HideFlags.HideInInspector);
            }
        }

        private static void OnAfterComponentAdded()
        {
            try
            {
                var wildcardListener = PersistentStorage.GetData<Component>(WildcardListenerKey);
                var componentType = PersistentStorage.GetData<TypeReference>(ListenerTypeKey).Type;
                var newEvent = PersistentStorage.GetData<Object>(EventKey);

                var component = wildcardListener.gameObject.GetComponent(componentType) as BaseScriptableEventListener;

                var wildcardSerializedObject = new SerializedObject(wildcardListener);
                var componentProperty = GetComponentProperty(wildcardSerializedObject);
                componentProperty.objectReferenceValue = component;
                wildcardSerializedObject.ApplyModifiedProperties();

                var componentEditor = new ScriptableEventListenerEditorHelper(new SerializedObject(component), component, null);
                componentEditor.SetEventValue(newEvent);
                componentEditor.SerializedObject.SetHideFlagsPersistently(HideFlags.HideInInspector);
            }
            finally
            {
                PersistentStorage.DeleteData(WildcardListenerKey);
                PersistentStorage.DeleteData(ListenerTypeKey);
                PersistentStorage.DeleteData(EventKey);
            }
        }

        private void RemoveComponent(Object component)
        {
            if (component == null)
                return;

            Undo.DestroyObjectImmediate(component);
        }

        private static Type GetListenerComponentType(Object newEvent)
        {
            if (newEvent == null)
                return null;

            var concreteEventType = newEvent.GetType();

            var eventType = concreteEventType.BaseType;

            if (!eventType.IsGenericType)
            {
                if (concreteEventType == typeof(ScriptableEvent) || concreteEventType == typeof(EventInstancer))
                {
                    return typeof(VoidScriptableEventListener);
                }

                Debug.LogError($"The type is not a known type: {concreteEventType}");
                return null;
            }

            var eventTypeDefinition = eventType.GetGenericTypeDefinition();

            if (eventTypeDefinition == typeof(Constant<>))
            {
                Debug.LogWarning("Constants don't change, so the event listener will never receive an event from them.");
                return null;
            }

            if (eventTypeDefinition == typeof(VariableWithHistory<>) || eventTypeDefinition == typeof(VariableInstancerWithHistory<>))
            {
                var genericArg = eventType.GenericTypeArguments[0];
                return typeof(ScriptableEventListener<,>).MakeGenericType(genericArg, genericArg);
            }

            var genericArgsCount = eventType.GetGenericArguments().Length;

            if (genericArgsCount == 1)
            {
                return typeof(ScriptableEventListener<>).MakeGenericType(eventType.GenericTypeArguments);
            }

            if (genericArgsCount == 2)
            {
                return typeof(ScriptableEventListener<,>).MakeGenericType(eventType.GenericTypeArguments);
            }

            if (genericArgsCount == 3)
            {
                return typeof(ScriptableEventListener<,,>).MakeGenericType(eventType.GenericTypeArguments);
            }

            return null;
        }
    }
}