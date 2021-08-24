namespace GenericScriptableArchitecture.Timeline.Editor
{
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor.Timeline.Actions;

    [MenuEntry("Scriptable Events/Add Event Emitter", 13001)]
    public class VoidEventCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context)
        {
            if (context.invocationTime == null)
                return false;

            AddMarkers(context, typeof(ScriptableEventEmitter), null);
            return true;
        }
    }

    [MenuEntry("Scriptable Events/Add Emitter From Event", 13002)]
    public class VoidEventFromCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context)
        {
            ObjectSelectorHelper.ShowGenericSelector(null, null, typeof(ScriptableEvent), true, "Scriptable Event",
                obj =>
                {
                    if (obj != null)
                        AddMarkers(context, typeof(ScriptableEventEmitter), obj);
                });

            return true;
        }
    }
}