namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.Runtime;
    using System;
    using System.Collections.Generic;

    internal class ScorpioScriptFunction
    {
        private string[] m_ListParameters;
        private int m_ParameterCount;
        private bool m_Params;
        private Script m_Script;
        private ScriptExecutable m_ScriptExecutable;

        public ScorpioScriptFunction(Script script, List<string> listParameters, ScriptExecutable scriptExecutable, bool bParams)
        {
            this.m_Script = script;
            this.m_ListParameters = listParameters.ToArray();
            this.m_ScriptExecutable = scriptExecutable;
            this.m_ParameterCount = listParameters.Count;
            this.m_Params = bParams;
        }

        public ScriptObject Call(ScriptContext parentContext, Dictionary<string, ScriptObject> objs, ScriptObject[] parameters)
        {
            int length = parameters.Length;
            if (this.m_Params)
            {
                ScriptArray array = this.m_Script.CreateArray();
                for (int i = 0; i < (this.m_ParameterCount - 1); i++)
                {
                    objs[this.m_ListParameters[i]] = ((parameters != null) && (length > i)) ? parameters[i] : this.m_Script.Null;
                }
                for (int j = this.m_ParameterCount - 1; j < length; j++)
                {
                    array.Add(parameters[j]);
                }
                objs[this.m_ListParameters[this.m_ParameterCount - 1]] = array;
            }
            else
            {
                for (int k = 0; k < this.m_ParameterCount; k++)
                {
                    objs[this.m_ListParameters[k]] = ((parameters != null) && (length > k)) ? parameters[k] : this.m_Script.Null;
                }
            }
            ScriptContext context = new ScriptContext(this.m_Script, this.m_ScriptExecutable, parentContext, Executable_Block.Function);
            context.Initialize(objs);
            return context.Execute();
        }

        public int GetParameterCount()
        {
            return this.m_ParameterCount;
        }

        public ScriptArray GetParameters()
        {
            ScriptArray array = this.m_Script.CreateArray();
            foreach (string str in this.m_ListParameters)
            {
                array.Add(this.m_Script.CreateString(str));
            }
            return array;
        }

        public bool IsParams()
        {
            return this.m_Params;
        }
    }
}

