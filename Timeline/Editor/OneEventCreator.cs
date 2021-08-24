namespace GenericScriptableArchitecture.Timeline.Editor
{
    using UnityEditor.Timeline.Actions;

    [MenuEntry("Scriptable Events/Add Event Emitter<T>", 13003)]
    public class OneEventCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context) => CreateGenericEmitter(typeof(ScriptableEvent<>), typeof(ScriptableEventEmitter<>), context, false);
    }

    [MenuEntry("Scriptable Events/Add Emitter From Event<T>", 13004)]
    public class OneEventFromCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context) => CreateGenericEmitter(typeof(ScriptableEvent<>), typeof(ScriptableEventEmitter<>), context, true);
    }
}