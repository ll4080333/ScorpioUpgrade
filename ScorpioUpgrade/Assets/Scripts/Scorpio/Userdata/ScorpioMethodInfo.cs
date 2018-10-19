namespace Scorpio.Userdata
{
    using System;

    public class ScorpioMethodInfo
    {
        public bool IsStatic;
        public string Name;
        public Type[] ParameterType;
        public string ParameterTypes;
        public bool Params;
        public Type ParamType;

        public ScorpioMethodInfo(string name, bool isStatic, Type[] parameterType, bool param, Type paramType, string parameterTypes)
        {
            this.Name = name;
            this.IsStatic = isStatic;
            this.ParameterType = parameterType;
            this.Params = param;
            this.ParamType = paramType;
            this.ParameterTypes = parameterTypes;
        }
    }
}

