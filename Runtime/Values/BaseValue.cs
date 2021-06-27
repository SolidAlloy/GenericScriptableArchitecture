namespace GenericScriptableArchitecture
{
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class BaseValue : GenericScriptableObject
    {
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        private bool _subscribedToPlayMode;

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

        private void OnDisable()
        {
            if ( ! _subscribedToPlayMode)
                return;

            _subscribedToPlayMode = false;
            EditorApplication.playModeStateChanged -= InitializeValues;
        }

        private void InitializeValues(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
                InitializeValues();
        }

        protected abstract void InitializeValues();
    }
}