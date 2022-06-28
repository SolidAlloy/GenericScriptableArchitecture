namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.Editor.MonoBehaviours;
    using GenericUnityObjects.Editor.Util;
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

        private static readonly List<GameObject> _debuggedGameObjects = new List<GameObject>();

        private ScriptableEventListener _target;
        private ScriptableEventListenerEditor _componentEditor;
        private BaseScriptableEventListener _component;
        private SerializedProperty _componentProperty;
        private bool _showGeneratedComponents;
        private GUIContent _showGeneratedLabel;
        private bool _debugFoldoutOpen;

        private void OnEnable()
        {
            _showGeneratedLabel = new GUIContent("Show generated components",
                "Show auto-generated components attached to this game object that are hidden by default. Use this if the component goes missing for some reason.");

            _target = (ScriptableEventListener) target;

            if (_target != null)
            {
                _showGeneratedComponents = _debuggedGameObjects.Contains(_target.gameObject);
                _debugFoldoutOpen = _showGeneratedComponents;
                _componentProperty = serializedObject.FindProperty(nameof(ScriptableEventListener._component));
            }
        }

        private void OnDisable()
        {
            if (target == null)
            {
                RemoveComponent(_component);
            }
        }

        public override void OnInspectorGUI()
        {
            if (_target == null)
            {
                var scriptProperty = serializedObject.FindProperty("m_Script");
                if (scriptProperty != null) EditorGUILayout.PropertyField(scriptProperty);
                return;
            }

            serializedObject.UpdateIfRequiredOrScript();
            CreateComponentEditorIfNeeded();

            if (_showGeneratedComponents)
                EditorGUILayout.PropertyField(_componentProperty);

            DrawObjectField();
            DrawUnityEvent();
            DrawDebugFlag();
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateComponentEditorIfNeeded()
        {
            if (_component == _target._component && _componentEditor != null)
                return;

            _component = _target._component;

            if (_component == null)
                return;

            _componentEditor = (ScriptableEventListenerEditor) CreateEditor(_component, typeof(ScriptableEventListenerEditor));
            _componentEditor.ShowEventField = false;  // for debug, we want to draw object field in normal editors, but still not draw them in inlined ones

            if (!_showGeneratedComponents)
                SetHideFlagsPersistently(_componentEditor.serializedObject, HideFlags.HideInInspector);

            // It is necessary to change DrawObjectField after HideFlags because serializedObject.ApplyModifiedProperties() inside SetHideFlags discards the changed value of DrawObjectField
            _component.DrawObjectField = _showGeneratedComponents;
        }

        private static void SetHideFlagsPersistently(SerializedObject serializedObject, HideFlags flags)
        {
            // The only way to set the hide flags persistently.
            var hideFlagsProp = serializedObject.FindProperty("m_ObjectHideFlags");
            int flagsValue = (int) flags;

            if (hideFlagsProp.intValue != flagsValue)
            {
                hideFlagsProp.intValue = flagsValue;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawObjectField()
        {
            var oldEvent = GetCurrentEvent();

            if (_target._component != null)
                Undo.RecordObject(_target._component, "Changed event field");

            var newEvent = GenericObjectDrawer.ObjectField("Event", oldEvent, typeof(BaseEvent), true);

            if (newEvent != oldEvent)
                ChangeEvent(newEvent);
        }

        private void DrawDebugFlag()
        {
            var wildcardComponentsCount = _target.GetComponents<ScriptableEventListener>().Length;
            var genericComponentsCount = _target.GetComponents<BaseScriptableEventListener>().Length;

            if (genericComponentsCount > wildcardComponentsCount)
                EditorGUILayout.HelpBox("The game object has more generated listeners than wildcard listener components (the visible ones). Tick 'Show Generated Components' checkbox in Debug to resolve the issue and remove invalid generated components.", MessageType.Warning);

            _debugFoldoutOpen = EditorGUILayout.Foldout(_debugFoldoutOpen, "Debug", true);

            if (!_debugFoldoutOpen)
                return;

            bool newValue = EditorGUILayout.Toggle(_showGeneratedLabel, _showGeneratedComponents);

            if (_showGeneratedComponents == newValue)
                return;

            if (_showGeneratedComponents && !newValue) // hide components
            {
                SetHideFlagsForComponents(_target.GetComponents<BaseScriptableEventListener>(), HideFlags.HideInInspector);
                _debuggedGameObjects.Remove(_target.gameObject);
            }
            else if (!_showGeneratedComponents && newValue) // show components
            {
                SetHideFlagsForComponents(_target.GetComponents<BaseScriptableEventListener>(), HideFlags.None);
                _debuggedGameObjects.Add(_target.gameObject);
            }

            _showGeneratedComponents = newValue;
        }

        private static void SetHideFlagsForComponents(IEnumerable<Component> components, HideFlags flags)
        {
            foreach (var component in components)
            {
                var editor = CreateEditor(component);
                SetHideFlagsPersistently(editor.serializedObject, flags);
            }
        }

        private void ChangeEvent(Object newEvent)
        {
            RemoveComponent(_target._component);

            var listenerType = GetListenerComponentType(newEvent);

            if (listenerType.GenericTypeDefinition == null)
                return;

            var component = AddComponent(listenerType.GenericTypeDefinition, listenerType.GenericArgs, out bool reloadRequired) as BaseScriptableEventListener;

            if (reloadRequired)
            {
                PersistentStorage.SaveData(WildcardListenerKey, _target);
                PersistentStorage.SaveData(ListenerTypeKey, new TypeReference(listenerType.GenericTypeDefinition.MakeGenericType(listenerType.GenericArgs)));
                PersistentStorage.SaveData(EventKey, newEvent);
                PersistentStorage.ExecuteOnScriptsReload(OnAfterComponentAdded);
                AssetDatabase.Refresh();
            }
            else
            {
                ChangeHiddenComponent(_target, component, newEvent as BaseEvent);
            }
        }

        private static void OnAfterComponentAdded()
        {
            try
            {
                var thisListener = PersistentStorage.GetData<ScriptableEventListener>(WildcardListenerKey);
                var componentType = PersistentStorage.GetData<TypeReference>(ListenerTypeKey).Type;
                var @event = PersistentStorage.GetData<Object>(EventKey) as BaseEvent;

                var component = thisListener.gameObject.GetComponent(componentType) as BaseScriptableEventListener;

                ChangeHiddenComponent(thisListener, component, @event);
            }
            finally
            {
                PersistentStorage.DeleteData(WildcardListenerKey);
                PersistentStorage.DeleteData(ListenerTypeKey);
                PersistentStorage.DeleteData(EventKey);
            }
        }

        private static void ChangeHiddenComponent(ScriptableEventListener listener, BaseScriptableEventListener component, BaseEvent @event)
        {
            // The flags are set persistently only through serializedObject, we do it when an editor is created.
            // However, if we don't set the flags here, the component will appear for a frame and we don't want that.
            component.hideFlags = HideFlags.HideInInspector;

            Undo.RecordObject(listener, "Change component");
            listener._component = component;

            Undo.RecordObject(component, "Change event");
            component.Event = @event;

            PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            PrefabUtility.RecordPrefabInstancePropertyModifications(listener);
        }

        private void RemoveComponent(Component component)
        {
            if (component == null)
                return;

            Undo.DestroyObjectImmediate(component);
        }

        private void DrawUnityEvent()
        {
            if (_componentEditor == null)
                return;

            _componentEditor.OnInspectorGUI();
        }

        private Object GetCurrentEvent()
        {
            var component = _target._component;
            return component == null ? null : component.Event;
        }

        private Component AddComponent(Type genericTypeDefinition, Type[] genericArgs, out bool reloadRequired)
        {
            if (genericArgs == null)
            {
                reloadRequired = false;
                return Undo.AddComponent(_target.gameObject, genericTypeDefinition);
            }

            return GenericBehaviourCreator.AddComponent(null, _target.gameObject, genericTypeDefinition, genericArgs, out reloadRequired);
        }

        private (Type GenericTypeDefinition, Type[] GenericArgs) GetListenerComponentType(Object newEvent)
        {
            if (newEvent == null)
                return (null, null);

            Type definition = null;
            Type[] genericArgs = null;

            var concreteEventType = newEvent.GetType();

            if (!concreteEventType.BaseType.IsGenericType)
            {
                if (concreteEventType == typeof(ScriptableEvent))
                {
                    definition = typeof(VoidScriptableEventListener);
                    return (definition, genericArgs);
                }

                Debug.LogError($"The type is not a known type: {concreteEventType}");
                return (definition, genericArgs);
            }

            var eventType = concreteEventType.BaseType.IsGenericType ? concreteEventType.BaseType : concreteEventType;
            var eventTypeDefinition = eventType.GetGenericTypeDefinition();

            if (eventTypeDefinition == typeof(Constant<>))
            {
                Debug.LogWarning("Constants don't change, so the event listener will never receive an event from them.");
            }
            else if (eventTypeDefinition == typeof(VariableWithHistory<>))
            {
                definition = typeof(ScriptableEventListener<,>);
                var genericArg = eventType.GenericTypeArguments[0];
                genericArgs = new [] { genericArg, genericArg };
            }
            else if (eventTypeDefinition == typeof(Variable<>) || eventTypeDefinition == typeof(ScriptableEvent<>))
            {
                definition = typeof(ScriptableEventListener<>);
                genericArgs = eventType.GenericTypeArguments;
            }
            else if (eventTypeDefinition == typeof(ScriptableEvent<,>))
            {
                definition = typeof(ScriptableEventListener<,>);
                genericArgs = eventType.GenericTypeArguments;
            }
            else if (eventTypeDefinition == typeof(ScriptableEvent<,,>))
            {
                definition = typeof(ScriptableEventListener<,,>);
                genericArgs = eventType.GenericTypeArguments;
            }

            return (definition, genericArgs);
        }
    }
}