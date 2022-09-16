namespace GenericScriptableArchitecture
{
    using JetBrains.Annotations;
    using SolidUtilities;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class BaseValue : BaseEvent
    {
#if UNITY_EDITOR
        [SerializeField, ResizableTextArea, UsedImplicitly] internal string _description;

        private bool _subscribedToPlayMode;
#endif

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            // If domain reload is disabled, OnEnable will not be called on Play Mode start, so we need to plug into
            // the 'play mode state changed' event.
            if (EditorSettings.enterPlayModeOptionsEnabled
                && EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload))
            {
                _subscribedToPlayMode = true;
                EditorApplication.playModeStateChanged += InitializeValues;
            }
#endif

            // DeepCopy() is not very performant, so execute it only in Play Mode.
            if (ApplicationUtil.InPlayMode)
                InitializeValues();
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            if ( ! _subscribedToPlayMode)
                return;

            _subscribedToPlayMode = false;
            EditorApplication.playModeStateChanged -= InitializeValues;
#endif
        }

#if UNITY_EDITOR
        private void InitializeValues(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
                InitializeValues();
        }
#endif

        protected abstract void InitializeValues();

        protected static T SerializedCopyInEditor<T>(T originalValue)
        {
#if UNITY_EDITOR
            return SerializationHelper.CreateCopy(originalValue);
#else
            return originalValue;
#endif
        }
    }
}
