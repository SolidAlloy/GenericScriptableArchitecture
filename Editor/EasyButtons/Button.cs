namespace EasyButtons.Editor
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;
    using UnityEditor;
    using Utils;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.Assertions;

    /// <summary>
    /// A class that holds information about a button and can draw it in the inspector.
    /// </summary>
    public abstract class Button
    {
        /// <summary> Display name of the button. </summary>
        [PublicAPI] public readonly string DisplayName;

        /// <summary> MethodInfo object the button is attached to. </summary>
        [PublicAPI] public readonly MethodInfo Method;

        protected Button(MethodInfo method)
        {
            DisplayName = ObjectNames.NicifyVariableName(method.Name);
            Method = method;
        }

        public void Draw(IEnumerable<object> targets)
        {
            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
            {
                DrawInternal(targets);
            }
        }

        public static Button Create(MethodInfo method, Func<string>[] getCustomArgNames)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == 0)
                return new ButtonWithoutParams(method);

            if (getCustomArgNames != null)
                Assert.AreEqual(parameters.Length, getCustomArgNames.Length);

            var paramDatas = new (ParameterInfo, Func<string>)[parameters.Length];

            for (int i = 0; i < paramDatas.Length; i++)
            {
                paramDatas[i] = (parameters[i], getCustomArgNames?[i]);
            }

            return new ButtonWithParams(method, paramDatas);
        }

        protected abstract void DrawInternal(IEnumerable<object> targets);
    }
}