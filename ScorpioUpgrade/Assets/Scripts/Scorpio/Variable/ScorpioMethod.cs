namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.Userdata;
    using System;

    public abstract class ScorpioMethod
    {
        protected UserdataMethod m_Method;
        protected string m_MethodName;

        protected ScorpioMethod()
        {
        }

        public abstract object Call(ScriptObject[] parameters);
        public abstract ScorpioMethod MakeGenericMethod(Type[] parameters);

        public UserdataMethod Method
        {
            get
            {
                return this.m_Method;
            }
        }

        public string MethodName
        {
            get
            {
                return this.m_MethodName;
            }
        }
    }
}

