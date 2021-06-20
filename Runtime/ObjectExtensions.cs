namespace GenericScriptableArchitecture
{
    using SolidUtilities.UnityEngineInternals;

    internal static class ObjectExtensions
    {
        public static T DeepCopyInEditor<T>(this T value)
        {
#if UNITY_EDITOR
            return value.DeepCopy();
#else
            return value;
#endif
        }
    }
}