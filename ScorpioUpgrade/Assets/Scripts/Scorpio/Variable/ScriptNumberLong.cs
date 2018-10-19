namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.CodeDom;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;

    public class ScriptNumberLong : ScriptNumber
    {
        private long m_Value;

        public ScriptNumberLong(Script script, long value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptNumber Abs()
        {
            if (this.m_Value >= 0L)
            {
                return new ScriptNumberLong(base.m_Script, this.m_Value);
            }
            return new ScriptNumberLong(base.m_Script, -this.m_Value);
        }

        public override ScriptObject Assign()
        {
            return new ScriptNumberLong(base.m_Script, this.m_Value);
        }

        public override ScriptObject AssignCompute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScriptNumber number = obj as ScriptNumber;
            if (number == null)
            {
                throw new ExecutionException(base.m_Script, this, "赋值逻辑计算 右边值必须为数字类型");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.AssignShi:
                    this.m_Value = this.m_Value << number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignShr:
                    this.m_Value = this.m_Value >> number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignXOR:
                    this.m_Value ^= number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignCombine:
                    this.m_Value &= number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignMinus:
                    this.m_Value -= number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignMultiply:
                    this.m_Value *= number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignDivide:
                    this.m_Value /= number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignModulo:
                    this.m_Value = this.m_Value % number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignInclusiveOr:
                    this.m_Value |= number.ToLong();
                    return this;

                case Scorpio.Compiler.TokenType.AssignPlus:
                    this.m_Value += number.ToLong();
                    return this;
            }
            throw new ExecutionException(base.m_Script, this, "Long不支持的运算符 " + type);
        }

        public override ScriptNumber Calc(CALC c)
        {
            switch (c)
            {
                case CALC.PRE_INCREMENT:
                    this.m_Value += 1L;
                    break;

                case CALC.POST_INCREMENT:
                    long num;
                    this.m_Value = (num = this.m_Value) + 1L;
                    return new ScriptNumberLong(base.m_Script, num);

                case CALC.PRE_DECREMENT:
                    this.m_Value -= 1L;
                    break;

                case CALC.POST_DECREMENT:
                    long num2;
                    this.m_Value = (num2 = this.m_Value) - 1L;
                    return new ScriptNumberLong(base.m_Script, num2);

                default:
                    return this;
            }
            return this;
        }

        public override ScriptNumber Clamp(ScriptNumber min, ScriptNumber max)
        {
            long num = min.ToLong();
            if (this.m_Value < num)
            {
                return new ScriptNumberLong(base.m_Script, num);
            }
            num = max.ToLong();
            if (this.m_Value > num)
            {
                return new ScriptNumberLong(base.m_Script, num);
            }
            return new ScriptNumberLong(base.m_Script, this.m_Value);
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberLong(base.m_Script, this.m_Value);
        }

        public override bool Compare(Scorpio.Compiler.TokenType type, ScriptObject num)
        {
            ScriptNumberLong @long = num as ScriptNumberLong;
            if (@long == null)
            {
                throw new ExecutionException(base.m_Script, this, "数字比较 两边的数字类型不一致 请先转换再比较 ");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Greater:
                    return (this.m_Value > @long.m_Value);

                case Scorpio.Compiler.TokenType.GreaterOrEqual:
                    return (this.m_Value >= @long.m_Value);

                case Scorpio.Compiler.TokenType.Less:
                    return (this.m_Value < @long.m_Value);

                case Scorpio.Compiler.TokenType.LessOrEqual:
                    return (this.m_Value <= @long.m_Value);
            }
            throw new ExecutionException(base.m_Script, this, "Long类型 操作符[" + type + "]不支持");
        }

        public override ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScriptNumber number = obj as ScriptNumber;
            if (number == null)
            {
                throw new ExecutionException(base.m_Script, this, "赋值逻辑计算 右边值必须为数字类型");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Multiply:
                    return new ScriptNumberLong(base.m_Script, this.m_Value * number.ToLong());

                case Scorpio.Compiler.TokenType.Divide:
                    return new ScriptNumberLong(base.m_Script, this.m_Value / number.ToLong());

                case Scorpio.Compiler.TokenType.Modulo:
                    return new ScriptNumberLong(base.m_Script, this.m_Value % number.ToLong());

                case Scorpio.Compiler.TokenType.InclusiveOr:
                    return new ScriptNumberLong(base.m_Script, this.m_Value | number.ToLong());

                case Scorpio.Compiler.TokenType.Minus:
                    return new ScriptNumberLong(base.m_Script, this.m_Value - number.ToLong());

                case Scorpio.Compiler.TokenType.Plus:
                    return new ScriptNumberLong(base.m_Script, this.m_Value + number.ToLong());

                case Scorpio.Compiler.TokenType.Shi:
                    return new ScriptNumberLong(base.m_Script, this.m_Value << number.ToInt32());

                case Scorpio.Compiler.TokenType.Shr:
                    return new ScriptNumberLong(base.m_Script, this.m_Value >> number.ToInt32());

                case Scorpio.Compiler.TokenType.XOR:
                    return new ScriptNumberLong(base.m_Script, this.m_Value ^ number.ToLong());

                case Scorpio.Compiler.TokenType.Combine:
                    return new ScriptNumberLong(base.m_Script, this.m_Value & number.ToLong());
            }
            throw new ExecutionException(base.m_Script, this, "Long不支持的运算符 " + type);
        }

        public override ScriptNumber Floor()
        {
            return new ScriptNumberLong(base.m_Script, this.m_Value);
        }

        public override ScriptNumber Minus()
        {
            return new ScriptNumberLong(base.m_Script, -this.m_Value);
        }

        public override ScriptNumber Negative()
        {
            return new ScriptNumberLong(base.m_Script, ~this.m_Value);
        }

        public override long ToLong()
        {
            return this.m_Value;
        }

        public override int BranchType
        {
            get
            {
                return 1;
            }
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
                return ObjectType.Number;
            }
        }

        public long Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

