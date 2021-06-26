namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;

    internal interface IStackTraceProvider
    {
        ICollection<StackTraceEntry> StackTraceEntries { get; }
    }
}