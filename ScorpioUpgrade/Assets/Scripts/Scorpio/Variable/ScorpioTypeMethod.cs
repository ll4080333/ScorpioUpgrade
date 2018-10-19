namespace Scorpio.Variable
{
    using Scorpio;
    using Scorpio.Userdata;
    using System;

    public class ScorpioTypeMethod : ScorpioMethod
    {
        private Script m_script;
        private Type m_Type;

        public ScorpioTypeMethod(Script script, string name, UserdataMethod method, Type type)
        {
            this.m_script = script;
            this.m_Type = type;
            base.m_Method = method;
            base.m_MethodName = name;
        }

        public override object Call(ScriptObject[] parameters)
        {
            int length = parameters.Length;
            Util.Assert(length > 0, this.m_script, "length > 0");
            if (length <= 1)
            {
                return base.m_Method.Call(parameters[0].ObjectValue, new ScriptObject[0]);
            }
            ScriptObject[] destinationArray = new ScriptObject[parameters.Length - 1];
            Array.Copy(parameters, 1, destinationArray, 0, destinationArray.Length);
            if (parameters[0] is ScriptNumber)
            {
                return base.m_Method.Call(Util.ChangeType_impl(parameters[0].ObjectValue, this.m_Type), destinationArray);
            }
            return base.m_Method.Call(parameters[0].ObjectValue, destinationArray);
        }

        public override ScorpioMethod MakeGenericMethod(Type[] parameters)
        {
            return new ScorpioTypeMethod(this.m_script, base.m_MethodName, base.m_Method.MakeGenericMethod(parameters), this.m_Type);
        }
    }
}

