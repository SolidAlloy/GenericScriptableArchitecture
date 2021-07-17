namespace GenericScriptableArchitecture
{
    using System;
    using System.Linq;
    using SolidUtilities.Extensions;
    using SolidUtilities.Helpers;
    using UnityEngine;

    public class StackTraceEntry : IEquatable<StackTraceEntry>
    {
        private readonly int _id;
        private readonly int _frameCount;
        private readonly string _stackTrace;
        private readonly object[] _values;

        private string _stringRepresentation;

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
            if (_stringRepresentation == null)
            {
                var valuesString = string.Join(", ", _values.Select(value => value ?? "null"));
                _stringRepresentation = $"{_frameCount}{(_values.Length != 0 ? $"   [{valuesString}]" : string.Empty)} {GetStackTraceWithLinks(_stackTrace)}";
            }

            return _stringRepresentation;
        }

        private string GetStackTraceWithLinks(string stackTrace)
        {
            string unityStackTrace = TransformStackTraceToUnity(stackTrace);
            return StackTraceHelper.AddLinks(unityStackTrace);
        }

        private string TransformStackTraceToUnity(string stackTrace)
        {
            // Remove the first four lines
            int thirdLineEnd = stackTrace.IndexOfNth('\n', 3);
            stackTrace = stackTrace.Remove(0, thirdLineEnd + 1);
            return StackTraceHelper.EnvironmentToUnityStyle(stackTrace);
        }

        public static implicit operator string(StackTraceEntry trace) => trace.ToString();
    }
}