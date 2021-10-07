namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class SymbolManager
    {
        private const string TimelineDefine = "GSA_TIMELINE";

        static SymbolManager()
        {
#if USE_TIMELINE
            if (!IsSymbolDefined(TimelineDefine))
                SetCompileDefine(TimelineDefine, true);
#else
            if (IsSymbolDefined(TimelineDefine))
                SetCompileDefine(TimelineDefine, false);
#endif
        }

        private static bool IsSymbolDefined(string define)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Split(';')
                .Any(defineSymbol => defineSymbol == define);
        }

        private static void SetCompileDefine(string define, bool enabled)
        {
            foreach (BuildTargetGroup targetGroup in GetAllBuildTargetGroups())
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();

                bool alreadyExists = false;

                for (int i = 0; i < defines.Count; i++)
                {
                    if (!string.Equals(define, defines[i], StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    alreadyExists = true;
                    if (!enabled)
                    {
                        defines.RemoveAt(i);
                    }
                }

                if (!alreadyExists && enabled)
                {
                    defines.Add(define);
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines.ToArray()));
            }
        }

        private static IEnumerable<BuildTargetGroup> GetAllBuildTargetGroups()
        {
            Type enumType = typeof(BuildTargetGroup);
            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);

            for (var i = 0; i < names.Length; i++)
            {
                string name = names[i];
                BuildTargetGroup value = (BuildTargetGroup)values.GetValue(i);

                if (value == BuildTargetGroup.Unknown) continue;

                MemberInfo[] member = enumType.GetMember(name);
                MemberInfo entry = member.FirstOrDefault(p => p.DeclaringType == enumType);

                if (entry == null)
                {
                    Debug.LogError($"Unhandled build target: {name}.");
                    continue;
                }

                if (entry.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length != 0)
                {
                    continue;
                }

                yield return value;
            }
        }
    }
}