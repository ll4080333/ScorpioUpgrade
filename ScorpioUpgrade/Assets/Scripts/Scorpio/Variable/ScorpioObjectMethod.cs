namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.Userdata;
    using System;

    public class ScorpioObjectMethod : ScorpioMethod
    {
        private object m_Object;

        public ScorpioObjectMethod(object obj, string name, UserdataMethod method)
        {
            this.m_Object = obj;
            base.m_Method = method;
            base.m_MethodName = name;
        }

        public override object Call(ScriptObject[] parameters)
        {
            return base.m_Method.Call(this.m_Object, parameters);
        }

        public override ScorpioMethod MakeGenericMethod(Type[] parameters)
        {
            return new ScorpioObjectMethod(this.m_Object, base.m_MethodName, base.m_Method.MakeGenericMethod(parameters));
        }
    }
}

