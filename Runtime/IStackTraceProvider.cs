namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;

    internal interface IStackTraceProvider
    {
        IEnumerable<StackTraceEntry> StackTraceEntries { get; }
    }
}