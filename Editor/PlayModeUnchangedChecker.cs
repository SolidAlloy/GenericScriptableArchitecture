namespace ExtendedScriptableObjects.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using SolidUtilities.Editor.Extensions;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    [InitializeOnLoad]
    internal static class PlayModeUnchangedChecker
    {
        private static readonly HashSet<string> ExcludeProps = new HashSet<string> { nameof(PlayModeUnchangedScriptableObject.IsSecondObject) };

        private static string[] _assetPaths;
        private static int _assetPathsLength;

        static PlayModeUnchangedChecker() => EditorApplication.playModeStateChanged += OnModeChange;

        private static void OnModeChange(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                OnEnteredPlayMode();
            }
            else if (stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                OnExitingPlayMode();
            }
        }

        private static void OnEnteredPlayMode()
        {
            _assetPaths = AssetDatabase.FindAssets($"t:{nameof(PlayModeUnchangedScriptableObject)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            _assetPathsLength = _assetPaths.Length;

            for (int i = 0; i < _assetPathsLength; ++i)
            {
                var asset = AssetDatabase.LoadAssetAtPath<PlayModeUnchangedScriptableObject>(_assetPaths[i]);
                var secondObject = Object.Instantiate(asset);
                secondObject.IsSecondObject = true;
                AssetDatabase.AddObjectToAsset(secondObject, asset);
            }
        }

        private static void OnExitingPlayMode()
        {
            Assert.IsNotNull(_assetPaths);

            for (int i = 0; i < _assetPathsLength; ++i)
            {
                var objects = AssetDatabase.LoadAllAssetsAtPath(_assetPaths[i]);

                PlayModeUnchangedScriptableObject firstObject = null;
                PlayModeUnchangedScriptableObject secondObject = null;

                Assert.IsTrue(objects.Length == 2);

                foreach (Object obj in objects)
                {
                    var typedObj = (PlayModeUnchangedScriptableObject) obj;

                    if (typedObj.IsSecondObject)
                    {
                        secondObject = typedObj;
                    }
                    else
                    {
                        firstObject = typedObj;
                    }
                }

                Assert.IsNotNull(firstObject);
                Assert.IsNotNull(secondObject);

                firstObject.CopyValuesFrom(secondObject, ExcludeProps);

                AssetDatabase.RemoveObjectFromAsset(secondObject);
            }
        }
    }
}