namespace Scorpio.Userdata
{
    using System;

    public interface IScorpioFastReflectMethod
    {
        object Call(object obj, string type, object[] args);
    }
}

