namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.Userdata;
    using System;

    public class ScorpioStaticMethod : ScorpioMethod
    {
        public ScorpioStaticMethod(string name, UserdataMethod method)
        {
            base.m_Method = method;
            base.m_MethodName = name;
        }

        public override object Call(ScriptObject[] parameters)
        {
            return base.m_Method.Call(null, parameters);
        }

        public override ScorpioMethod MakeGenericMethod(Type[] parameters)
        {
            return new ScorpioStaticMethod(base.m_MethodName, base.m_Method.MakeGenericMethod(parameters));
        }
    }
}

