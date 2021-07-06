namespace GenericScriptableArchitecture
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using SolidUtilities.Extensions;
    using UnityEngine;

    public class StackTraceEntry : IEquatable<StackTraceEntry>
    {
        private readonly int _id;
        private readonly int _frameCount;
        private readonly string _stackTrace;
        private readonly object[] _values;

        public StackTraceEntry(params object[] values)
        {
            _id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            _stackTrace = Environment.StackTrace;
            _values = values;

            if (Application.isPlaying)
            {
                _frameCount = Time.frameCount;
            }
        }

        public override bool Equals(object obj) => Equals(obj as StackTraceEntry);

        public bool Equals(StackTraceEntry other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _id == other._id;
        }

        public static bool operator ==(StackTraceEntry first, StackTraceEntry second)
        {
            if (first is null)
            {
                if (second is null)
                {
                    return true;
                }

                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(StackTraceEntry first, StackTraceEntry second) => !(first == second);

        public override int GetHashCode() => _id;

        public override string ToString()
        {
            var valuesString = string.Join(", ", _values.Select(value => value ?? "null"));
            return $"{_frameCount}{(_values.Length != 0 ? $"   [{valuesString}]" : string.Empty)} {GetStackTraceWithLinks(_stackTrace)}";
        }

        private string GetStackTraceWithLinks(string stackTrace)
        {
            string unityStackTrace = TransformStackTraceToUnity(stackTrace);
            return AddLinks(unityStackTrace);
        }

        private string TransformStackTraceToUnity(string stackTrace)
        {
            // Remove first three lines
            int thirdLineEnd = stackTrace.IndexOfNth('\n', 2);
            stackTrace = stackTrace.Remove(0, thirdLineEnd + 1);

            // Remove at at the start of lines
            stackTrace = Regex.Replace(stackTrace, @" +at ", string.Empty);

            // Remove the location part of the stack line if there is no reference to a file on disc
            stackTrace = Regex.Replace(stackTrace, @" \[0x00000\] in <.*?(?=\n|$)", string.Empty);

            // Replace [0x00000] in with (at
            stackTrace = Regex.Replace(stackTrace, @"\[0x\w+?\] in", "(at");

            // Add a closing parenthese
            stackTrace = Regex.Replace(stackTrace, @"(?<=:\d+) ", ")");

            return stackTrace;
        }

        private string AddLinks(string unityStackTrace)
        {
            var assembly = typeof(UnityEditor.Editor).Assembly;
            var consoleWindow = assembly.GetType("UnityEditor.ConsoleWindow");
            var method = consoleWindow.GetMethod("StacktraceWithHyperlinks", BindingFlags.NonPublic | BindingFlags.Static);
            return (string) method.Invoke(null, new object[] {unityStackTrace, 0});
        }

        public static implicit operator string(StackTraceEntry trace) => trace.ToString();
    }
}