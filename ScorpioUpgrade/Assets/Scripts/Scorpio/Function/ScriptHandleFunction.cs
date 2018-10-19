namespace Scorpio.Function
{
    using Scorpio;
    using Scorpio.Exception;
    using System;

    public class ScriptHandleFunction : ScriptFunction
    {
        private ScorpioHandle m_Handle;

        public ScriptHandleFunction(Script script, ScorpioHandle handle) : this(script, handle.GetType().FullName, handle)
        {
        }

        public ScriptHandleFunction(Script script, string name, ScorpioHandle handle) : base(script, name)
        {
            this.m_Handle = handle;
        }

        public override object Call(ScriptObject[] parameters)
        {
            object obj2;
            try
            {
                obj2 = this.m_Handle.Call(parameters);
            }
            catch (Exception exception)
            {
                throw new ExecutionException(base.m_Script, "CallFunction [" + base.Name + "] is error : " + exception.ToString());
            }
            return obj2;
        }
    }
}

