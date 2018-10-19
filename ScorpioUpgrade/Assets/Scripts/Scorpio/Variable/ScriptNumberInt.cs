namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.CodeDom;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;

    public class ScriptNumberInt : ScriptNumber
    {
        private int m_Value;

        public ScriptNumberInt(Script script, int value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptNumber Abs()
        {
            if (this.m_Value >= 0)
            {
                return new ScriptNumberInt(base.m_Script, this.m_Value);
            }
            return new ScriptNumberInt(base.m_Script, -this.m_Value);
        }

        public override ScriptObject Assign()
        {
            return new ScriptNumberInt(base.m_Script, this.m_Value);
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
                    this.m_Value ^= number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignCombine:
                    this.m_Value &= number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignMinus:
                    this.m_Value -= number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignMultiply:
                    this.m_Value *= number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignDivide:
                    this.m_Value /= number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignModulo:
                    this.m_Value = this.m_Value % number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignInclusiveOr:
                    this.m_Value |= number.ToInt32();
                    return this;

                case Scorpio.Compiler.TokenType.AssignPlus:
                    this.m_Value += number.ToInt32();
                    return this;
            }
            throw new ExecutionException(base.m_Script, this, "Int不支持的运算符 " + type);
        }

        public override ScriptNumber Calc(CALC c)
        {
            switch (c)
            {
                case CALC.PRE_INCREMENT:
                    this.m_Value++;
                    break;

                case CALC.POST_INCREMENT:
                    return new ScriptNumberInt(base.m_Script, this.m_Value++);

                case CALC.PRE_DECREMENT:
                    this.m_Value--;
                    break;

                case CALC.POST_DECREMENT:
                    return new ScriptNumberInt(base.m_Script, this.m_Value--);

                default:
                    return this;
            }
            return this;
        }

        public override ScriptNumber Clamp(ScriptNumber min, ScriptNumber max)
        {
            if (this.m_Value < min.ToInt32())
            {
                return new ScriptNumberInt(base.m_Script, min.ToInt32());
            }
            if (this.m_Value > max.ToInt32())
            {
                return new ScriptNumberInt(base.m_Script, max.ToInt32());
            }
            return new ScriptNumberInt(base.m_Script, this.m_Value);
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberInt(base.m_Script, this.m_Value);
        }

        public override bool Compare(Scorpio.Compiler.TokenType type, ScriptObject num)
        {
            ScriptNumberInt num2 = num as ScriptNumberInt;
            if (num2 == null)
            {
                throw new ExecutionException(base.m_Script, this, "数字比较 两边的数字类型不一致 请先转换再比较 ");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Greater:
                    return (this.m_Value > num2.m_Value);

                case Scorpio.Compiler.TokenType.GreaterOrEqual:
                    return (this.m_Value >= num2.m_Value);

                case Scorpio.Compiler.TokenType.Less:
                    return (this.m_Value < num2.m_Value);

                case Scorpio.Compiler.TokenType.LessOrEqual:
                    return (this.m_Value <= num2.m_Value);
            }
            throw new ExecutionException(base.m_Script, this, "Int类型 操作符[" + type + "]不支持");
        }

        public override ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScriptNumber number = obj as ScriptNumber;
            if (number == null)
            {
                throw new ExecutionException(base.m_Script, this, "逻辑计算 右边值必须为数字类型");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Multiply:
                    return new ScriptNumberInt(base.m_Script, this.m_Value * number.ToInt32());

                case Scorpio.Compiler.TokenType.Divide:
                    return new ScriptNumberInt(base.m_Script, this.m_Value / number.ToInt32());

                case Scorpio.Compiler.TokenType.Modulo:
                    return new ScriptNumberInt(base.m_Script, this.m_Value % number.ToInt32());

                case Scorpio.Compiler.TokenType.InclusiveOr:
                    return new ScriptNumberInt(base.m_Script, this.m_Value | number.ToInt32());

                case Scorpio.Compiler.TokenType.Minus:
                    return new ScriptNumberInt(base.m_Script, this.m_Value - number.ToInt32());

                case Scorpio.Compiler.TokenType.Plus:
                    return new ScriptNumberInt(base.m_Script, this.m_Value + number.ToInt32());

                case Scorpio.Compiler.TokenType.Shi:
                    return new ScriptNumberInt(base.m_Script, this.m_Value << number.ToInt32());

                case Scorpio.Compiler.TokenType.Shr:
                    return new ScriptNumberInt(base.m_Script, this.m_Value >> number.ToInt32());

                case Scorpio.Compiler.TokenType.XOR:
                    return new ScriptNumberInt(base.m_Script, this.m_Value ^ number.ToInt32());

                case Scorpio.Compiler.TokenType.Combine:
                    return new ScriptNumberInt(base.m_Script, this.m_Value & number.ToInt32());
            }
            throw new ExecutionException(base.m_Script, this, "Int不支持的运算符 " + type);
        }

        public override ScriptNumber Floor()
        {
            return new ScriptNumberInt(base.m_Script, this.m_Value);
        }

        public override ScriptNumber Minus()
        {
            return new ScriptNumberInt(base.m_Script, -this.m_Value);
        }

        public override ScriptNumber Negative()
        {
            return new ScriptNumberInt(base.m_Script, ~this.m_Value);
        }

        public override int ToInt32()
        {
            return this.m_Value;
        }

        public override int BranchType
        {
            get
            {
                return 2;
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

        public int Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

