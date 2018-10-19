namespace Scorpio.Function
{
    using Scorpio;
    using Scorpio.Exception;
    using Scorpio.Runtime;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;

    public class ScriptScriptFunction : ScriptFunction
    {
        private bool m_IsStaticFunction;
        private ScriptContext m_ParentContext;
        private ScorpioScriptFunction m_ScriptFunction;
        private Dictionary<string, ScriptObject> m_stackObject;

        internal ScriptScriptFunction(Script script, string name, ScorpioScriptFunction function) : base(script, name)
        {
            this.m_stackObject = new Dictionary<string, ScriptObject>();
            this.m_IsStaticFunction = true;
            this.m_ScriptFunction = function;
        }

        public override object Call(ScriptObject[] parameters)
        {
            return this.m_ScriptFunction.Call(this.m_ParentContext, this.m_stackObject, parameters);
        }

        public override ScriptObject Clone()
        {
            return this.Create();
        }

        public ScriptScriptFunction Create()
        {
            return new ScriptScriptFunction(base.m_Script, base.Name, this.m_ScriptFunction) { m_IsStaticFunction = this.IsStaticFunction };
        }

        public override int GetParamCount()
        {
            return this.m_ScriptFunction.GetParameterCount();
        }

        public override ScriptArray GetParams()
        {
            return this.m_ScriptFunction.GetParameters();
        }

        public override ScriptObject GetValue(object key)
        {
            if (!(key is string))
            {
                throw new ExecutionException(base.m_Script, this, "Function GetValue只支持String类型 key值为:" + key);
            }
            string str = (string) key;
            if (!this.m_stackObject.ContainsKey(str))
            {
                return base.m_Script.Null;
            }
            return this.m_stackObject[str];
        }

        public override bool IsParams()
        {
            return this.m_ScriptFunction.IsParams();
        }

        public override bool IsStatic()
        {
            return this.m_IsStaticFunction;
        }

        public ScriptScriptFunction SetParentContext(ScriptContext context)
        {
            this.m_ParentContext = context;
            return this;
        }

        public void SetTable(ScriptTable table)
        {
            this.m_IsStaticFunction = false;
            this.m_stackObject["this"] = table;
            this.m_stackObject["self"] = table;
        }

        public override void SetValue(object key, ScriptObject value)
        {
            if (!(key is string))
            {
                throw new ExecutionException(base.m_Script, this, "Function SetValue只支持String类型 key值为:" + key);
            }
            this.m_stackObject[(string) key] = value;
        }

        public bool IsStaticFunction
        {
            get
            {
                return this.m_IsStaticFunction;
            }
        }
    }
}

