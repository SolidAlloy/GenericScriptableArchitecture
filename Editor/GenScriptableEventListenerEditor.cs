namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Reflection;
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.Editor.MonoBehaviours;
    using GenericUnityObjects.Editor.Util;
    using TypeReferences;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(GenScriptableEventListener))]
    public class GenScriptableEventListenerEditor : Editor // TODO: change temp name
    {
        private const string WildcardListenerKey = "ListenerEditor_WildcardListener";
        private const string ListenerTypeKey = "ListenerEditor_ListenerType";
        private const string EventKey = "ListenerEditor_EventReference";

        private GenScriptableEventListener _target;
        private ScriptableEventListenerEditor _componentEditor;

        private void OnEnable()
        {
            _target = target as GenScriptableEventListener;
        }

        public override void OnInspectorGUI()
        {
            DrawObjectField();
            DrawUnityEvent();
        }

        private void DrawObjectField()
        {
            var oldEvent = GetCurrentEvent();
            var newEvent = GenericObjectDrawer.ObjectField("Event", oldEvent, typeof(BaseEvent), true);

            if (newEvent != oldEvent)
                ChangeEvent(newEvent);
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
                _target._component = component;
                component.Event = newEvent as BaseEvent;
            }
        }

        private static void OnAfterComponentAdded()
        {
            try
            {
                var thisListener = PersistentStorage.GetData<GenScriptableEventListener>(WildcardListenerKey);
                var componentType = PersistentStorage.GetData<TypeReference>(ListenerTypeKey).Type;
                var @event = PersistentStorage.GetData<Object>(EventKey) as BaseEvent;

                var component = thisListener.gameObject.GetComponent(componentType) as BaseScriptableEventListener;

                thisListener._component = component;
                component.Event = @event;
            }
            finally
            {
                PersistentStorage.DeleteData(WildcardListenerKey);
                PersistentStorage.DeleteData(ListenerTypeKey);
                PersistentStorage.DeleteData(EventKey);
            }
        }

        private void RemoveComponent(Component component)
        {
            if (component == null)
                return;

            if (Application.isPlaying)
            {
                Destroy(component);
            }
            else
            {
                DestroyImmediate(component);
            }
        }

        private void DrawUnityEvent()
        {
            if (_target._component != null && _componentEditor == null)
            {
                _componentEditor = (ScriptableEventListenerEditor) CreateEditor(_target._component, typeof(ScriptableEventListenerEditor));
            }

            if (_componentEditor == null)
                return;

            // TODO: pass info to EventHolderDrawer that the object field shouldn't be drawn.
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
                return _target.gameObject.AddComponent(genericTypeDefinition);
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
                    definition = typeof(ScriptableEventListener);
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