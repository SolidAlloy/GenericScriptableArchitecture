namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor.SettingsManagement;

    internal static class StackTraceSettings
    {
        private const string PackageName = "com.solidalloy.generic-scriptable-architecture";

        private static Settings _instance;

        private static UserSetting<bool> _enabledInProject;

        public static bool EnabledInProject
        {
            get
            {
                InitializeIfNeeded();
                return _enabledInProject.value;
            }

            set => _enabledInProject.value = value;
        }

        private static void InitializeIfNeeded()
        {
            if (_instance != null)
                return;

            _instance = new Settings(PackageName);

            _enabledInProject = new UserSetting<bool>(_instance, nameof(_enabledInProject), false);
        }
    }
}