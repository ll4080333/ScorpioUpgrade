namespace Scorpio
{
    using System;

    public interface ScorpioHandle
    {
        object Call(ScriptObject[] Parameters);
    }
}

