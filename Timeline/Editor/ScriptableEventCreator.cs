namespace GenericScriptableArchitecture.Timeline.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GenericUnityObjects;
    using GenericUnityObjects.Editor.ScriptableObjects;
    using GenericUnityObjects.Editor.Util;
    using GenericUnityObjects.UnityEditorInternals;
    using GenericUnityObjects.Util;
    using TypeReferences;
    using UnityEditor;
    using UnityEditor.Timeline.Actions;
    using UnityEngine;
    using UnityEngine.Timeline;
    using Object = UnityEngine.Object;

    public abstract class ScriptableEventCreator : TimelineAction
    {
        private const string TrackAssetsKey = "TrackAssets";
        private const string TimeKey = "TimelineTime";
        private const string EmitterTypeKey = "EmitterType";
        private const string EventTypeKey = "EventType";
        private const string FindEventKey = "FindEvent";

        private static readonly Action<ICollection<TrackAsset>, Type, double, Object> _addMarkers;

        public override ActionValidity Validate(ActionContext context)
        {
            return ActionValidity.Valid;
        }

        static ScriptableEventCreator()
        {
            var timelineContextMenuType = typeof(MenuEntryAttribute).Assembly.GetType("UnityEditor.Timeline.SequencerContextMenu");
            var addMarkersMethod = timelineContextMenuType.GetMethod("AddMarkersCallback", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(ICollection<TrackAsset>), typeof(Type), typeof(double), typeof(Object) }, null);
            _addMarkers = (Action<ICollection<TrackAsset>, Type, double, Object>) Delegate.CreateDelegate(typeof(Action<ICollection<TrackAsset>, Type, double, Object>), addMarkersMethod);
        }

        protected static void AddMarkers(ActionContext context, Type type, Object obj)
        {
            if (context.invocationTime == null)
                return;

            _addMarkers(GetTracks(context), type, context.invocationTime.Value, obj);
        }

        protected static bool CreateGenericEmitter(Type eventTypeWithoutArgs, Type emitterTypeWithoutArgs, ActionContext context, bool findEvent)
        {
            if (context.invocationTime == null)
                return false;

            TypeSelectionWindow.Create(eventTypeWithoutArgs, genericArgs =>
            {
                Type eventEmitterType = emitterTypeWithoutArgs.MakeGenericType(genericArgs);

                if (ScriptableObjectsDatabase.TryGetConcreteType(eventEmitterType, out Type concreteType))
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    CreateMarker(GetTracks(context), context.invocationTime.Value, concreteType, eventTypeWithoutArgs, genericArgs, findEvent);
                }
                else
                {
                    PersistentStorage.SaveData(EmitterTypeKey, new TypeReference(eventEmitterType));
                    PersistentStorage.SaveData(TrackAssetsKey, GetTracks(context));
                    // ReSharper disable once PossibleInvalidOperationException
                    PersistentStorage.SaveData(TimeKey, context.invocationTime.Value);
                    PersistentStorage.SaveData(EventTypeKey, new TypeReference(eventTypeWithoutArgs));
                    PersistentStorage.SaveData(FindEventKey, findEvent);
                    PersistentStorage.ExecuteOnScriptsReload(FinishEmitterCreation);

                    ConcreteClassCreator<ScriptableObject>.CreateConcreteClass(emitterTypeWithoutArgs, genericArgs);
                    AssetDatabase.Refresh();
                }
            });

            return true;
        }

        private static void CreateMarker(List<TrackAsset> tracks, double time, Type concreteEmitterType, Type eventTypeWithoutArgs, Type[] genericArgs, bool findEvent)
        {
            if (!findEvent)
            {
                CreateGenericEmitter(tracks, concreteEmitterType, time, null);
                return;
            }

            Type eventType = eventTypeWithoutArgs.MakeGenericType(genericArgs);
            ObjectSelectorHelper.ShowGenericSelector(null, null, eventType, true, "Scriptable Event",
                obj =>
                {
                    if (obj != null)
                    {
                        CreateGenericEmitter(tracks, concreteEmitterType, time, obj);
                    }
                });
        }

        private static void CreateGenericEmitter(List<TrackAsset> tracks, Type concreteEmitterType, double time, Object obj)
        {
            _addMarkers(tracks, concreteEmitterType, time, obj);

            var createdMarker = Array.Find(Selection.objects, obj => obj.GetType() == concreteEmitterType);

            if (createdMarker != null)
                createdMarker.name = GetNiceEmitterName(concreteEmitterType.BaseType);
        }

        private static void FinishEmitterCreation()
        {
            try
            {
                Type eventEmitterType = PersistentStorage.GetData<TypeReference>(EmitterTypeKey).Type;
                var tracks = PersistentStorage.GetData<List<TrackAsset>>(TrackAssetsKey);
                var time = PersistentStorage.GetData<double>(TimeKey);
                var eventType = PersistentStorage.GetData<TypeReference>(EventTypeKey).Type;
                var findEvent = PersistentStorage.GetData<bool>(FindEventKey);

                var concreteType = ScriptableObjectsDatabase.GetConcreteType(eventEmitterType);

                CreateMarker(tracks, time, concreteType, eventType, concreteType.BaseType.GenericTypeArguments, findEvent);
            }
            finally
            {
                PersistentStorage.DeleteData(EmitterTypeKey);
                PersistentStorage.DeleteData(TrackAssetsKey);
                PersistentStorage.DeleteData(TimeKey);
                PersistentStorage.DeleteData(EventTypeKey);
                PersistentStorage.DeleteData(FindEventKey);
            }
        }

        private static List<TrackAsset> GetTracks(ActionContext context) => context.tracks.Concat(context.clips.Select(clip => clip.
#if TIMELINE_1_5_OR_NEWER
            GetParentTrack()
#else
            parentTrack
#endif
        )).Distinct().ToList();

        private static string GetNiceEmitterName(Type eventEmitterType)
        {
            return TypeUtility.GetNiceNameOfGenericType(eventEmitterType)
                .Replace('<', ' ')
                .Replace(',', ' ')
                .Replace(">", string.Empty);
        }
    }
}