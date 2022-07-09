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

        private ScriptableEventListener _target;
        private ScriptableEventListenerEditor _componentEditor;
        private BaseScriptableEventListener _component;
        private GeneratedComponentsDebugDrawer _debugDrawer;

        private void OnEnable()
        {
            _target = (ScriptableEventListener) target;

            if (_target != null)
            {
                var componentProperty = serializedObject.FindProperty(nameof(ScriptableEventListener._component));
                _debugDrawer = new GeneratedComponentsDebugDrawer(componentProperty, _target.gameObject,
                    () => _target.GetComponents<ScriptableEventListener>(),
                    () => _target.GetComponents<BaseScriptableEventListener>());
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

            _debugDrawer.DrawComponentProperty();
            CreateComponentEditorIfNeeded();
            DrawObjectField();
            DrawUnityEvent();
            _debugDrawer.DrawDebugFlag();
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

            if (!_debugDrawer.ShowGeneratedComponents)
                _componentEditor.serializedObject.SetHideFlagsPersistently(HideFlags.HideInInspector);

            // It is necessary to change DrawObjectField after HideFlags because serializedObject.ApplyModifiedProperties() inside SetHideFlags discards the changed value of DrawObjectField
            _component.DrawObjectField = _debugDrawer.ShowGeneratedComponents;
        }

        private void DrawObjectField()
        {
            var oldEvent = GetCurrentEvent();

            if (_target._component != null)
                Undo.RecordObject(_target._component, "Changed event field");

            var newEvent = GenericObjectDrawer.ObjectField("Event", oldEvent, typeof(IBaseEvent), true);

            if (newEvent != oldEvent)
                ChangeEvent(newEvent);
        }

        private void ChangeEvent(Object newEvent)
        {
            RemoveComponent(_target._component);

            var listenerType = GetListenerComponentType(newEvent);

            if (listenerType == null)
                return;

            var component = AddComponentHelper.AddComponent(_target.gameObject, listenerType, out bool reloadRequired) as BaseScriptableEventListener;

            if (reloadRequired)
            {
                PersistentStorage.SaveData(WildcardListenerKey, _target);
                PersistentStorage.SaveData(ListenerTypeKey, new TypeReference(listenerType));
                PersistentStorage.SaveData(EventKey, newEvent);
                PersistentStorage.ExecuteOnScriptsReload(OnAfterComponentAdded);
                AssetDatabase.Refresh();
            }
            else
            {
                ChangeHiddenComponent(_target, component, (IBaseEvent) newEvent);
            }
        }

        private static void OnAfterComponentAdded()
        {
            try
            {
                var thisListener = PersistentStorage.GetData<ScriptableEventListener>(WildcardListenerKey);
                var componentType = PersistentStorage.GetData<TypeReference>(ListenerTypeKey).Type;
                var @event = (IBaseEvent) PersistentStorage.GetData<Object>(EventKey);

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

        private static void ChangeHiddenComponent(ScriptableEventListener listener, BaseScriptableEventListener component, IBaseEvent @event)
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
            return component == null ? null : (Object) component.Event;
        }

        private Type GetListenerComponentType(Object newEvent)
        {
            if (newEvent == null)
                return null;

            var concreteEventType = newEvent.GetType();

            var eventType = concreteEventType.BaseType;

            if (!eventType.IsGenericType)
            {
                if (concreteEventType == typeof(ScriptableEvent))
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

            if (eventTypeDefinition == typeof(Variable<>) || eventTypeDefinition == typeof(ScriptableEvent<>) || eventTypeDefinition == typeof(VariableInstancer<>))
            {
                return typeof(ScriptableEventListener<>).MakeGenericType(eventType.GenericTypeArguments);
            }

            if (eventTypeDefinition == typeof(ScriptableEvent<,>))
            {
                return typeof(ScriptableEventListener<,>).MakeGenericType(eventType.GenericTypeArguments);
            }

            if (eventTypeDefinition == typeof(VariableWithHistory<>) || eventTypeDefinition == typeof(VariableInstancerWithHistory<>))
            {
                var genericArg = eventType.GenericTypeArguments[0];
                return typeof(ScriptableEventListener<,>).MakeGenericType(genericArg, genericArg);
            }

            if (eventTypeDefinition == typeof(ScriptableEvent<,,>))
            {
                return typeof(ScriptableEventListener<,,>).MakeGenericType(eventType.GenericTypeArguments);
            }

            return null;
        }
    }
}