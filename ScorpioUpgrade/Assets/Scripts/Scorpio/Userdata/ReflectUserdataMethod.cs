namespace Scorpio.Userdata
{
    using Scorpio;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ReflectUserdataMethod : UserdataMethod
    {
        public ReflectUserdataMethod(Script script, Type type, string methodName, List<MethodInfo> methods)
        {
            base.Initialize(script, type, methodName, methods);
        }

        public ReflectUserdataMethod(Script script, Type type, string methodName, ConstructorInfo[] cons)
        {
            base.Initialize(script, type, methodName, cons);
        }
    }
}

