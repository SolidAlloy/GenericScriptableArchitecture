namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;

    internal interface IStackTraceProvider
    {
        bool Expanded { get; set; }

        ICollection<StackTraceEntry> Entries { get; }
    }
}