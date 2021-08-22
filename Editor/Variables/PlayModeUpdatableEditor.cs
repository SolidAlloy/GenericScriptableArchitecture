namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;

    internal abstract class PlayModeUpdatableEditor : GenericHeaderEditor
    {
        private const int UpdateFrameCount = 20;
        private int _frameCounter;

        protected virtual void OnEnable()
        {
            EditorApplication.update += OnUpdate;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (_frameCounter++ < UpdateFrameCount)
                return;

            _frameCounter = 0;
            Update();
        }

        protected virtual void Update() { }
    }
}