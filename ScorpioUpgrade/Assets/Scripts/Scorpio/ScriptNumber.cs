namespace Scorpio
{
    using Scorpio.CodeDom;
    using Scorpio.Exception;
    using System;

    public abstract class ScriptNumber : ScriptObject
    {
        protected ScriptNumber(Script script) : base(script)
        {
        }

        public virtual ScriptNumber Abs()
        {
            throw new ExecutionException(base.m_Script, "数字类型不支持Abs函数");
        }

        public virtual ScriptNumber Calc(CALC c)
        {
            throw new ExecutionException(base.m_Script, "数字类型不支持Calc函数");
        }

        public virtual ScriptNumber Clamp(ScriptNumber min, ScriptNumber max)
        {
            throw new ExecutionException(base.m_Script, "数字类型不支持Clamp函数");
        }

        public virtual ScriptNumber Floor()
        {
            throw new ExecutionException(base.m_Script, "数字类型不支持Floor函数");
        }

        public virtual ScriptNumber Minus()
        {
            throw new ExecutionException(base.m_Script, "数字类型不支持Minus函数");
        }

        public virtual ScriptNumber Negative()
        {
            throw new ExecutionException(base.m_Script, "数字类型不支持Negative函数");
        }

        public ScriptNumber Pow(ScriptNumber value)
        {
            return base.m_Script.CreateDouble(Math.Pow(this.ToDouble(), value.ToDouble()));
        }

        public ScriptNumber Sqrt()
        {
            return base.m_Script.CreateDouble(Math.Sqrt(this.ToDouble()));
        }

        public virtual double ToDouble()
        {
            return Util.ToDouble(this.ObjectValue);
        }

        public virtual int ToInt32()
        {
            return Util.ToInt32(this.ObjectValue);
        }

        public virtual long ToLong()
        {
            return Util.ToInt64(this.ObjectValue);
        }

        public override ObjectType Type
        {
            get
            {
                return ObjectType.Number;
            }
        }
    }
}

