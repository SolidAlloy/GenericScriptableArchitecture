namespace GenericScriptableArchitecture.Timeline.Editor
{
    using UnityEditor.Timeline.Actions;

    [MenuEntry("Scriptable Events/Add Event Emitter<T1, T2>", 13005)]
    public class TwoEventsCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context) => CreateGenericEmitter(typeof(ScriptableEvent<,>), typeof(ScriptableEventEmitter<,>), context, false);
    }

    [MenuEntry("Scriptable Events/Add Emitter From Event<T1, T2>", 13006)]
    public class TwoEventsFromCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context) => CreateGenericEmitter(typeof(ScriptableEvent<,>), typeof(ScriptableEventEmitter<,>), context, true);
    }
}