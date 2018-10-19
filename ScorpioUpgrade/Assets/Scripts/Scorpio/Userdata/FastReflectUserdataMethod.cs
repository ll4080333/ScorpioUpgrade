namespace Scorpio.Userdata
{
    using Scorpio;
    using System;

    public class FastReflectUserdataMethod : UserdataMethod
    {
        public FastReflectUserdataMethod(bool isStatic, Script script, Type type, string methodName, ScorpioMethodInfo[] methods, IScorpioFastReflectMethod fastMethod)
        {
            base.Initialize(isStatic, script, type, methodName, methods, fastMethod);
        }
    }
}

