namespace Scorpio.Exception
{
    using Scorpio;
    using System;

    internal class ExecutionException : ScriptException
    {
        private string m_Source;

        public ExecutionException(Script script, string strMessage) : base(strMessage)
        {
            this.m_Source = "";
            if (script != null)
            {
                StackInfo currentStackInfo = script.GetCurrentStackInfo();
                if (currentStackInfo != null)
                {
                    this.m_Source = string.Concat(new object[] { currentStackInfo.Breviary, ":", currentStackInfo.Line, ": " });
                }
            }
        }

        public ExecutionException(Script script, ScriptObject obj, string strMessage) : base(strMessage)
        {
            this.m_Source = "";
            if (script != null)
            {
                StackInfo currentStackInfo = script.GetCurrentStackInfo();
                if (currentStackInfo != null)
                {
                    this.m_Source = string.Concat(new object[] { currentStackInfo.Breviary, ":", currentStackInfo.Line, "[", obj.Name, "]:" });
                }
            }
        }

        public override string Message
        {
            get
            {
                return (this.m_Source + base.Message);
            }
        }
    }
}

