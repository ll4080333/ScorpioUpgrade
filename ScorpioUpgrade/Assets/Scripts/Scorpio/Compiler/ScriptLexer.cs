namespace Scorpio.Compiler
{
    using Scorpio;
    using Scorpio.Exception;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class ScriptLexer
    {
        private const int BREVIARY_CHAR = 20;
        private char ch;
        private int m_iCacheLine;
        private int m_iSourceChar;
        private int m_iSourceLine;
        private LexState m_lexState;
        private List<string> m_listSourceLines = new List<string>();
        private List<Token> m_listTokens = new List<Token>();
        private string m_strBreviary;
        private string m_strToken;

        public ScriptLexer(string buffer, string strBreviary)
        {
            string[] strArray = buffer.Split(new char[] { '\n' });
            if (Util.IsNullOrEmpty(strBreviary))
            {
                this.m_strBreviary = (strArray.Length > 0) ? strArray[0] : "";
                if (this.m_strBreviary.Length > 20)
                {
                    this.m_strBreviary = this.m_strBreviary.Substring(0, 20);
                }
            }
            else
            {
                this.m_strBreviary = strBreviary;
            }
            foreach (string str in strArray)
            {
                this.m_listSourceLines.Add(str + '\n');
            }
            this.m_iSourceLine = 0;
            this.m_iSourceChar = 0;
            this.lexState = LexState.None;
        }

        private void AddToken(Scorpio.Compiler.TokenType type)
        {
            this.AddToken(type, this.ch);
        }

        private void AddToken(Scorpio.Compiler.TokenType type, object lexeme)
        {
            this.m_listTokens.Add(new Token(type, lexeme, this.m_iSourceLine, this.m_iSourceChar));
            this.lexState = LexState.None;
        }

        public string GetBreviary()
        {
            return this.m_strBreviary;
        }

        public List<Token> GetTokens()
        {
            this.m_iSourceLine = 0;
            this.m_iSourceChar = 0;
            this.lexState = LexState.None;
            this.m_listTokens.Clear();
            while (!this.EndOfSource)
            {
                Scorpio.Compiler.TokenType eval;
                char ch;
                if (this.EndOfLine)
                {
                    this.IgnoreLine();
                }
                else
                {
                    this.ch = this.ReadChar();
                    switch (this.lexState)
                    {
                        case LexState.None:
                            ch = this.ch;
                            if (ch > '@')
                            {
                                goto Label_019E;
                            }
                            switch (ch)
                            {
                                case '\t':
                                case '\n':
                                case '\r':
                                case ' ':
                                {
                                    continue;
                                }
                                case '!':
                                    goto Label_02DE;

                                case '"':
                                    goto Label_031F;

                                case '#':
                                    goto Label_0253;

                                case '%':
                                    goto Label_02AB;

                                case '&':
                                    goto Label_02C4;

                                case '\'':
                                    goto Label_032C;

                                case '(':
                                    goto Label_01D7;

                                case ')':
                                    goto Label_01E3;

                                case '*':
                                    goto Label_0292;

                                case '+':
                                    goto Label_0279;

                                case ',':
                                    goto Label_021F;

                                case '-':
                                    goto Label_0285;

                                case '.':
                                    goto Label_026D;

                                case '/':
                                    goto Label_029F;

                                case ':':
                                    goto Label_022C;

                                case ';':
                                    goto Label_0239;

                                case '<':
                                    goto Label_02F8;

                                case '=':
                                    goto Label_02B8;

                                case '>':
                                    goto Label_02EB;

                                case '?':
                                    goto Label_0246;

                                case '@':
                                    goto Label_0312;
                            }
                            goto Label_0339;

                        case LexState.AssignOrEqual:
                            if (this.ch != '=')
                            {
                                goto Label_05FD;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.Equal, "==");
                            break;

                        case LexState.CommentOrDivideOrAssignDivide:
                            switch (this.ch)
                            {
                                case '*':
                                    goto Label_0523;

                                case '/':
                                    goto Label_0517;

                                case '=':
                                    goto Label_052F;
                            }
                            goto Label_0541;

                        case LexState.LineComment:
                            if (this.ch == '\n')
                            {
                                this.lexState = LexState.None;
                            }
                            break;

                        case LexState.BlockCommentStart:
                            if (this.ch == '*')
                            {
                                this.lexState = LexState.BlockCommentEnd;
                            }
                            break;

                        case LexState.BlockCommentEnd:
                            if (this.ch != '/')
                            {
                                goto Label_05D5;
                            }
                            this.lexState = LexState.None;
                            break;

                        case LexState.PeriodOrParams:
                            if (this.ch != '.')
                            {
                                goto Label_03E3;
                            }
                            this.lexState = LexState.Params;
                            break;

                        case LexState.Params:
                            if (this.ch != '.')
                            {
                                goto Label_0416;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.Params, "...");
                            break;

                        case LexState.PlusOrIncrementOrAssignPlus:
                            if (this.ch != '+')
                            {
                                goto Label_0443;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.Increment, "++");
                            break;

                        case LexState.MinusOrDecrementOrAssignMinus:
                            if (this.ch != '-')
                            {
                                goto Label_0493;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.Decrement, "--");
                            break;

                        case LexState.MultiplyOrAssignMultiply:
                            if (this.ch != '=')
                            {
                                goto Label_04E3;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.AssignMultiply, "*=");
                            break;

                        case LexState.ModuloOrAssignModulo:
                            if (this.ch != '=')
                            {
                                goto Label_0575;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.AssignModulo, "%=");
                            break;

                        case LexState.AndOrCombine:
                            if (this.ch != '&')
                            {
                                goto Label_0631;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.And, "&&");
                            break;

                        case LexState.OrOrInclusiveOr:
                            if (this.ch != '|')
                            {
                                goto Label_0681;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.Or, "||");
                            break;

                        case LexState.XorOrAssignXor:
                            if (this.ch != '=')
                            {
                                goto Label_06D1;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.AssignXOR, "^=");
                            break;

                        case LexState.ShiOrAssignShi:
                            if (this.ch != '=')
                            {
                                goto Label_07CF;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.AssignShi, "<<=");
                            break;

                        case LexState.ShrOrAssignShr:
                            if (this.ch != '=')
                            {
                                goto Label_079B;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.AssignShr, ">>=");
                            break;

                        case LexState.NotOrNotEqual:
                            if (this.ch != '=')
                            {
                                goto Label_0803;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.NotEqual, "!=");
                            break;

                        case LexState.GreaterOrGreaterEqual:
                            if (this.ch != '=')
                            {
                                goto Label_0705;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.GreaterOrEqual, ">=");
                            break;

                        case LexState.LessOrLessEqual:
                            if (this.ch != '=')
                            {
                                goto Label_0750;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.LessOrEqual, "<=");
                            break;

                        case LexState.String:
                            if (this.ch != '"')
                            {
                                goto Label_0838;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.String, this.m_strToken);
                            break;

                        case LexState.StringEscape:
                            if ((this.ch != '\\') && (this.ch != '"'))
                            {
                                goto Label_08D2;
                            }
                            this.m_strToken = this.m_strToken + this.ch;
                            this.lexState = LexState.String;
                            break;

                        case LexState.SingleString:
                            if (this.ch != '\'')
                            {
                                goto Label_09A5;
                            }
                            this.AddToken(Scorpio.Compiler.TokenType.String, this.m_strToken);
                            break;

                        case LexState.SingleStringEscape:
                            if ((this.ch != '\\') && (this.ch != '\''))
                            {
                                goto Label_0A3F;
                            }
                            this.m_strToken = this.m_strToken + this.ch;
                            this.lexState = LexState.SingleString;
                            break;

                        case LexState.SimpleStringStart:
                            if (this.ch != '"')
                            {
                                goto Label_0B18;
                            }
                            this.m_iCacheLine = this.m_iSourceLine;
                            this.lexState = LexState.SimpleString;
                            break;

                        case LexState.SimpleString:
                            if (this.ch != '"')
                            {
                                goto Label_0B63;
                            }
                            this.lexState = LexState.SimpleStringQuotationMarkOrOver;
                            break;

                        case LexState.SimpleStringQuotationMarkOrOver:
                            if (this.ch != '"')
                            {
                                goto Label_0BB3;
                            }
                            this.m_strToken = this.m_strToken + '"';
                            this.lexState = LexState.SimpleString;
                            break;

                        case LexState.SingleSimpleString:
                            if (this.ch != '\'')
                            {
                                goto Label_0C00;
                            }
                            this.lexState = LexState.SingleSimpleStringQuotationMarkOrOver;
                            break;

                        case LexState.SingleSimpleStringQuotationMarkOrOver:
                            if (this.ch != '\'')
                            {
                                goto Label_0C50;
                            }
                            this.m_strToken = this.m_strToken + '\'';
                            this.lexState = LexState.SingleSimpleString;
                            break;

                        case LexState.NumberOrHexNumber:
                            if (this.ch != 'x')
                            {
                                goto Label_0C9D;
                            }
                            this.lexState = LexState.HexNumber;
                            break;

                        case LexState.Number:
                            if (!char.IsDigit(this.ch) && (this.ch != '.'))
                            {
                                goto Label_0CF3;
                            }
                            this.m_strToken = this.m_strToken + this.ch;
                            break;

                        case LexState.HexNumber:
                            if (!this.IsHexDigit(this.ch))
                            {
                                goto Label_0D70;
                            }
                            this.m_strToken = this.m_strToken + this.ch;
                            break;

                        case LexState.Identifier:
                            if (!this.IsIdentifier(this.ch))
                            {
                                goto Label_0DE2;
                            }
                            this.m_strToken = this.m_strToken + this.ch;
                            break;
                    }
                }
                continue;
            Label_019E:
                switch (ch)
                {
                    case '[':
                    {
                        this.AddToken(Scorpio.Compiler.TokenType.LeftBracket);
                        continue;
                    }
                    case '\\':
                        goto Label_0339;

                    case ']':
                    {
                        this.AddToken(Scorpio.Compiler.TokenType.RightBracket);
                        continue;
                    }
                    case '^':
                    {
                        this.lexState = LexState.XorOrAssignXor;
                        continue;
                    }
                    default:
                        switch (ch)
                        {
                            case '{':
                            {
                                this.AddToken(Scorpio.Compiler.TokenType.LeftBrace);
                                continue;
                            }
                            case '|':
                            {
                                this.lexState = LexState.OrOrInclusiveOr;
                                continue;
                            }
                            case '}':
                            {
                                this.AddToken(Scorpio.Compiler.TokenType.RightBrace);
                                continue;
                            }
                            case '~':
                            {
                                this.AddToken(Scorpio.Compiler.TokenType.Negative);
                                continue;
                            }
                        }
                        goto Label_0339;
                }
            Label_01D7:
                this.AddToken(Scorpio.Compiler.TokenType.LeftPar);
                continue;
            Label_01E3:
                this.AddToken(Scorpio.Compiler.TokenType.RightPar);
                continue;
            Label_021F:
                this.AddToken(Scorpio.Compiler.TokenType.Comma);
                continue;
            Label_022C:
                this.AddToken(Scorpio.Compiler.TokenType.Colon);
                continue;
            Label_0239:
                this.AddToken(Scorpio.Compiler.TokenType.SemiColon);
                continue;
            Label_0246:
                this.AddToken(Scorpio.Compiler.TokenType.QuestionMark);
                continue;
            Label_0253:
                this.AddToken(Scorpio.Compiler.TokenType.Sharp);
                continue;
            Label_026D:
                this.lexState = LexState.PeriodOrParams;
                continue;
            Label_0279:
                this.lexState = LexState.PlusOrIncrementOrAssignPlus;
                continue;
            Label_0285:
                this.lexState = LexState.MinusOrDecrementOrAssignMinus;
                continue;
            Label_0292:
                this.lexState = LexState.MultiplyOrAssignMultiply;
                continue;
            Label_029F:
                this.lexState = LexState.CommentOrDivideOrAssignDivide;
                continue;
            Label_02AB:
                this.lexState = LexState.ModuloOrAssignModulo;
                continue;
            Label_02B8:
                this.lexState = LexState.AssignOrEqual;
                continue;
            Label_02C4:
                this.lexState = LexState.AndOrCombine;
                continue;
            Label_02DE:
                this.lexState = LexState.NotOrNotEqual;
                continue;
            Label_02EB:
                this.lexState = LexState.GreaterOrGreaterEqual;
                continue;
            Label_02F8:
                this.lexState = LexState.LessOrLessEqual;
                continue;
            Label_0312:
                this.lexState = LexState.SimpleStringStart;
                continue;
            Label_031F:
                this.lexState = LexState.String;
                continue;
            Label_032C:
                this.lexState = LexState.SingleString;
                continue;
            Label_0339:
                if (this.ch == '0')
                {
                    this.lexState = LexState.NumberOrHexNumber;
                    this.m_strToken = "";
                }
                else if (char.IsDigit(this.ch))
                {
                    this.lexState = LexState.Number;
                    this.m_strToken = this.ch.ToString();
                }
                else if (this.IsIdentifier(this.ch))
                {
                    this.lexState = LexState.Identifier;
                    this.m_strToken = this.ch.ToString();
                }
                else
                {
                    this.ThrowInvalidCharacterException(this.ch);
                }
                continue;
            Label_03E3:
                this.AddToken(Scorpio.Compiler.TokenType.Period, ".");
                this.UndoChar();
                continue;
            Label_0416:
                this.ThrowInvalidCharacterException(this.ch);
                continue;
            Label_0443:
                if (this.ch == '=')
                {
                    this.AddToken(Scorpio.Compiler.TokenType.AssignPlus, "+=");
                }
                else
                {
                    this.AddToken(Scorpio.Compiler.TokenType.Plus, "+");
                    this.UndoChar();
                }
                continue;
            Label_0493:
                if (this.ch == '=')
                {
                    this.AddToken(Scorpio.Compiler.TokenType.AssignMinus, "-=");
                }
                else
                {
                    this.AddToken(Scorpio.Compiler.TokenType.Minus, "-");
                    this.UndoChar();
                }
                continue;
            Label_04E3:
                this.AddToken(Scorpio.Compiler.TokenType.Multiply, "*");
                this.UndoChar();
                continue;
            Label_0517:
                this.lexState = LexState.LineComment;
                continue;
            Label_0523:
                this.lexState = LexState.BlockCommentStart;
                continue;
            Label_052F:
                this.AddToken(Scorpio.Compiler.TokenType.AssignDivide, "/=");
                continue;
            Label_0541:
                this.AddToken(Scorpio.Compiler.TokenType.Divide, "/");
                this.UndoChar();
                continue;
            Label_0575:
                this.AddToken(Scorpio.Compiler.TokenType.Modulo, "%");
                this.UndoChar();
                continue;
            Label_05D5:
                this.lexState = LexState.BlockCommentStart;
                continue;
            Label_05FD:
                this.AddToken(Scorpio.Compiler.TokenType.Assign, "=");
                this.UndoChar();
                continue;
            Label_0631:
                if (this.ch == '=')
                {
                    this.AddToken(Scorpio.Compiler.TokenType.AssignCombine, "&=");
                }
                else
                {
                    this.AddToken(Scorpio.Compiler.TokenType.Combine, "&");
                    this.UndoChar();
                }
                continue;
            Label_0681:
                if (this.ch == '=')
                {
                    this.AddToken(Scorpio.Compiler.TokenType.AssignInclusiveOr, "|=");
                }
                else
                {
                    this.AddToken(Scorpio.Compiler.TokenType.InclusiveOr, "|");
                    this.UndoChar();
                }
                continue;
            Label_06D1:
                this.AddToken(Scorpio.Compiler.TokenType.XOR, "^");
                this.UndoChar();
                continue;
            Label_0705:
                if (this.ch == '>')
                {
                    this.lexState = LexState.ShrOrAssignShr;
                }
                else
                {
                    this.AddToken(Scorpio.Compiler.TokenType.Greater, ">");
                    this.UndoChar();
                }
                continue;
            Label_0750:
                if (this.ch == '<')
                {
                    this.lexState = LexState.ShiOrAssignShi;
                }
                else
                {
                    this.AddToken(Scorpio.Compiler.TokenType.Less, "<");
                    this.UndoChar();
                }
                continue;
            Label_079B:
                this.AddToken(Scorpio.Compiler.TokenType.Shr, ">>");
                this.UndoChar();
                continue;
            Label_07CF:
                this.AddToken(Scorpio.Compiler.TokenType.Shi, "<<");
                this.UndoChar();
                continue;
            Label_0803:
                this.AddToken(Scorpio.Compiler.TokenType.Not, "!");
                this.UndoChar();
                continue;
            Label_0838:
                if (this.ch == '\\')
                {
                    this.lexState = LexState.StringEscape;
                }
                else if ((this.ch == '\r') || (this.ch == '\n'))
                {
                    this.ThrowInvalidCharacterException(this.ch);
                }
                else
                {
                    this.m_strToken = this.m_strToken + this.ch;
                }
                continue;
            Label_08D2:
                if (this.ch == 't')
                {
                    this.m_strToken = this.m_strToken + '\t';
                    this.lexState = LexState.String;
                }
                else if (this.ch == 'r')
                {
                    this.m_strToken = this.m_strToken + '\r';
                    this.lexState = LexState.String;
                }
                else if (this.ch == 'n')
                {
                    this.m_strToken = this.m_strToken + '\n';
                    this.lexState = LexState.String;
                }
                else
                {
                    this.m_strToken = this.m_strToken + this.ch;
                    this.lexState = LexState.String;
                }
                continue;
            Label_09A5:
                if (this.ch == '\\')
                {
                    this.lexState = LexState.SingleStringEscape;
                }
                else if ((this.ch == '\r') || (this.ch == '\n'))
                {
                    this.ThrowInvalidCharacterException(this.ch);
                }
                else
                {
                    this.m_strToken = this.m_strToken + this.ch;
                }
                continue;
            Label_0A3F:
                if (this.ch == 't')
                {
                    this.m_strToken = this.m_strToken + '\t';
                    this.lexState = LexState.SingleString;
                }
                else if (this.ch == 'r')
                {
                    this.m_strToken = this.m_strToken + '\r';
                    this.lexState = LexState.SingleString;
                }
                else if (this.ch == 'n')
                {
                    this.m_strToken = this.m_strToken + '\n';
                    this.lexState = LexState.SingleString;
                }
                else
                {
                    this.m_strToken = this.m_strToken + this.ch;
                    this.lexState = LexState.SingleString;
                }
                continue;
            Label_0B18:
                if (this.ch == '\'')
                {
                    this.m_iCacheLine = this.m_iSourceLine;
                    this.lexState = LexState.SingleSimpleString;
                }
                else
                {
                    this.ThrowInvalidCharacterException(this.ch);
                }
                continue;
            Label_0B63:
                this.m_strToken = this.m_strToken + this.ch;
                continue;
            Label_0BB3:
                this.m_listTokens.Add(new Token(Scorpio.Compiler.TokenType.SimpleString, this.m_strToken, this.m_iCacheLine, this.m_iSourceChar));
                this.lexState = LexState.None;
                this.UndoChar();
                continue;
            Label_0C00:
                this.m_strToken = this.m_strToken + this.ch;
                continue;
            Label_0C50:
                this.m_listTokens.Add(new Token(Scorpio.Compiler.TokenType.SimpleString, this.m_strToken, this.m_iCacheLine, this.m_iSourceChar));
                this.lexState = LexState.None;
                this.UndoChar();
                continue;
            Label_0C9D:
                this.m_strToken = "0";
                this.lexState = LexState.Number;
                this.UndoChar();
                continue;
            Label_0CF3:
                if (this.ch == 'L')
                {
                    long num = long.Parse(this.m_strToken);
                    this.AddToken(Scorpio.Compiler.TokenType.Number, num);
                }
                else
                {
                    double num2 = double.Parse(this.m_strToken);
                    this.AddToken(Scorpio.Compiler.TokenType.Number, num2);
                    this.UndoChar();
                }
                continue;
            Label_0D70:
                if (Util.IsNullOrEmpty(this.m_strToken))
                {
                    this.ThrowInvalidCharacterException(this.ch);
                }
                long lexeme = long.Parse(this.m_strToken, NumberStyles.HexNumber);
                this.AddToken(Scorpio.Compiler.TokenType.Number, lexeme);
                this.UndoChar();
                continue;
            Label_0DE2:
                switch (this.m_strToken)
                {
                    case "eval":
                        eval = Scorpio.Compiler.TokenType.Eval;
                        break;

                    case "var":
                    case "local":
                        eval = Scorpio.Compiler.TokenType.Var;
                        break;

                    case "function":
                        eval = Scorpio.Compiler.TokenType.Function;
                        break;

                    case "if":
                        eval = Scorpio.Compiler.TokenType.If;
                        break;

                    case "elseif":
                    case "elif":
                        eval = Scorpio.Compiler.TokenType.ElseIf;
                        break;

                    case "else":
                        eval = Scorpio.Compiler.TokenType.Else;
                        break;

                    case "while":
                        eval = Scorpio.Compiler.TokenType.While;
                        break;

                    case "for":
                        eval = Scorpio.Compiler.TokenType.For;
                        break;

                    case "foreach":
                        eval = Scorpio.Compiler.TokenType.Foreach;
                        break;

                    case "in":
                        eval = Scorpio.Compiler.TokenType.In;
                        break;

                    case "switch":
                        eval = Scorpio.Compiler.TokenType.Switch;
                        break;

                    case "case":
                        eval = Scorpio.Compiler.TokenType.Case;
                        break;

                    case "default":
                        eval = Scorpio.Compiler.TokenType.Default;
                        break;

                    case "try":
                        eval = Scorpio.Compiler.TokenType.Try;
                        break;

                    case "catch":
                        eval = Scorpio.Compiler.TokenType.Catch;
                        break;

                    case "throw":
                        eval = Scorpio.Compiler.TokenType.Throw;
                        break;

                    case "continue":
                        eval = Scorpio.Compiler.TokenType.Continue;
                        break;

                    case "break":
                        eval = Scorpio.Compiler.TokenType.Break;
                        break;

                    case "return":
                        eval = Scorpio.Compiler.TokenType.Return;
                        break;

                    case "define":
                        eval = Scorpio.Compiler.TokenType.Define;
                        break;

                    case "ifndef":
                        eval = Scorpio.Compiler.TokenType.Ifndef;
                        break;

                    case "endif":
                        eval = Scorpio.Compiler.TokenType.Endif;
                        break;

                    case "null":
                    case "nil":
                        eval = Scorpio.Compiler.TokenType.Null;
                        break;

                    case "true":
                    case "false":
                        eval = Scorpio.Compiler.TokenType.Boolean;
                        break;

                    case "new":
                        eval = Scorpio.Compiler.TokenType.None;
                        break;

                    //case "name":
                    //    eval = Scorpio.Compiler.TokenType.Name;
                    //    break;

                    default:
                        eval = Scorpio.Compiler.TokenType.Identifier;
                        break;
                }
                switch (eval)
                {
                    case Scorpio.Compiler.TokenType.Boolean:
                        this.m_listTokens.Add(new Token(eval, this.m_strToken == "true", this.m_iSourceLine, this.m_iSourceChar));
                        break;
                    case Scorpio.Compiler.TokenType.Null:
                        this.m_listTokens.Add(new Token(eval, null, this.m_iSourceLine, this.m_iSourceChar));
                        break;

                        // TODO 修复为提示，对用户录入的值进行覆盖处理
                    //case TokenType.Name:
                    //    throw new ScriptException("Can't define [name] field,[name] is scorpio fixed field(readonly)");

                    default:
                        if (eval != Scorpio.Compiler.TokenType.None)
                            this.m_listTokens.Add(new Token(eval,this.m_strToken,this.m_iSourceLine,this.m_iSourceChar));
                        break;
                }
                this.UndoChar();
                this.lexState = LexState.None;
            }
            this.m_listTokens.Add(new Token(Scorpio.Compiler.TokenType.Finished, "", this.m_iSourceLine, this.m_iSourceChar));

            // TODO 检查是否插入Prototype表
            // TODO 检查是否拥有Name字段，有则抛出异常（语法不正确所致）, 检查通过则 为表添加name字段

            return this.m_listTokens;
        }

        private void IgnoreLine()
        {
            this.m_iSourceLine++;
            this.m_iSourceChar = 0;
        }

        private bool IsHexDigit(char c)
        {
            return (char.IsDigit(c) || ((('a' <= c) && (c <= 'f')) || (('A' <= c) && (c <= 'F'))));
        }

        private bool IsIdentifier(char ch)
        {
            if (ch != '_')
            {
                return char.IsLetterOrDigit(ch);
            }
            return true;
        }

        private char ReadChar()
        {
            if (this.EndOfSource)
            {
                throw new LexerException("End of source reached.");
            }
            char ch = this.m_listSourceLines[this.m_iSourceLine][this.m_iSourceChar++];
            if (this.m_iSourceChar >= this.m_listSourceLines[this.m_iSourceLine].Length)
            {
                this.m_iSourceChar = 0;
                this.m_iSourceLine++;
            }
            return ch;
        }

        private void ThrowInvalidCharacterException(char ch)
        {
            throw new LexerException(string.Concat(new object[] { this.m_strBreviary, ":", this.m_iSourceLine + 1, "  Unexpected character [", ch, "]  Line:", this.m_iSourceLine + 1, " Column:", this.m_iSourceChar, " [", this.m_listSourceLines[this.m_iSourceLine], "]" }));
        }

        private void UndoChar()
        {
            if ((this.m_iSourceLine == 0) && (this.m_iSourceChar == 0))
            {
                throw new LexerException("Cannot undo char beyond start of source.");
            }
            this.m_iSourceChar--;
            if (this.m_iSourceChar < 0)
            {
                this.m_iSourceLine--;
                this.m_iSourceChar = this.m_listSourceLines[this.m_iSourceLine].Length - 1;
            }
        }

        private bool EndOfLine
        {
            get
            {
                return (this.m_iSourceChar >= this.m_listSourceLines[this.m_iSourceLine].Length);
            }
        }

        private bool EndOfSource
        {
            get
            {
                return (this.m_iSourceLine >= this.m_listSourceLines.Count);
            }
        }

        private LexState lexState
        {
            get
            {
                return this.m_lexState;
            }
            set
            {
                this.m_lexState = value;
                if (this.m_lexState == LexState.None)
                {
                    this.m_strToken = "";
                }
            }
        }

        private enum LexState
        {
            /// <summary>
            /// 
            /// </summary>
            None,
            /// <summary>
            /// 分配 | 赋值
            /// </summary>
            AssignOrEqual,
            /// <summary>
            /// 注释 | 除法 | ???
            /// </summary>
            CommentOrDivideOrAssignDivide,
            /// <summary>
            /// 单行注释
            /// </summary>
            LineComment,
            /// <summary>
            /// 块注释头
            /// </summary>
            BlockCommentStart,
            /// <summary>
            /// 块注释尾
            /// </summary>
            BlockCommentEnd,
            /// <summary>
            /// 
            /// </summary>
            PeriodOrParams,
            /// <summary>
            /// 参数
            /// </summary>
            Params,
            /// <summary>
            /// 加 | 递增 | ???
            /// </summary>
            PlusOrIncrementOrAssignPlus,
            /// <summary>
            /// 减 | 递减 | ???
            /// </summary>
            MinusOrDecrementOrAssignMinus,
            /// <summary>
            /// 乘法 | *???
            /// </summary>
            MultiplyOrAssignMultiply,
            /// <summary>
            /// 模运算 | ???
            /// </summary>
            ModuloOrAssignModulo,
            /// <summary>
            /// 与
            /// </summary>
            AndOrCombine,
            /// <summary>
            /// 或
            /// </summary>
            OrOrInclusiveOr,
            /// <summary>
            /// 异或
            /// </summary>
            XorOrAssignXor,
            /// <summary>
            /// 位移运算符 <<
            /// </summary>
            ShiOrAssignShi,
            /// <summary>
            /// 位移运算符 >>
            /// </summary>
            ShrOrAssignShr,
            /// <summary>
            /// 非 | 不等于
            /// </summary>
            NotOrNotEqual,
            /// <summary>
            /// 大于等于
            /// </summary>
            GreaterOrGreaterEqual,
            /// <summary>
            /// 小于等于
            /// </summary>
            LessOrLessEqual,
            /// <summary>
            /// 字符串
            /// </summary>
            String,
            /// <summary>
            /// 字符串尾
            /// </summary>
            StringEscape,
            /// <summary>
            /// 字符串头
            /// </summary>
            SingleString,
            /// <summary>
            /// 单字符串尾
            /// </summary>
            SingleStringEscape,
            /// <summary>
            /// 单字符串头
            /// </summary>
            SimpleStringStart,
            /// <summary>
            /// 字符串
            /// </summary>
            SimpleString,
            /// <summary>
            /// 简单字符串引号
            /// </summary>
            SimpleStringQuotationMarkOrOver,
            /// <summary>
            /// 字符串
            /// </summary>
            SingleSimpleString,
            /// <summary>
            /// 
            /// </summary>
            SingleSimpleStringQuotationMarkOrOver,
            /// <summary>
            /// 数字 | 十六进制数
            /// </summary>
            NumberOrHexNumber,
            /// <summary>
            /// 数字
            /// </summary>
            Number,
            /// <summary>
            /// 十六进制数
            /// </summary>
            HexNumber,
            /// <summary>
            /// 标识符
            /// </summary>
            Identifier
        }
    }
}

