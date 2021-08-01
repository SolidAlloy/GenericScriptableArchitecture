namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using SolidUtilities.Editor.Helpers;
    using UnityEditor;
    using UnityEngine;

    public static class SettingsDrawer
    {
        private const string EnabledInProjectLabel = "Enable stack traces in all assets";

        private static readonly GUIContent _enabledInProjectContent = new GUIContent(EnabledInProjectLabel,
            "This has higher priority than the disabled stack trace option on individual assets.");

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Generic ScriptableObject Architecture", SettingsScope.Project)
            {
                guiHandler = OnGUI,
                keywords = GetKeywords()
            };
        }

        private static void OnGUI(string searchContext)
        {
            if (StackTraceSettings.EnabledInProject)
            {
                EditorGUILayoutHelper.DrawInfoMessage(
                    "Stack traces may affect performance of the game in editor. If possible, enable it on individual assets when they require debugging.");
            }

            using (EditorGUIUtilityHelper.LabelWidthBlock(220f))
            {
                StackTraceSettings.EnabledInProject = EditorGUILayout.Toggle(_enabledInProjectContent, StackTraceSettings.EnabledInProject);
            }
        }

        private static HashSet<string> GetKeywords()
        {
            var keywords = new HashSet<string>();
            keywords.AddWords(EnabledInProjectLabel);
            return keywords;
        }

        private static readonly char[] _separators = { ' ' };

        private static void AddWords(this HashSet<string> set, string phrase)
        {
            foreach (string word in phrase.Split(_separators))
            {
                set.Add(word);
            }
        }
    }
}