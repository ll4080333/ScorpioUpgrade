namespace Scorpio.Function
{
    using Scorpio;
    using Scorpio.Exception;
    using Scorpio.Variable;
    using System;

    public class ScriptMethodFunction : ScriptFunction
    {
        private ScorpioMethod m_Method;

        public ScriptMethodFunction(Script script, ScorpioMethod method) : this(script, method.MethodName, method)
        {
        }

        public ScriptMethodFunction(Script script, string name, ScorpioMethod method) : base(script, name)
        {
            this.m_Method = method;
        }

        public override object Call(ScriptObject[] parameters)
        {
            object obj2;
            try
            {
                obj2 = this.m_Method.Call(parameters);
            }
            catch (Exception exception)
            {
                throw new ExecutionException(base.m_Script, "CallFunction [" + base.Name + "] is error : " + exception.ToString());
            }
            return obj2;
        }

        public ScorpioMethod Method
        {
            get
            {
                return this.m_Method;
            }
        }
    }
}

