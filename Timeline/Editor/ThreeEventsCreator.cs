namespace GenericScriptableArchitecture.Timeline.Editor
{
    using UnityEditor.Timeline.Actions;

    [MenuEntry("Scriptable Events/Add Event Emitter<T1, T2, T3>", 13007)]
    public class ThreeEventsCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context) => CreateGenericEmitter(typeof(ScriptableEvent<,,>), typeof(ScriptableEventEmitter<,,>), context, false);
    }

    [MenuEntry("Scriptable Events/Add Emitter From Event<T1, T2, T3>", 13008)]
    public class ThreeEventsFromCreator : ScriptableEventCreator
    {
        public override bool Execute(ActionContext context) => CreateGenericEmitter(typeof(ScriptableEvent<,,>), typeof(ScriptableEventEmitter<,,>), context, true);
    }
}