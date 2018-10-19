namespace Scorpio.Function
{
    using Scorpio;
    using Scorpio.Exception;
    using System;

    public class ScriptDelegateFunction : ScriptFunction
    {
        private ScorpioFunction m_Function;

        public ScriptDelegateFunction(Script script, ScorpioFunction function) : this(script, function.ToString(), function)
        {
        }

        public ScriptDelegateFunction(Script script, string name, ScorpioFunction function) : base(script, name)
        {
            this.m_Function = function;
        }

        public override object Call(ScriptObject[] parameters)
        {
            object obj2;
            try
            {
                obj2 = this.m_Function(base.m_Script, parameters);
            }
            catch (Exception exception)
            {
                throw new ExecutionException(base.m_Script, "CallFunction [" + base.Name + "] is error : " + exception.ToString());
            }
            return obj2;
        }
    }
}

