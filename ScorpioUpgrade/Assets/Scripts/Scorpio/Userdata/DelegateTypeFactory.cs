namespace Scorpio.Userdata
{
    using Scorpio;
    using System;

    public interface DelegateTypeFactory
    {
        Delegate CreateDelegate(Script script, Type type, ScriptFunction func);
    }
}

