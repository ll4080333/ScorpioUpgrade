﻿namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.CodeDom;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using System;

    public class ScriptNumberDouble : ScriptNumber
    {
        private double m_Value;

        public ScriptNumberDouble(Script script, double value) : base(script)
        {
            this.m_Value = value;
        }

        public override ScriptNumber Abs()
        {
            if (this.m_Value >= 0.0)
            {
                return new ScriptNumberDouble(base.m_Script, this.m_Value);
            }
            return new ScriptNumberDouble(base.m_Script, -this.m_Value);
        }

        public override ScriptObject Assign()
        {
            return new ScriptNumberDouble(base.m_Script, this.m_Value);
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
                case Scorpio.Compiler.TokenType.AssignMinus:
                    this.m_Value -= number.ToDouble();
                    return this;

                case Scorpio.Compiler.TokenType.AssignMultiply:
                    this.m_Value *= number.ToDouble();
                    return this;

                case Scorpio.Compiler.TokenType.AssignDivide:
                    this.m_Value /= number.ToDouble();
                    return this;

                case Scorpio.Compiler.TokenType.AssignModulo:
                    this.m_Value = this.m_Value % number.ToDouble();
                    return this;

                case Scorpio.Compiler.TokenType.AssignPlus:
                    this.m_Value += number.ToDouble();
                    return this;
            }
            throw new ExecutionException(base.m_Script, this, "Double不支持的运算符 " + type);
        }

        public override ScriptNumber Calc(CALC c)
        {
            switch (c)
            {
                case CALC.PRE_INCREMENT:
                    this.m_Value++;
                    break;

                case CALC.POST_INCREMENT:
                    double num;
                    this.m_Value = (num = this.m_Value) + 1.0;
                    return new ScriptNumberDouble(base.m_Script, num);

                case CALC.PRE_DECREMENT:
                    this.m_Value--;
                    break;

                case CALC.POST_DECREMENT:
                    double num2;
                    this.m_Value = (num2 = this.m_Value) - 1.0;
                    return new ScriptNumberDouble(base.m_Script, num2);

                default:
                    return this;
            }
            return this;
        }

        public override ScriptNumber Clamp(ScriptNumber min, ScriptNumber max)
        {
            if (this.m_Value < min.ToDouble())
            {
                return new ScriptNumberDouble(base.m_Script, min.ToDouble());
            }
            if (this.m_Value > max.ToDouble())
            {
                return new ScriptNumberDouble(base.m_Script, max.ToDouble());
            }
            return new ScriptNumberDouble(base.m_Script, this.m_Value);
        }

        public override ScriptObject Clone()
        {
            return new ScriptNumberDouble(base.m_Script, this.m_Value);
        }

        public override bool Compare(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScriptNumberDouble num = obj as ScriptNumberDouble;
            if (num == null)
            {
                throw new ExecutionException(base.m_Script, this, "数字比较 两边的数字类型不一致 请先转换再比较");
            }
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Greater:
                    return (this.m_Value > num.m_Value);

                case Scorpio.Compiler.TokenType.GreaterOrEqual:
                    return (this.m_Value >= num.m_Value);

                case Scorpio.Compiler.TokenType.Less:
                    return (this.m_Value < num.m_Value);

                case Scorpio.Compiler.TokenType.LessOrEqual:
                    return (this.m_Value <= num.m_Value);
            }
            throw new ExecutionException(base.m_Script, this, "Double类型 操作符[" + type + "]不支持");
        }

        public override ScriptObject Compute(Scorpio.Compiler.TokenType type, ScriptObject obj)
        {
            ScriptNumber number = obj as ScriptNumber;
            if (number == null)
            {
                throw new ExecutionException(base.m_Script, this, "逻辑计算 右边值必须为数字类型");
            }
            Scorpio.Compiler.TokenType type2 = type;
            if (type2 != Scorpio.Compiler.TokenType.Plus)
            {
                switch (type2)
                {
                    case Scorpio.Compiler.TokenType.Multiply:
                        return new ScriptNumberDouble(base.m_Script, this.m_Value * number.ToDouble());

                    case Scorpio.Compiler.TokenType.Divide:
                        return new ScriptNumberDouble(base.m_Script, this.m_Value / number.ToDouble());

                    case Scorpio.Compiler.TokenType.Modulo:
                        return new ScriptNumberDouble(base.m_Script, this.m_Value % number.ToDouble());

                    case Scorpio.Compiler.TokenType.Minus:
                        return new ScriptNumberDouble(base.m_Script, this.m_Value - number.ToDouble());
                }
            }
            else
            {
                return new ScriptNumberDouble(base.m_Script, this.m_Value + number.ToDouble());
            }
            throw new ExecutionException(base.m_Script, this, "Double不支持的运算符 " + type);
        }

        public override ScriptNumber Floor()
        {
            return new ScriptNumberDouble(base.m_Script, Math.Floor(this.m_Value));
        }

        public override ScriptNumber Minus()
        {
            return new ScriptNumberDouble(base.m_Script, -this.m_Value);
        }

        public override double ToDouble()
        {
            return this.m_Value;
        }

        public override int BranchType
        {
            get
            {
                return 0;
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

        public double Value
        {
            get
            {
                return this.m_Value;
            }
        }
    }
}

