namespace Scorpio.Serialize
{
    using Scorpio;
    using Scorpio.Compiler;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ScorpioMaker
    {
        private static sbyte LineFlag = 0x7f;

        public static List<Token> Deserialize(byte[] data)
        {
            List<Token> list = new List<Token>();
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadSByte();
                    int num = reader.ReadInt32();
                    int num2 = 0;
                    for (int i = 0; i < num; i++)
                    {
                        sbyte num4 = reader.ReadSByte();
                        if (num4 == LineFlag)
                        {
                            num2 = reader.ReadInt32();
                            num4 = reader.ReadSByte();
                        }
                        Scorpio.Compiler.TokenType tokenType = (Scorpio.Compiler.TokenType) num4;
                        object lexeme = null;
                        switch (tokenType)
                        {
                            case Scorpio.Compiler.TokenType.Boolean:
                                lexeme = reader.ReadSByte() == 1;
                                goto Label_00DC;

                            case Scorpio.Compiler.TokenType.Number:
                                if (reader.ReadSByte() != 1)
                                {
                                    break;
                                }
                                lexeme = reader.ReadDouble();
                                goto Label_00DC;

                            case Scorpio.Compiler.TokenType.String:
                            case Scorpio.Compiler.TokenType.SimpleString:
                                lexeme = Util.ReadString(reader);
                                goto Label_00DC;

                            case Scorpio.Compiler.TokenType.Identifier:
                                lexeme = Util.ReadString(reader);
                                goto Label_00DC;

                            default:
                                lexeme = tokenType.ToString();
                                goto Label_00DC;
                        }
                        lexeme = reader.ReadInt64();
                    Label_00DC:
                        list.Add(new Token(tokenType, lexeme, num2 - 1, 0));
                    }
                }
            }
            return list;
        }

        public static string DeserializeToString(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadSByte();
                    int num = reader.ReadInt32();
                    for (int i = 0; i < num; i++)
                    {
                        sbyte num3 = reader.ReadSByte();
                        if (num3 == LineFlag)
                        {
                            int num4 = reader.ReadInt32();
                            num3 = reader.ReadSByte();
                            int length = builder.ToString().Split(new char[] { '\n' }).Length;
                            for (int j = length; j < num4; j++)
                            {
                                builder.Append('\n');
                            }
                        }
                        Scorpio.Compiler.TokenType type = (Scorpio.Compiler.TokenType) num3;
                        object tokenString = null;
                        switch (type)
                        {
                            case Scorpio.Compiler.TokenType.Boolean:
                                tokenString = (reader.ReadSByte() == 1) ? "true" : "false";
                                goto Label_015C;

                            case Scorpio.Compiler.TokenType.Number:
                                if (reader.ReadSByte() != 1)
                                {
                                    break;
                                }
                                tokenString = reader.ReadDouble();
                                goto Label_015C;

                            case Scorpio.Compiler.TokenType.String:
                                tokenString = "\"" + Util.ReadString(reader).Replace("\n", @"\n") + "\"";
                                goto Label_015C;

                            case Scorpio.Compiler.TokenType.SimpleString:
                                tokenString = "@\"" + Util.ReadString(reader) + "\"";
                                goto Label_015C;

                            case Scorpio.Compiler.TokenType.Identifier:
                                tokenString = Util.ReadString(reader);
                                goto Label_015C;

                            default:
                                tokenString = GetTokenString(type);
                                goto Label_015C;
                        }
                        tokenString = reader.ReadInt64() + "L";
                    Label_015C:
                        builder.Append(tokenString + " ");
                    }
                }
            }
            return builder.ToString();
        }

        private static string GetTokenString(Scorpio.Compiler.TokenType type)
        {
            switch (type)
            {
                case Scorpio.Compiler.TokenType.Var:
                    return "var";

                case Scorpio.Compiler.TokenType.LeftBrace:
                    return "{";

                case Scorpio.Compiler.TokenType.RightBrace:
                    return "}";

                case Scorpio.Compiler.TokenType.LeftPar:
                    return "(";

                case Scorpio.Compiler.TokenType.RightPar:
                    return ")";

                case Scorpio.Compiler.TokenType.LeftBracket:
                    return "[";

                case Scorpio.Compiler.TokenType.RightBracket:
                    return "]";

                case Scorpio.Compiler.TokenType.Period:
                    return ".";

                case Scorpio.Compiler.TokenType.Comma:
                    return ",";

                case Scorpio.Compiler.TokenType.Colon:
                    return ":";

                case Scorpio.Compiler.TokenType.SemiColon:
                    return ";";

                case Scorpio.Compiler.TokenType.QuestionMark:
                    return "?";

                case Scorpio.Compiler.TokenType.Sharp:
                    return "#";

                case Scorpio.Compiler.TokenType.Plus:
                    return "+";

                case Scorpio.Compiler.TokenType.Increment:
                    return "++";

                case Scorpio.Compiler.TokenType.AssignPlus:
                    return "+=";

                case Scorpio.Compiler.TokenType.Minus:
                    return "-";

                case Scorpio.Compiler.TokenType.Decrement:
                    return "--";

                case Scorpio.Compiler.TokenType.AssignMinus:
                    return "-=";

                case Scorpio.Compiler.TokenType.Multiply:
                    return "*";

                case Scorpio.Compiler.TokenType.AssignMultiply:
                    return "*=";

                case Scorpio.Compiler.TokenType.Divide:
                    return "/";

                case Scorpio.Compiler.TokenType.AssignDivide:
                    return "/=";

                case Scorpio.Compiler.TokenType.Modulo:
                    return "%";

                case Scorpio.Compiler.TokenType.AssignModulo:
                    return "%=";

                case Scorpio.Compiler.TokenType.InclusiveOr:
                    return "|";

                case Scorpio.Compiler.TokenType.AssignInclusiveOr:
                    return "|=";

                case Scorpio.Compiler.TokenType.Or:
                    return "||";

                case Scorpio.Compiler.TokenType.Combine:
                    return "&";

                case Scorpio.Compiler.TokenType.AssignCombine:
                    return "&=";

                case Scorpio.Compiler.TokenType.And:
                    return "&&";

                case Scorpio.Compiler.TokenType.XOR:
                    return "^";

                case Scorpio.Compiler.TokenType.AssignXOR:
                    return "^=";

                case Scorpio.Compiler.TokenType.Negative:
                    return "~";

                case Scorpio.Compiler.TokenType.Shi:
                    return "<<";

                case Scorpio.Compiler.TokenType.AssignShi:
                    return "<<=";

                case Scorpio.Compiler.TokenType.Shr:
                    return ">>";

                case Scorpio.Compiler.TokenType.AssignShr:
                    return ">>=";

                case Scorpio.Compiler.TokenType.Not:
                    return "!";

                case Scorpio.Compiler.TokenType.Assign:
                    return "=";

                case Scorpio.Compiler.TokenType.Equal:
                    return "==";

                case Scorpio.Compiler.TokenType.NotEqual:
                    return "!=";

                case Scorpio.Compiler.TokenType.Greater:
                    return ">";

                case Scorpio.Compiler.TokenType.GreaterOrEqual:
                    return ">=";

                case Scorpio.Compiler.TokenType.Less:
                    return "<";

                case Scorpio.Compiler.TokenType.LessOrEqual:
                    return "<=";

                case Scorpio.Compiler.TokenType.Params:
                    return "...";

                case Scorpio.Compiler.TokenType.If:
                    return "if";

                case Scorpio.Compiler.TokenType.Else:
                    return "else";

                case Scorpio.Compiler.TokenType.ElseIf:
                    return "elif";

                case Scorpio.Compiler.TokenType.Ifndef:
                    return "ifndef";

                case Scorpio.Compiler.TokenType.Endif:
                    return "endif";

                case Scorpio.Compiler.TokenType.For:
                    return "for";

                case Scorpio.Compiler.TokenType.Foreach:
                    return "foreach";

                case Scorpio.Compiler.TokenType.In:
                    return "in";

                case Scorpio.Compiler.TokenType.Switch:
                    return "switch";

                case Scorpio.Compiler.TokenType.Case:
                    return "case";

                case Scorpio.Compiler.TokenType.Default:
                    return "default";

                case Scorpio.Compiler.TokenType.Break:
                    return "break";

                case Scorpio.Compiler.TokenType.Continue:
                    return "continue";

                case Scorpio.Compiler.TokenType.Return:
                    return "return";

                case Scorpio.Compiler.TokenType.While:
                    return "while";

                case Scorpio.Compiler.TokenType.Function:
                    return "function";

                case Scorpio.Compiler.TokenType.Try:
                    return "try";

                case Scorpio.Compiler.TokenType.Catch:
                    return "catch";

                case Scorpio.Compiler.TokenType.Throw:
                    return "throw";

                case Scorpio.Compiler.TokenType.Define:
                    return "define";

                case Scorpio.Compiler.TokenType.Null:
                    return "null";

                case Scorpio.Compiler.TokenType.Eval:
                    return "eval";
            }
            return "";
        }

        public static byte[] Serialize(string breviary, string data)
        {
            List<Token> tokens = new ScriptLexer(data, breviary).GetTokens();
            if (tokens.Count == 0)
            {
                return new byte[0];
            }
            int sourceLine = 0;
            byte[] buffer = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((sbyte) 0);
                    writer.Write(tokens.Count);
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        Token token = tokens[i];
                        if (sourceLine != token.SourceLine)
                        {
                            sourceLine = token.SourceLine;
                            writer.Write(LineFlag);
                            writer.Write(token.SourceLine);
                        }
                        writer.Write((sbyte) token.Type);
                        switch (token.Type)
                        {
                            case Scorpio.Compiler.TokenType.Boolean:
                            {
                                writer.Write(((bool) token.Lexeme) ? ((sbyte) 1) : ((sbyte) 0));
                                continue;
                            }
                            case Scorpio.Compiler.TokenType.Number:
                            {
                                if (!(token.Lexeme is double))
                                {
                                    break;
                                }
                                writer.Write((sbyte) 1);
                                writer.Write((double) token.Lexeme);
                                continue;
                            }
                            case Scorpio.Compiler.TokenType.String:
                            case Scorpio.Compiler.TokenType.SimpleString:
                            {
                                Util.WriteString(writer, (string) token.Lexeme);
                                continue;
                            }
                            case Scorpio.Compiler.TokenType.Null:
                            case Scorpio.Compiler.TokenType.Eval:
                            {
                                continue;
                            }
                            case Scorpio.Compiler.TokenType.Identifier:
                            {
                                Util.WriteString(writer, (string) token.Lexeme);
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                        writer.Write((sbyte) 2);
                        writer.Write((long) token.Lexeme);
                    }
                    buffer = stream.ToArray();
                }
            }
            return buffer;
        }
    }
}

