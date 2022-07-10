namespace GenericScriptableArchitecture.Editor
{
    using System;
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.Editor.Util;
    using GenericUnityObjects.UnityEditorInternals;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using TypeReferences;
    using UnityEditor;
    using UnityEngine;
    using EditorGUIHelper = GenericUnityObjects.UnityEditorInternals.EditorGUIHelper;
    using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
#endif

    [CustomPropertyDrawer(typeof(BaseValue), true)]
    [CustomPropertyDrawer(typeof(BaseScriptableEvent), true)]
    [CustomPropertyDrawer(typeof(BaseRuntimeSet), true)]
#if ODIN_INSPECTOR
    [DrawerPriority(0, 0, 4)]
#endif
    public class ScriptableArchitectureObjectDrawer : CreatableObjectDrawer { }

    [CustomPropertyDrawer(typeof(BaseVariableInstancer), true)]
#if ODIN_INSPECTOR
    [DrawerPriority(0, 0, 4)]
#endif
    public class VariableInstancerDrawer : InstancerDrawer
    {
        protected override string BaseFieldName => nameof(VariableInstancer<int>._variableReference);

        protected override bool IsScriptableAssetTypeValid(Type propertyType, Type objectType)
        {
            var genericArgument = propertyType.GetGenericArguments();
            var variableType = typeof(Variable<>).MakeGenericType(genericArgument);
            return objectType.InheritsFrom(variableType);
        }
    }

    [CustomPropertyDrawer(typeof(BaseEventInstancer), true)]
#if ODIN_INSPECTOR
    [DrawerPriority(0, 0, 4)]
#endif
    public class EventInstancerDrawer : InstancerDrawer
    {
        protected override string BaseFieldName => nameof(EventInstancer._base);

        protected override bool IsScriptableAssetTypeValid(Type propertyType, Type objectType)
        {
            if (!propertyType.IsGenericType)
            {
                return objectType.InheritsFrom(typeof(ScriptableEvent));
            }

            var genericArguments = propertyType.GetGenericArguments();

            Type typeDefinition = genericArguments.Length switch
            {
                1 => typeof(ScriptableEvent<>),
                2 => typeof(ScriptableEvent<,>),
                3 => typeof(ScriptableEvent<,,>),
                _ => throw new ArgumentOutOfRangeException()
            };

            var variableType = typeDefinition.MakeGenericType(genericArguments);
            return objectType.InheritsFrom(variableType);
        }
    }

    public abstract class InstancerDrawer : CreatableObjectDrawer
    {
        protected abstract string BaseFieldName { get; }

        protected abstract bool IsScriptableAssetTypeValid(Type propertyType, Type objectType);

        protected override void DrawObjectField(Rect objectRect, SerializedProperty property, GUIContent label)
        {
            GUIContent propertyLabel = EditorGUI.BeginProperty(objectRect, label, property);

            Object objectBeingEdited = property.serializedObject.targetObject;

            // Allow scene objects if the object being edited is NOT persistent
            bool allowSceneObjects = ! (objectBeingEdited == null || EditorUtility.IsPersistent(objectBeingEdited));

            var newReference = EditorGUIHelper.GenericObjectField(objectRect, label == null ? null : propertyLabel, property.objectReferenceValue,
                allowSceneObjects, new ObjectFieldHelper(property, newObject => ValidateObjectFieldAssignment(property, newObject)));

            if (property.objectReferenceValue != newReference)
            {
                SetNewReference(property, newReference, BaseFieldName);
            }

            EditorGUI.EndProperty();
        }

        private bool ValidateObjectFieldAssignment(SerializedProperty property, Object newObject)
        {
            var propertyType = property.GetObjectType();
            var objectType = newObject.GetType();

            // Simple case: the dragged object inherits from the expected VariableInstancer type.
            if (objectType.InheritsFrom(propertyType))
                return true;

            // The variable can only be dragged onto an object field that is empty.
            if (property.objectReferenceValue != null)
                return false;

            return IsScriptableAssetTypeValid(propertyType, objectType);
        }

        private const string ThisComponentKey = "VariableInstancerDrawer_ThisComponent";
        private const string AddedComponentTypeKey = "VariableInstancerDrawer_AddedComponentType";
        private const string VariableKey = "VariableInstancerDrawer_Variable";
        private const string FieldNameKey = "VariableInstancerDrawer_FieldNameKey";

        private static void SetNewReference(SerializedProperty property, Object newReference, string baseFieldName)
        {
            var propertyType = property.GetObjectType();
            var objectType = newReference.GetType();

            // Simple case: the dragged object inherits from the expected VariableInstancer type.
            if (objectType.InheritsFrom(propertyType))
            {
                property.objectReferenceValue = newReference;
                return;
            }

            // The variable can only be dragged onto an object field that is empty.
            if (property.objectReferenceValue != null)
                return;

            CreateMonoBehaviour(property, propertyType, (addedComponent, reloadRequired) =>
            {
                if (!reloadRequired)
                {
                    AssignVariable(addedComponent, newReference, baseFieldName);
                    return;
                }

                PersistentStorage.SaveData(ThisComponentKey, property.serializedObject.targetObject);
                PersistentStorage.SaveData(AddedComponentTypeKey, new TypeReference(propertyType));
                PersistentStorage.SaveData(VariableKey, newReference);
                PersistentStorage.SaveData(FieldNameKey, baseFieldName);
                PersistentStorage.ExecuteOnScriptsReload(OnAfterComponentAdded);
            });
        }

        private static void OnAfterComponentAdded()
        {
            try
            {
                var thisComponent = PersistentStorage.GetData<Object>(ThisComponentKey);
                var addedComponentType = PersistentStorage.GetData<TypeReference>(AddedComponentTypeKey).Type;
                var variable = PersistentStorage.GetData<Object>(VariableKey);
                string fieldName = PersistentStorage.GetData<string>(FieldNameKey);
                var addedComponent = ((MonoBehaviour)thisComponent).gameObject.GetComponent(addedComponentType);
                AssignVariable(addedComponent, variable, fieldName);
            }
            finally
            {
                PersistentStorage.DeleteData(ThisComponentKey);
                PersistentStorage.DeleteData(AddedComponentTypeKey);
                PersistentStorage.DeleteData(VariableKey);
                PersistentStorage.DeleteData(FieldNameKey);
            }
        }

        private static void AssignVariable(Component instancerComponent, Object variable, string baseFieldName)
        {
            var serializedObject = new SerializedObject(instancerComponent);
            serializedObject.FindProperty(baseFieldName).objectReferenceValue = variable;
            serializedObject.ApplyModifiedProperties();
        }
    }
}