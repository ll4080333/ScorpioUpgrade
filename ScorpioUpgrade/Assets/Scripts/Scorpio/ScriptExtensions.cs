namespace Scorpio
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ScriptExtensions
    {
        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            return del.Method;
        }

        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
    }
}

