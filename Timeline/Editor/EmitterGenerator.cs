namespace GenericScriptableArchitecture.Timeline.Editor
{
    using System;
    using GenericUnityObjects;
    using GenericUnityObjects.Editor.Util;
    using UnityEditor;

    [InitializeOnLoad]
    public static class EmitterGenerator
    {
        private static readonly Type _event1 = typeof(ScriptableEvent<>);
        private static readonly Type _event2 = typeof(ScriptableEvent<,>);
        private static readonly Type _event3 = typeof(ScriptableEvent<,,>);

        private static readonly Type _emitter1 = typeof(ScriptableEventEmitter<>);
        private static readonly Type _emitter2 = typeof(ScriptableEventEmitter<,>);
        private static readonly Type _emitter3 = typeof(ScriptableEventEmitter<,,>);

        static EmitterGenerator()
        {
            ConcreteClassCreator<GenericScriptableObject>.ConcreteClassAdded += (typeWithoutArgs, genericArgs) =>
            {
                var emitterType = GetMatchingEmitterType(typeWithoutArgs);

                if (emitterType != null)
                    ConcreteClassCreator<GenericScriptableObject>.CreateConcreteClass(emitterType, genericArgs);
            };
        }

        private static Type GetMatchingEmitterType(Type typeWithoutArgs)
        {
            if (typeWithoutArgs == _event1)
                return _emitter1;

            if (typeWithoutArgs == _event2)
                return _emitter2;

            if (typeWithoutArgs == _event3)
                return _emitter3;

            return null;
        }
    }
}