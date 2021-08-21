namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.Editor;
    using UnityEditor;

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
}