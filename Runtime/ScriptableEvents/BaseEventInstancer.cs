namespace GenericScriptableArchitecture
{
    using UnityEngine;

    // Such an execution order allows the instance to be awaken before all the default classes including those predefined in execution order by Unity (like EventSystem with -1000 order).
    // It's ok that instancers are awaken even before EventSystem because they don't depend on any of those classes.
    [DefaultExecutionOrder(-2000)]
    public abstract class BaseEventInstancer : MonoBehaviour
    {
        internal abstract ScriptableEventHelper ScriptableEventHelper { get; }

        internal abstract BaseScriptableEvent Base { get; }
    }
}