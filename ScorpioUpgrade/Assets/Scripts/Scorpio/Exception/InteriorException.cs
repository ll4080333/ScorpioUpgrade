namespace Scorpio.Exception
{
    using Scorpio;
    using System;

    public class InteriorException : Exception
    {
        public ScriptObject obj;

        public InteriorException(ScriptObject obj)
        {
            this.obj = obj;
        }
    }
}

