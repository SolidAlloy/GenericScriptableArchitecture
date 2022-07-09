namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.Editor.Util;
    using GenericUnityObjects.UnityEditorInternals;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using TypeReferences;
    using UnityEditor;
    using UnityEngine;
    using EditorGUIHelper = GenericUnityObjects.UnityEditorInternals.EditorGUIHelper;

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
    public class VariableInstancerDrawer : CreatableObjectDrawer
    {
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
                SetNewReference(property, newReference);
            }

            EditorGUI.EndProperty();
        }

        private static bool ValidateObjectFieldAssignment(SerializedProperty property, Object newObject)
        {
            var propertyType = property.GetObjectType();
            var objectType = newObject.GetType();

            // Simple case: the dragged object inherits from the expected VariableInstancer type.
            if (objectType.InheritsFrom(propertyType))
                return true;

            // The variable can only be dragged onto an object field that is empty.
            if (property.objectReferenceValue != null)
                return false;

            var genericArgument = propertyType.GetGenericArguments();
            var variableType = typeof(Variable<>).MakeGenericType(genericArgument);
            return objectType.InheritsFrom(variableType);
        }

        private const string ThisComponentKey = "VariableInstancerDrawer_ThisComponent";
        private const string AddedComponentTypeKey = "VariableInstancerDrawer_AddedComponentType";
        private const string VariableKey = "VariableInstancerDrawer_Variable";

        private static void SetNewReference(SerializedProperty property, Object newReference)
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
                    AssignVariable(addedComponent, newReference);
                    return;
                }

                PersistentStorage.SaveData(ThisComponentKey, property.serializedObject.targetObject);
                PersistentStorage.SaveData(AddedComponentTypeKey, new TypeReference(propertyType));
                PersistentStorage.SaveData(VariableKey, newReference);
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
                var addedComponent = ((MonoBehaviour)thisComponent).gameObject.GetComponent(addedComponentType);
                AssignVariable(addedComponent, variable);
            }
            finally
            {
                PersistentStorage.DeleteData(ThisComponentKey);
                PersistentStorage.DeleteData(AddedComponentTypeKey);
                PersistentStorage.DeleteData(VariableKey);
            }
        }

        private static void AssignVariable(Component instancerComponent, Object variable)
        {
            var serializedObject = new SerializedObject(instancerComponent);
            serializedObject.FindProperty(nameof(VariableInstancer<int>._variableReference)).objectReferenceValue = variable;
            serializedObject.ApplyModifiedProperties();
        }
    }
}