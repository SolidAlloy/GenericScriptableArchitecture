namespace GenericScriptableArchitecture
{
    using UnityEngine;

    [ExecuteAlways] // Needed so that if the component is disabled in edit mode, it also disables the hidden component.
    public class ScriptableEventListener : MonoBehaviour
    {
        [SerializeField] internal BaseScriptableEventListener _component;

        private void OnEnable()
        {
            if (_component != null)
                _component.enabled = true;
        }

        private void OnDisable()
        {
            if (_component != null)
                _component.enabled = false;
        }
    }
}