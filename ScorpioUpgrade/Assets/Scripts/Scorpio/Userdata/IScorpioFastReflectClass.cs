namespace Scorpio.Userdata
{
    using Scorpio;
    using System;

    public interface IScorpioFastReflectClass
    {
        FastReflectUserdataMethod GetConstructor();
        object GetValue(object obj, string name);
        void SetValue(object obj, string name, ScriptObject value);
    }
}

