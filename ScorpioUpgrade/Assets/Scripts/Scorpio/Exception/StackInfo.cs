namespace Scorpio.Exception
{
    using System;

    public class StackInfo
    {
        public string Breviary;
        public int Line;

        public StackInfo()
        {
            this.Breviary = "";
            this.Line = 1;
        }

        public StackInfo(string breviary, int line)
        {
            this.Breviary = "";
            this.Line = 1;
            this.Breviary = breviary;
            this.Line = line;
        }
    }
}

