namespace GenericScriptableArchitecture
{
    using UnityEditor;

    internal static class ApplicationUtil
    {
        public static bool InPlayMode
        {
            get
            {
#if UNITY_EDITOR
                return EditorApplication.isPlayingOrWillChangePlaymode;
#else
                return true;
#endif
            }
        }

        public static bool InEditMode => ! InPlayMode;
    }
}