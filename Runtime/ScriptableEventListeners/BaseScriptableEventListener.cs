namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class BaseScriptableEventListener : MonoBehaviour
    {
        [SerializeField] internal StackTraceProvider _stackTrace;

        internal abstract IBaseEvent Event { get; set; }

        internal abstract bool DrawObjectField { get; set; }

        protected bool CanBeInvoked()
        {
#if UNITY_EDITOR
            if ( ! EditorApplication.isPlaying)
            {
                Debug.LogError($"Tried to listen with the {name} listener in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }
    }
}