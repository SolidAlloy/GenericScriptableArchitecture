#if UNIRX
namespace GenericScriptableArchitecture
{
    using System;

    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => ex.Throw();
    }

    internal static class Stubs<T1, T2>
    {
        public static readonly Action<T1, T2> Ignore = (x, y) => { };
    }
}
#endif