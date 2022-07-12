namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.Editor;
    using UnityEditor;

    public abstract class PlayModeUpdatableEditor : Editor
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

        protected override void OnHeaderGUI()
        {
            GenericHeaderUtility.OnHeaderGUI(this);
        }

        private void OnUpdate()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode || target == null || _frameCounter++ < UpdateFrameCount)
                return;

            _frameCounter = 0;
            Update();
        }

        protected virtual void Update() { }
    }
}