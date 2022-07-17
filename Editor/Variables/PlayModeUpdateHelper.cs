namespace GenericScriptableArchitecture.Editor
{
    using System;
    using UnityEditor;

    public class PlayModeUpdateHelper : IDisposable
    {
        private const int UpdateFrameCount = 20;
        private int _frameCounter;
        private readonly Action _onUpdate;
        private readonly Editor _editor;

        public PlayModeUpdateHelper(Editor editor, Action onUpdate)
        {
            _editor = editor;
            _onUpdate = onUpdate;
            EditorApplication.update += OnUpdate;
        }

        private void OnUpdate()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode || _editor.targets.Length == 0 || _editor.target == null || _frameCounter++ < UpdateFrameCount)
                return;

            _frameCounter = 0;
            _onUpdate();
        }

        public void Dispose()
        {
            EditorApplication.update -= OnUpdate;
        }
    }
}