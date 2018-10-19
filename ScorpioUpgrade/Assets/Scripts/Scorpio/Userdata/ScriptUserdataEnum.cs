namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Exception;
    using System;
    using System.Collections.Generic;

    public class ScriptUserdataEnum : ScriptUserdata
    {
        private Dictionary<string, ScriptEnum> m_Enums;

        public ScriptUserdataEnum(Script script, Type value) : base(script)
        {
            base.m_Value = value;
            base.m_ValueType = value;
            this.m_Enums = new Dictionary<string, ScriptEnum>();
            foreach (string str in Enum.GetNames(base.ValueType))
            {
                this.m_Enums[str] = new ScriptEnum(base.m_Script, Enum.Parse(base.ValueType, str));
            }
        }

        public override object Call(ScriptObject[] parameters)
        {
            throw new ExecutionException(base.m_Script, "枚举类型不支持实例化");
        }

        public override ScriptObject GetValue(object key)
        {
            if (!(key is string))
            {
                throw new ExecutionException(base.m_Script, "Enum GetValue只支持String类型");
            }
            string str = (string) key;
            if (!this.m_Enums.ContainsKey(str))
            {
                throw new ExecutionException(base.m_Script, "枚举[" + base.ValueType.ToString() + "] 元素[" + str + "] 不存在");
            }
            return this.m_Enums[str];
        }
    }
}

