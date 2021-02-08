namespace ExtendedScriptableObjects.Editor
{
    using System.Linq;
    using GenericUnityObjects.UnityEditorInternals;
    using SolidUtilities.Editor.Extensions;
    using SolidUtilities.Helpers;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(PlayModeUnchangedScriptableObject), true)]
    internal class PlayModeUnchangedEditor : GenericHeaderEditor
    {
        public bool ShowDescription = true;

        private static readonly string[] _excludeMonoScript = { "m_Script" };
        private static readonly string[] _excludeMonoScriptAndDescription = { "m_Script", "_description" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Application.isPlaying)
            {
                DrawWarning();
                DrawButtons();
            }

            DrawPropertyFields();

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawPropertyFields()
        {
            var propertiesToExclude = ShowDescription ? _excludeMonoScript : _excludeMonoScriptAndDescription;
            DrawPropertiesExcluding(serializedObject, propertiesToExclude);
        }

        private void DrawButtons()
        {
            using (new DrawHelper.HorizontalBlock(null))
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("SAVE", EditorStyles.miniButton))
                {
                    Object secondObject = GetSecondObject();
                    secondObject.CopyValuesFrom(serializedObject);
                }

                if (GUILayout.Button("RESTORE", EditorStyles.miniButton))
                {
                    Object secondObject = GetSecondObject();
                    serializedObject.CopyValuesFrom(secondObject);

                    // Unset focus from all elements because if a field has focus,
                    // its new loaded value will not be visible until the focus is set to another element.
                    GUI.FocusControl(string.Empty);
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(5);
        }

        private Object GetSecondObject()
        {
            string path = AssetDatabase.GetAssetPath(target);
            return AssetDatabase.LoadAllAssetsAtPath(path)
                .First(obj => ((PlayModeUnchangedScriptableObject) obj).IsSecondObject);
        }

        private static void DrawWarning()
        {
            EditorGUILayout.HelpBox(
                "Changes will NOT be saved automatically. They will be restored after exiting Play Mode. " +
                "Use SAVE button to save them.",
                MessageType.Warning);

            GUILayout.Space(5);
        }
    }
}
