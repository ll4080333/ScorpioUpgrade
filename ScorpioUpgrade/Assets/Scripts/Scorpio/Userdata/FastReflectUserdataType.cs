namespace Scorpio.Userdata
{
    using Scorpio;
    using Scorpio.Compiler;
    using Scorpio.Variable;
    using System;
    using System.Reflection;

    public class FastReflectUserdataType : UserdataType
    {
        private FastReflectUserdataMethod m_Constructor;
        private IScorpioFastReflectClass m_Value;

        public FastReflectUserdataType(Script script, Type type, IScorpioFastReflectClass value) : base(script, type)
        {
            this.m_Value = value;
            this.m_Constructor = value.GetConstructor();
        }

        public override void AddExtensionMethod(MethodInfo method)
        {
        }

        public override object CreateInstance(ScriptObject[] parameters)
        {
            return this.m_Constructor.Call(null, parameters);
        }

        public override ScorpioMethod GetComputeMethod(Scorpio.Compiler.TokenType type)
        {
            Scorpio.Compiler.TokenType type2 = type;
            if (type2 != Scorpio.Compiler.TokenType.Plus)
            {
                switch (type2)
                {
                    case Scorpio.Compiler.TokenType.Multiply:
                        return (this.m_Value.GetValue(null, "op_Multiply") as ScorpioMethod);

                    case Scorpio.Compiler.TokenType.Divide:
                        return (this.m_Value.GetValue(null, "op_Division") as ScorpioMethod);

                    case Scorpio.Compiler.TokenType.Minus:
                        return (this.m_Value.GetValue(null, "op_Subtraction") as ScorpioMethod);
                }
            }
            else
            {
                return (this.m_Value.GetValue(null, "op_Addition") as ScorpioMethod);
            }
            return null;
        }

        public override object GetValue_impl(object obj, string name)
        {
            return this.m_Value.GetValue(obj, name);
        }

        public override void SetValue_impl(object obj, string name, ScriptObject value)
        {
            this.m_Value.SetValue(obj, name, value);
        }
    }
}

