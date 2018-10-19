namespace Scorpio.Runtime
{
    using System;

    public enum Opcode
    {
        MOV,
        VAR,
        CALL_BLOCK,
        CALL_IF,

        CALL_FOR,
        CALL_FORSIMPLE,
        CALL_FOREACH,
        CALL_WHILE,

        CALL_SWITCH,
        CALL_TRY,
        CALL_FUNCTION,

        THROW,
        RESOLVE,
        RET,
        BREAK,
        CONTINUE
    }
}

