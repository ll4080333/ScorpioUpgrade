namespace Scorpio
{
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;

    public class ScriptString : ScriptObject
    {
        private string m_Value;

        public ScriptString(Script script, string value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptObject Assign()
        {
            return new ScriptString(base.m_Script, this.m_Value);
        }

        public override ScriptObject AssignCompute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            if (type != Scorpio.Compiler.TokenType.AssignPlus)
            {
                throw new ExecutionException(base.m_Script, this, "String类型 操作符[" + type + "]不支持");
            }
            this.m_Value = this.m_Value + obj.ToString();
            return this;
        }

        public override ScriptObject Clone()
        {
            return new ScriptString(base.m_Script, this.m_Value);
        }

        public override bool Compare(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScriptString str = obj as ScriptString;
            if (str == null)
            {
                throw new ExecutionException(base.m_Script, this, "字符串比较 右边必须为字符串类型");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Greater:
                    return (string.Compare(this.m_Value, str.m_Value) > 0);

                case Scorpio.Compiler.TokenType.GreaterOrEqual:
                    return (string.Compare(this.m_Value, str.m_Value) >= 0);

                case Scorpio.Compiler.TokenType.Less:
                    return (string.Compare(this.m_Value, str.m_Value) < 0);

                case Scorpio.Compiler.TokenType.LessOrEqual:
                    return (string.Compare(this.m_Value, str.m_Value) <= 0);
            }
            throw new ExecutionException(base.m_Script, this, "String类型 操作符[" + type + "]不支持");
        }

        public override ScriptObject GetValue(object index)
        {
            if ((!(index is double) && !(index is int)) && !(index is long))
            {
                throw new ExecutionException(base.m_Script, this, "String GetValue只支持Number类型");
            }
            return new ScriptString(base.m_Script, this.m_Value[Util.ToInt32(index)].ToString());
        }

        public override string ToJson()
        {
            return ("\"" + this.m_Value.Replace("\"", "\\\"") + "\"");
        }

        public override object KeyValue
        {
            get
            {
                return this.m_Value;
            }
        }

        public override object ObjectValue
        {
            get
            {
                return this.m_Value;
            }
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.String;
            }
        }

        public string Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

