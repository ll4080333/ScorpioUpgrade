namespace Scorpio.Compiler
{
    using Scorpio;
    using Scorpio.CodeDom;
    using Scorpio.CodeDom.Temp;
    using Scorpio.Exception;
    using Scorpio.Function;
    using Scorpio.Runtime;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;

    public class ScriptParser
    {
        private DefineState m_define;
        private Stack<DefineState> m_Defines = new Stack<DefineState>();
        private Stack<ScriptExecutable> m_Executables = new Stack<ScriptExecutable>();
        private int m_iNextToken;
        private List<Token> m_listTokens;
        private Script m_script;
        private ScriptExecutable m_scriptExecutable;
        private string m_strBreviary;

        public ScriptParser(Script script, List<Token> listTokens, string strBreviary)
        {
            this.m_script = script;
            this.m_strBreviary = strBreviary;
            this.m_iNextToken = 0;
            this.m_listTokens = new List<Token>(listTokens);
        }

        public void BeginExecutable(Executable_Block block)
        {
            this.m_scriptExecutable = new ScriptExecutable(block);
            this.m_Executables.Push(this.m_scriptExecutable);
        }

        public void EndExecutable()
        {
            this.m_Executables.Pop();
            this.m_scriptExecutable = (this.m_Executables.Count > 0) ? this.m_Executables.Peek() : null;
        }

        private CodeArray GetArray()
        {
            this.ReadLeftBracket();
            Token token = this.PeekToken();
            CodeArray array = new CodeArray();
            while (token.Type != Scorpio.Compiler.TokenType.RightBracket)
            {
                if (this.PeekToken().Type != Scorpio.Compiler.TokenType.RightBracket)
                {
                    array._Elements.Add(this.GetObject());
                    token = this.PeekToken();
                    if (token.Type == Scorpio.Compiler.TokenType.Comma)
                    {
                        this.ReadComma();
                        continue;
                    }
                    if (token.Type != Scorpio.Compiler.TokenType.RightBracket)
                    {
                        throw new ParserException("Comma ',' or right parenthesis ']' expected in array object.", token);
                    }
                }
                break;
            }
            this.ReadRightBracket();
            array.Init();
            return array;
        }

        private CodeEval GetEval()
        {
            return new CodeEval { EvalObject = this.GetObject() };
        }

        private CodeCallFunction GetFunction(CodeObject member)
        {
            this.ReadLeftParenthesis();
            List<CodeObject> parameters = new List<CodeObject>();
            Token token = this.PeekToken();
            while (token.Type != Scorpio.Compiler.TokenType.RightPar)
            {
                parameters.Add(this.GetObject());
                token = this.PeekToken();
                if (token.Type == Scorpio.Compiler.TokenType.Comma)
                {
                    this.ReadComma();
                }
                else
                {
                    if (token.Type != Scorpio.Compiler.TokenType.RightPar)
                    {
                        throw new ParserException("Comma ',' or right parenthesis ')' expected in function declararion.", token);
                    }
                    break;
                }
            }
            this.ReadRightParenthesis();
            return new CodeCallFunction(member, parameters);
        }

        private CodeObject GetObject()
        {
            Stack<TempOperator> operateStack = new Stack<TempOperator>();
            Stack<CodeObject> objectStack = new Stack<CodeObject>();
            do
            {
                objectStack.Push(this.GetOneObject());
            }
            while (this.P_Operator(operateStack, objectStack));
            while (operateStack.Count > 0)
            {
                objectStack.Push(new CodeOperator(objectStack.Pop(), objectStack.Pop(), operateStack.Pop().Operator, this.m_strBreviary, this.GetSourceLine()));
            }
            CodeObject obj2 = objectStack.Pop();
            if (obj2 is CodeMember)
            {
                CodeMember member = obj2 as CodeMember;
                if (member.Calc == CALC.NONE)
                {
                    Token token = this.ReadToken();
                    switch (token.Type)
                    {
                        case Scorpio.Compiler.TokenType.AssignShi:
                        case Scorpio.Compiler.TokenType.AssignShr:
                        case Scorpio.Compiler.TokenType.Assign:
                        case Scorpio.Compiler.TokenType.AssignCombine:
                        case Scorpio.Compiler.TokenType.AssignXOR:
                        case Scorpio.Compiler.TokenType.AssignMinus:
                        case Scorpio.Compiler.TokenType.AssignMultiply:
                        case Scorpio.Compiler.TokenType.AssignDivide:
                        case Scorpio.Compiler.TokenType.AssignModulo:
                        case Scorpio.Compiler.TokenType.AssignInclusiveOr:
                        case Scorpio.Compiler.TokenType.AssignPlus:
                            obj2 = new CodeAssign(member, this.GetObject(), token.Type, this.m_strBreviary, token.SourceLine);
                            goto Label_0121;
                    }
                    this.UndoToken();
                }
            }
        Label_0121:
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.QuestionMark)
            {
                this.ReadToken();
                CodeTernary ternary = new CodeTernary {
                    Allow = obj2,
                    True = this.GetObject()
                };
                this.ReadColon();
                ternary.False = this.GetObject();
                return ternary;
            }
            return obj2;
        }

        private DefineObject GetOneDefine()
        {
            Token token = this.ReadToken();
            bool flag = false;
            if (token.Type == Scorpio.Compiler.TokenType.Not)
            {
                flag = true;
                token = this.ReadToken();
            }
            if (token.Type == Scorpio.Compiler.TokenType.LeftPar)
            {
                DefineObject obj2 = this.ReadDefine();
                this.ReadRightParenthesis();
                obj2.Not = flag;
                return obj2;
            }
            if (token.Type != Scorpio.Compiler.TokenType.Identifier)
            {
                throw new ParserException("宏定义判断只支持 字符串", token);
            }
            return new DefineString(token.Lexeme.ToString()) { Not = flag };
        }

        private CodeObject GetOneObject()
        {
            CodeObject parent = null;
            Token token = this.ReadToken();
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            CALC nONE = CALC.NONE;
            while (true)
            {
                if (token.Type == Scorpio.Compiler.TokenType.Not)
                {
                    flag = true;
                }
                else if (token.Type == Scorpio.Compiler.TokenType.Minus)
                {
                    flag2 = true;
                }
                else
                {
                    if (token.Type != Scorpio.Compiler.TokenType.Negative)
                    {
                        break;
                    }
                    flag3 = true;
                }
                token = this.ReadToken();
            }
            if (token.Type == Scorpio.Compiler.TokenType.Increment)
            {
                nONE = CALC.PRE_INCREMENT;
                token = this.ReadToken();
            }
            else if (token.Type == Scorpio.Compiler.TokenType.Decrement)
            {
                nONE = CALC.PRE_DECREMENT;
                token = this.ReadToken();
            }
            switch (token.Type)
            {
                case Scorpio.Compiler.TokenType.LeftBrace:
                    this.UndoToken();
                    parent = this.GetTable();
                    break;

                case Scorpio.Compiler.TokenType.LeftPar:
                    parent = new CodeRegion(this.GetObject());
                    this.ReadRightParenthesis();
                    break;

                case Scorpio.Compiler.TokenType.LeftBracket:
                    this.UndoToken();
                    parent = this.GetArray();
                    break;

                case Scorpio.Compiler.TokenType.Function:
                    this.UndoToken();
                    parent = new CodeFunction(this.ParseFunctionDeclaration(false));
                    break;

                case Scorpio.Compiler.TokenType.Boolean:
                case Scorpio.Compiler.TokenType.Number:
                case Scorpio.Compiler.TokenType.String:
                case Scorpio.Compiler.TokenType.SimpleString:
                    parent = new CodeScriptObject(this.m_script, token.Lexeme);
                    break;

                case Scorpio.Compiler.TokenType.Null:
                    parent = new CodeScriptObject(this.m_script, null);
                    break;

                case Scorpio.Compiler.TokenType.Eval:
                    parent = this.GetEval();
                    break;

                case Scorpio.Compiler.TokenType.Identifier:
                    parent = new CodeMember((string) token.Lexeme);
                    break;

                default:
                    throw new ParserException("Object起始关键字错误 ", token);
            }
            parent.StackInfo = new StackInfo(this.m_strBreviary, token.SourceLine);
            parent = this.GetVariable(parent);
            parent.Not = flag;
            parent.Minus = flag2;
            parent.Negative = flag3;
            if (parent is CodeMember)
            {
                if (nONE != CALC.NONE)
                {
                    ((CodeMember) parent).Calc = nONE;
                    return parent;
                }
                Token token2 = this.ReadToken();
                if (token2.Type == Scorpio.Compiler.TokenType.Increment)
                {
                    nONE = CALC.POST_INCREMENT;
                }
                else if (token2.Type == Scorpio.Compiler.TokenType.Decrement)
                {
                    nONE = CALC.POST_DECREMENT;
                }
                else
                {
                    this.UndoToken();
                }
                if (nONE != CALC.NONE)
                {
                    ((CodeMember) parent).Calc = nONE;
                }
                return parent;
            }
            if (nONE != CALC.NONE)
            {
                throw new ParserException("++ 或者 -- 只支持变量的操作", token);
            }
            return parent;
        }

        private int GetSourceLine()
        {
            return this.PeekToken().SourceLine;
        }

        private CodeTable GetTable()
        {
            CodeTable table = new CodeTable();
            this.ReadLeftBrace();
            while (this.PeekToken().Type != Scorpio.Compiler.TokenType.RightBrace)
            {
                Token token = this.ReadToken();
                if ((token.Type != Scorpio.Compiler.TokenType.Comma) && (token.Type != Scorpio.Compiler.TokenType.SemiColon))
                {
                    if ((((token.Type == Scorpio.Compiler.TokenType.Identifier) || (token.Type == Scorpio.Compiler.TokenType.String)) || ((token.Type == Scorpio.Compiler.TokenType.SimpleString) || (token.Type == Scorpio.Compiler.TokenType.Number))) || ((token.Type == Scorpio.Compiler.TokenType.Boolean) || (token.Type == Scorpio.Compiler.TokenType.Null)))
                    {
                        Token token2 = this.ReadToken();
                        if ((token2.Type == Scorpio.Compiler.TokenType.Assign) || (token2.Type == Scorpio.Compiler.TokenType.Colon))
                        {
                            if (token.Type == Scorpio.Compiler.TokenType.Null)
                            {
                                table._Variables.Add(new CodeTable.TableVariable(this.m_script.Null.KeyValue, this.GetObject()));
                            }
                            else
                            {
                                table._Variables.Add(new CodeTable.TableVariable(token.Lexeme, this.GetObject()));
                            }
                        }
                        else
                        {
                            if ((token2.Type != Scorpio.Compiler.TokenType.Comma) && (token2.Type != Scorpio.Compiler.TokenType.SemiColon))
                            {
                                throw new ParserException("Table变量赋值符号为[=]或者[:]", token);
                            }
                            if (token.Type == Scorpio.Compiler.TokenType.Null)
                            {
                                table._Variables.Add(new CodeTable.TableVariable(this.m_script.Null.KeyValue, new CodeScriptObject(this.m_script, null)));
                            }
                            else
                            {
                                table._Variables.Add(new CodeTable.TableVariable(token.Lexeme, new CodeScriptObject(this.m_script, null)));
                            }
                        }
                    }
                    else
                    {
                        if (token.Type != Scorpio.Compiler.TokenType.Function)
                        {
                            throw new ParserException("Table开始关键字必须为[变量名称]或者[function]关键字", token);
                        }
                        this.UndoToken();
                        table._Functions.Add(this.ParseFunctionDeclaration(true));
                    }
                }
            }
            this.ReadRightBrace();
            table.Init();
            return table;
        }

        private CodeObject GetVariable(CodeObject parent)
        {
            CodeObject function = parent;
            while (true)
            {
                Token token = this.ReadToken();
                if (token.Type == Scorpio.Compiler.TokenType.Period)
                {
                    function = new CodeMember(this.ReadIdentifier(), function);
                }
                else if (token.Type == Scorpio.Compiler.TokenType.LeftBracket)
                {
                    CodeObject member = this.GetObject();
                    this.ReadRightBracket();
                    if (member is CodeScriptObject)
                    {
                        ScriptObject obj4 = ((CodeScriptObject) member).Object;
                        if (member.Not)
                        {
                            function = new CodeMember(!obj4.LogicOperation(), function);
                        }
                        else if (member.Minus)
                        {
                            ScriptNumber number = obj4 as ScriptNumber;
                            if (number == null)
                            {
                                throw new ParserException("Script Object Type [" + obj4.Type + "] is cannot use [-] sign", token);
                            }
                            function = new CodeMember(number.Minus().KeyValue, function);
                        }
                        else if (member.Negative)
                        {
                            ScriptNumber number2 = obj4 as ScriptNumber;
                            if (number2 == null)
                            {
                                throw new ParserException("Script Object Type [" + obj4.Type + "] is cannot use [~] sign", token);
                            }
                            function = new CodeMember(number2.Negative().KeyValue, function);
                        }
                        else
                        {
                            function = new CodeMember(obj4.KeyValue, function);
                        }
                    }
                    else
                    {
                        function = new CodeMember(member, function);
                    }
                }
                else if (token.Type == Scorpio.Compiler.TokenType.LeftPar)
                {
                    this.UndoToken();
                    function = this.GetFunction(function);
                }
                else
                {
                    this.UndoToken();
                    return function;
                }
                function.StackInfo = new StackInfo(this.m_strBreviary, token.SourceLine);
            }
        }

        protected bool HasMoreTokens()
        {
            return (this.m_iNextToken < this.m_listTokens.Count);
        }

        private bool IsDefine()
        {
            return this.IsDefine_impl(this.ReadDefine());
        }

        private bool IsDefine_impl(DefineObject define)
        {
            bool flag = false;
            if (define is DefineString)
            {
                flag = this.m_script.ContainDefine(((DefineString) define).Define);
            }
            else
            {
                DefineOperate operate = (DefineOperate) define;
                bool flag2 = this.IsDefine_impl(operate.Left);
                if (flag2 && !operate.and)
                {
                    flag = true;
                }
                else if (!flag2 && operate.and)
                {
                    flag = false;
                }
                else if (operate.and)
                {
                    flag = flag2 && this.IsDefine_impl(operate.Right);
                }
                else
                {
                    flag = flag2 || this.IsDefine_impl(operate.Right);
                }
            }
            if (define.Not)
            {
                flag = !flag;
            }
            return flag;
        }

        private bool OperatorDefine(Stack<bool> operateStack, Stack<DefineObject> objectStack)
        {
            Token token = this.PeekToken();
            if ((token.Type != Scorpio.Compiler.TokenType.And) && (token.Type != Scorpio.Compiler.TokenType.Or))
            {
                return false;
            }
            this.ReadToken();
            while (operateStack.Count > 0)
            {
                objectStack.Push(new DefineOperate(objectStack.Pop(), objectStack.Pop(), operateStack.Pop()));
            }
            operateStack.Push(token.Type == Scorpio.Compiler.TokenType.And);
            return true;
        }

        private bool P_Operator(Stack<TempOperator> operateStack, Stack<CodeObject> objectStack)
        {
            TempOperator oper = TempOperator.GetOper(this.PeekToken().Type);
            if (oper == null)
            {
                return false;
            }
            this.ReadToken();
            while (operateStack.Count > 0)
            {
                if (operateStack.Peek().Level < oper.Level)
                {
                    break;
                }
                objectStack.Push(new CodeOperator(objectStack.Pop(), objectStack.Pop(), operateStack.Pop().Operator, this.m_strBreviary, this.GetSourceLine()));
            }
            operateStack.Push(oper);
            return true;
        }

        public ScriptExecutable Parse()
        {
            this.m_iNextToken = 0;
            return this.ParseStatementBlock(Executable_Block.Context, false, Scorpio.Compiler.TokenType.Finished);
        }

        private void ParseBlock()
        {
            this.UndoToken();
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_BLOCK, new CodeCallBlock(this.ParseStatementBlock(Executable_Block.Block))));
        }

        private void ParseCase(List<CodeObject> allow)
        {
            allow.Add(this.GetObject());
            this.ReadColon();
            if (this.ReadToken().Type == Scorpio.Compiler.TokenType.Case)
            {
                this.ParseCase(allow);
            }
            else
            {
                this.UndoToken();
            }
        }

        private TempCondition ParseCondition(bool condition, Executable_Block block)
        {
            CodeObject allow = null;
            if (condition)
            {
                this.ReadLeftParenthesis();
                allow = this.GetObject();
                this.ReadRightParenthesis();
            }
            return new TempCondition(allow, this.ParseStatementBlock(block), block);
        }

        private void ParseExpression()
        {
            this.UndoToken();
            Token token = this.PeekToken();
            CodeObject obj2 = this.GetObject();
            if (obj2 is CodeCallFunction)
            {
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FUNCTION, obj2));
            }
            else if (obj2 is CodeMember)
            {
                if ((obj2 as CodeMember).Calc == CALC.NONE)
                {
                    throw new ParserException("变量后缀不支持此操作符  " + this.PeekToken().Type, token);
                }
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RESOLVE, obj2));
            }
            else
            {
                if (!(obj2 is CodeAssign) && !(obj2 is CodeEval))
                {
                    throw new ParserException("语法不支持起始符号为 " + obj2.GetType(), token);
                }
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RESOLVE, obj2));
            }
        }

        private void ParseFor()
        {
            this.ReadLeftParenthesis();
            int iNextToken = this.m_iNextToken;
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Var)
            {
                this.ReadToken();
            }
            Token token = this.ReadToken();
            if ((token.Type == Scorpio.Compiler.TokenType.Identifier) && (this.ReadToken().Type == Scorpio.Compiler.TokenType.Assign))
            {
                CodeObject obj2 = this.GetObject();
                if (this.ReadToken().Type == Scorpio.Compiler.TokenType.Comma)
                {
                    this.ParseFor_Simple((string) token.Lexeme, obj2);
                    return;
                }
            }
            this.m_iNextToken = iNextToken;
            this.ParseFor_impl();
        }

        private void ParseFor_impl()
        {
            CodeFor @for = new CodeFor();
            if (this.ReadToken().Type != Scorpio.Compiler.TokenType.SemiColon)
            {
                this.UndoToken();
                @for.BeginExecutable = this.ParseStatementBlock(Executable_Block.ForBegin, false, Scorpio.Compiler.TokenType.SemiColon);
            }
            if (this.ReadToken().Type != Scorpio.Compiler.TokenType.SemiColon)
            {
                this.UndoToken();
                @for.Condition = this.GetObject();
                this.ReadSemiColon();
            }
            if (this.ReadToken().Type != Scorpio.Compiler.TokenType.RightPar)
            {
                this.UndoToken();
                @for.LoopExecutable = this.ParseStatementBlock(Executable_Block.ForLoop, false, Scorpio.Compiler.TokenType.RightPar);
            }
            @for.BlockExecutable = this.ParseStatementBlock(Executable_Block.For);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FOR, @for));
        }

        private void ParseFor_Simple(string Identifier, CodeObject obj)
        {
            CodeForSimple simple = new CodeForSimple {
                Identifier = Identifier,
                Begin = obj,
                Finished = this.GetObject()
            };
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Comma)
            {
                this.ReadToken();
                simple.Step = this.GetObject();
            }
            this.ReadRightParenthesis();
            simple.BlockExecutable = this.ParseStatementBlock(Executable_Block.For);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FORSIMPLE, simple));
        }

        private void ParseForeach()
        {
            CodeForeach @foreach = new CodeForeach();
            this.ReadLeftParenthesis();
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Var)
            {
                this.ReadToken();
            }
            @foreach.Identifier = this.ReadIdentifier();
            this.ReadIn();
            @foreach.LoopObject = this.GetObject();
            this.ReadRightParenthesis();
            @foreach.BlockExecutable = this.ParseStatementBlock(Executable_Block.Foreach);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_FOREACH, @foreach));
        }

        private void ParseFunction()
        {
            Token token = this.PeekToken();
            this.UndoToken();
            ScriptScriptFunction func = this.ParseFunctionDeclaration(true);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.MOV, new CodeMember(func.Name), new CodeFunction(func, this.m_strBreviary, token.SourceLine)));
        }

        private ScriptScriptFunction ParseFunctionDeclaration(bool needName)
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.Function)
            {
                throw new ParserException("Function declaration must start with the 'function' keyword.", token);
            }
            string name = needName ? this.ReadIdentifier() : ((this.PeekToken().Type == Scorpio.Compiler.TokenType.Identifier) ? this.ReadIdentifier() : "");
            List<string> listParameters = new List<string>();
            bool bParams = false;
            Token token2 = this.ReadToken();
            if (token2.Type != Scorpio.Compiler.TokenType.LeftPar)
            {
                goto Label_0107;
            }
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.RightPar)
            {
                goto Label_00F9;
            }
        Label_0073:
            token = this.ReadToken();
            if (token.Type == Scorpio.Compiler.TokenType.Params)
            {
                token = this.ReadToken();
                bParams = true;
            }
            if (token.Type != Scorpio.Compiler.TokenType.Identifier)
            {
                throw new ParserException("Unexpected token '" + token.Lexeme + "' in function declaration.", token);
            }
            string item = token.Lexeme.ToString();
            listParameters.Add(item);
            token = this.PeekToken();
            if ((token.Type == Scorpio.Compiler.TokenType.Comma) && !bParams)
            {
                this.ReadComma();
                goto Label_0073;
            }
            if (token.Type != Scorpio.Compiler.TokenType.RightPar)
            {
                throw new ParserException("Comma ',' or right parenthesis ')' expected in function declararion.", token);
            }
        Label_00F9:
            this.ReadRightParenthesis();
            token2 = this.ReadToken();
        Label_0107:
            if (token2.Type == Scorpio.Compiler.TokenType.LeftBrace)
            {
                this.UndoToken();
            }
            ScriptExecutable scriptExecutable = this.ParseStatementBlock(Executable_Block.Function);
            return new ScriptScriptFunction(this.m_script, name, new ScorpioScriptFunction(this.m_script, listParameters, scriptExecutable, bParams));
        }

        private void ParseIf()
        {
            Token token;
            CodeIf @if = new CodeIf {
                If = this.ParseCondition(true, Executable_Block.If)
            };
            List<TempCondition> elseIf = new List<TempCondition>();
        Label_001A:
            token = this.ReadToken();
            if (token.Type == Scorpio.Compiler.TokenType.ElseIf)
            {
                elseIf.Add(this.ParseCondition(true, Executable_Block.If));
                goto Label_001A;
            }
            if (token.Type == Scorpio.Compiler.TokenType.Else)
            {
                if (this.PeekToken().Type == Scorpio.Compiler.TokenType.If)
                {
                    this.ReadToken();
                    elseIf.Add(this.ParseCondition(true, Executable_Block.If));
                    goto Label_001A;
                }
                this.UndoToken();
            }
            else
            {
                this.UndoToken();
            }
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Else)
            {
                this.ReadToken();
                @if.Else = this.ParseCondition(false, Executable_Block.If);
            }
            @if.Init(elseIf);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_IF, @if));
        }

        private void ParseReturn()
        {
            Token token = this.PeekToken();
            if (((token.Type == Scorpio.Compiler.TokenType.RightBrace) || (token.Type == Scorpio.Compiler.TokenType.SemiColon)) || (token.Type == Scorpio.Compiler.TokenType.Finished))
            {
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RET, ""));
            }
            else
            {
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.RET, this.GetObject()));
            }
        }

        private void ParseSharp()
        {
            Token token = this.ReadToken();
            if (token.Type == Scorpio.Compiler.TokenType.Define)
            {
                if (this.m_scriptExecutable.m_Block != Executable_Block.Context)
                {
                    throw new ParserException("#define只能使用在上下文", token);
                }
                this.m_script.PushDefine(this.ReadIdentifier());
            }
            else if (token.Type == Scorpio.Compiler.TokenType.If)
            {
                if (this.m_define == null)
                {
                    if (this.IsDefine())
                    {
                        this.m_define = new DefineState(DefineType.Already);
                    }
                    else
                    {
                        this.m_define = new DefineState(DefineType.Being);
                        this.PopSharp();
                    }
                }
                else if (this.m_define.State == DefineType.Already)
                {
                    if (this.IsDefine())
                    {
                        this.m_Defines.Push(this.m_define);
                        this.m_define = new DefineState(DefineType.Already);
                    }
                    else
                    {
                        this.m_Defines.Push(this.m_define);
                        this.m_define = new DefineState(DefineType.Being);
                        this.PopSharp();
                    }
                }
                else if (this.m_define.State == DefineType.Being)
                {
                    this.m_Defines.Push(this.m_define);
                    this.m_define = new DefineState(DefineType.Break);
                    this.PopSharp();
                }
                else if (this.m_define.State == DefineType.Break)
                {
                    this.m_Defines.Push(this.m_define);
                    this.m_define = new DefineState(DefineType.Break);
                    this.PopSharp();
                }
            }
            else if (token.Type == Scorpio.Compiler.TokenType.Ifndef)
            {
                if (this.m_define == null)
                {
                    if (!this.IsDefine())
                    {
                        this.m_define = new DefineState(DefineType.Already);
                    }
                    else
                    {
                        this.m_define = new DefineState(DefineType.Being);
                        this.PopSharp();
                    }
                }
                else if (this.m_define.State == DefineType.Already)
                {
                    if (!this.IsDefine())
                    {
                        this.m_Defines.Push(this.m_define);
                        this.m_define = new DefineState(DefineType.Already);
                    }
                    else
                    {
                        this.m_Defines.Push(this.m_define);
                        this.m_define = new DefineState(DefineType.Being);
                        this.PopSharp();
                    }
                }
                else if (this.m_define.State == DefineType.Being)
                {
                    this.m_Defines.Push(this.m_define);
                    this.m_define = new DefineState(DefineType.Break);
                    this.PopSharp();
                }
            }
            else if (token.Type == Scorpio.Compiler.TokenType.ElseIf)
            {
                if (this.m_define == null)
                {
                    throw new ParserException("未找到#if或#ifndef", token);
                }
                if ((this.m_define.State == DefineType.Already) || (this.m_define.State == DefineType.Break))
                {
                    this.m_define.State = DefineType.Break;
                    this.PopSharp();
                }
                else if (this.IsDefine())
                {
                    this.m_define.State = DefineType.Already;
                }
                else
                {
                    this.m_define.State = DefineType.Being;
                    this.PopSharp();
                }
            }
            else if (token.Type == Scorpio.Compiler.TokenType.Else)
            {
                if (this.m_define == null)
                {
                    throw new ParserException("未找到#if或#ifndef", token);
                }
                if ((this.m_define.State == DefineType.Already) || (this.m_define.State == DefineType.Break))
                {
                    this.m_define.State = DefineType.Break;
                    this.PopSharp();
                }
                else
                {
                    this.m_define.State = DefineType.Already;
                }
            }
            else
            {
                if (token.Type != Scorpio.Compiler.TokenType.Endif)
                {
                    throw new ParserException("#后缀不支持" + token.Type, token);
                }
                if (this.m_define == null)
                {
                    throw new ParserException("未找到#if或#ifndef", token);
                }
                if (this.m_Defines.Count > 0)
                {
                    this.m_define = this.m_Defines.Pop();
                    if (this.m_define.State == DefineType.Break)
                    {
                        this.PopSharp();
                    }
                }
                else
                {
                    this.m_define = null;
                }
            }
        }

        private void ParseStatement()
        {
            Token token = this.ReadToken();
            UnityEngine.Debug.Log(string.Format("<color=yellow>{0}</color>",token.Type));
            switch (token.Type)
            {
                case Scorpio.Compiler.TokenType.Var:
                    this.ParseVar();
                    return;

                case Scorpio.Compiler.TokenType.LeftBrace:
                    this.ParseBlock();
                    return;

                case Scorpio.Compiler.TokenType.SemiColon:
                    return;

                case Scorpio.Compiler.TokenType.Sharp:
                    this.ParseSharp();
                    return;

                case Scorpio.Compiler.TokenType.Increment:
                case Scorpio.Compiler.TokenType.Decrement:
                case Scorpio.Compiler.TokenType.Eval:
                case Scorpio.Compiler.TokenType.Identifier:
                    this.ParseExpression();
                    return;

                case Scorpio.Compiler.TokenType.If:
                    this.ParseIf();
                    return;

                case Scorpio.Compiler.TokenType.For:
                    this.ParseFor();
                    return;

                case Scorpio.Compiler.TokenType.Foreach:
                    this.ParseForeach();
                    return;

                case Scorpio.Compiler.TokenType.Switch:
                    this.ParseSwtich();
                    return;

                case Scorpio.Compiler.TokenType.Break:
                    this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.BREAK, new CodeObject(this.m_strBreviary, token.SourceLine)));
                    return;

                case Scorpio.Compiler.TokenType.Continue:
                    this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CONTINUE, new CodeObject(this.m_strBreviary, token.SourceLine)));
                    return;

                case Scorpio.Compiler.TokenType.Return:
                    this.ParseReturn();
                    return;

                case Scorpio.Compiler.TokenType.While:
                    this.ParseWhile();
                    return;

                case Scorpio.Compiler.TokenType.Function:
                    this.ParseFunction();
                    return;

                case Scorpio.Compiler.TokenType.Try:
                    this.ParseTry();
                    return;

                case Scorpio.Compiler.TokenType.Throw:
                    this.ParseThrow();
                    return;
            }
            throw new ParserException("不支持的语法 ", token);
        }

        private ScriptExecutable ParseStatementBlock(Executable_Block block)
        {
            return this.ParseStatementBlock(block, true, Scorpio.Compiler.TokenType.RightBrace);
        }

        private ScriptExecutable ParseStatementBlock(Executable_Block block, bool readLeftBrace, Scorpio.Compiler.TokenType finished)
        {
            this.BeginExecutable(block);
            if (readLeftBrace && (this.PeekToken().Type != Scorpio.Compiler.TokenType.LeftBrace))
            {
                this.ParseStatement();
                if (this.PeekToken().Type == Scorpio.Compiler.TokenType.SemiColon)
                {
                    this.ReadToken();
                }
            }
            else
            {
                if (readLeftBrace)
                {
                    this.ReadLeftBrace();
                }
                while (this.HasMoreTokens())
                {
                    if (this.ReadToken().Type == finished)
                    {
                        break;
                    }
                    this.UndoToken();
                    this.ParseStatement();
                }
            }
            ScriptExecutable scriptExecutable = this.m_scriptExecutable;
            scriptExecutable.EndScriptInstruction();
            this.EndExecutable();
            return scriptExecutable;
        }

        private void ParseSwtich()
        {
            Token token;
            CodeSwitch switch2 = new CodeSwitch();
            this.ReadLeftParenthesis();
            switch2.Condition = this.GetObject();
            this.ReadRightParenthesis();
            this.ReadLeftBrace();
            List<TempCase> cases = new List<TempCase>();
        Label_002A:
            token = this.ReadToken();
            if (token.Type == Scorpio.Compiler.TokenType.Case)
            {
                List<CodeObject> allow = new List<CodeObject>();
                this.ParseCase(allow);
                cases.Add(new TempCase(this.m_script, allow, this.ParseStatementBlock(Executable_Block.Switch, false, Scorpio.Compiler.TokenType.Break)));
                goto Label_002A;
            }
            if (token.Type == Scorpio.Compiler.TokenType.Default)
            {
                this.ReadColon();
                switch2.Default = new TempCase(this.m_script, null, this.ParseStatementBlock(Executable_Block.Switch, false, Scorpio.Compiler.TokenType.Break));
                goto Label_002A;
            }
            if (token.Type == Scorpio.Compiler.TokenType.SemiColon)
            {
                goto Label_002A;
            }
            this.UndoToken();
            this.ReadRightBrace();
            switch2.SetCases(cases);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_SWITCH, switch2));
        }

        private void ParseThrow()
        {
            CodeThrow @throw = new CodeThrow {
                obj = this.GetObject()
            };
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.THROW, @throw));
        }

        private void ParseTry()
        {
            CodeTry @try = new CodeTry {
                TryExecutable = this.ParseStatementBlock(Executable_Block.Context)
            };
            this.ReadCatch();
            this.ReadLeftParenthesis();
            @try.Identifier = this.ReadIdentifier();
            this.ReadRightParenthesis();
            @try.CatchExecutable = this.ParseStatementBlock(Executable_Block.Context);
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_TRY, @try));
        }

        private void ParseVar()
        {
            Token token;
        Label_0000:
            token = this.PeekToken();
            if (token.Type == Scorpio.Compiler.TokenType.Function)
            {
                ScriptScriptFunction func = this.ParseFunctionDeclaration(true);
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.VAR, func.Name));
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.MOV, new CodeMember(func.Name), new CodeFunction(func, this.m_strBreviary, token.SourceLine)));
            }
            else
            {
                this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.VAR, this.ReadIdentifier()));
                if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Assign)
                {
                    this.UndoToken();
                    this.ParseStatement();
                }
            }
            if (this.ReadToken().Type != Scorpio.Compiler.TokenType.Comma)
            {
                this.UndoToken();
            }
            else
            {
                if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Var)
                {
                    this.ReadToken();
                }
                goto Label_0000;
            }
        }

        private void ParseWhile()
        {
            CodeWhile @while = new CodeWhile {
                While = this.ParseCondition(true, Executable_Block.While)
            };
            this.m_scriptExecutable.AddScriptInstruction(new ScriptInstruction(Opcode.CALL_WHILE, @while));
        }

        protected Token PeekToken()
        {
            if (!this.HasMoreTokens())
            {
                throw new LexerException("Unexpected end of token stream.");
            }
            return this.m_listTokens[this.m_iNextToken];
        }

        private void PopSharp()
        {
        Label_0000:
            while (this.ReadToken().Type != Scorpio.Compiler.TokenType.Sharp)
            {
            }
            if (this.PeekToken().Type == Scorpio.Compiler.TokenType.Define)
            {
                this.ReadToken();
                goto Label_0000;
            }
            this.ParseSharp();
        }

        protected void ReadCatch()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.Catch)
            {
                throw new ParserException("Catch 'catch' expected.", token);
            }
        }

        protected void ReadColon()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.Colon)
            {
                throw new ParserException("Colon ':' expected.", token);
            }
        }

        protected void ReadComma()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.Comma)
            {
                throw new ParserException("Comma ',' expected.", token);
            }
        }

        private DefineObject ReadDefine()
        {
            Stack<bool> operateStack = new Stack<bool>();
            Stack<DefineObject> objectStack = new Stack<DefineObject>();
            do
            {
                objectStack.Push(this.GetOneDefine());
            }
            while (this.OperatorDefine(operateStack, objectStack));
            while (operateStack.Count > 0)
            {
                objectStack.Push(new DefineOperate(objectStack.Pop(), objectStack.Pop(), operateStack.Pop()));
            }
            return objectStack.Pop();
        }

        protected string ReadIdentifier()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.Identifier)
            {
                throw new ParserException("Identifier expected.", token);
            }
            return token.Lexeme.ToString();
        }

        protected void ReadIn()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.In)
            {
                throw new ParserException("In 'in' expected.", token);
            }
        }

        protected void ReadLeftBrace()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.LeftBrace)
            {
                throw new ParserException("Left brace '{' expected.", token);
            }
        }

        protected void ReadLeftBracket()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.LeftBracket)
            {
                throw new ParserException("Left bracket '[' expected for array indexing expression.", token);
            }
        }

        protected void ReadLeftParenthesis()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.LeftPar)
            {
                throw new ParserException("Left parenthesis '(' expected.", token);
            }
        }

        protected void ReadRightBrace()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.RightBrace)
            {
                throw new ParserException("Right brace '}' expected.", token);
            }
        }

        protected void ReadRightBracket()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.RightBracket)
            {
                throw new ParserException("Right bracket ']' expected for array indexing expression.", token);
            }
        }

        protected void ReadRightParenthesis()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.RightPar)
            {
                throw new ParserException("Right parenthesis ')' expected.", token);
            }
        }

        protected void ReadSemiColon()
        {
            Token token = this.ReadToken();
            if (token.Type != Scorpio.Compiler.TokenType.SemiColon)
            {
                throw new ParserException("SemiColon ';' expected.", token);
            }
        }

        protected Token ReadToken()
        {
            if (!this.HasMoreTokens())
            {
                throw new LexerException("Unexpected end of token stream.");
            }
            return this.m_listTokens[this.m_iNextToken++];
        }

        protected void UndoToken()
        {
            if (this.m_iNextToken <= 0)
            {
                throw new LexerException("No more tokens to undo.");
            }
            this.m_iNextToken--;
        }

        private class DefineObject
        {
            public bool Not;
        }

        private class DefineOperate : ScriptParser.DefineObject
        {
            public bool and;
            public ScriptParser.DefineObject Left;
            public ScriptParser.DefineObject Right;

            public DefineOperate(ScriptParser.DefineObject left, ScriptParser.DefineObject right, bool and)
            {
                this.Left = left;
                this.Right = right;
                this.and = and;
            }
        }

        private class DefineState
        {
            public ScriptParser.DefineType State;

            public DefineState(ScriptParser.DefineType state)
            {
                this.State = state;
            }
        }

        private class DefineString : ScriptParser.DefineObject
        {
            public string Define;

            public DefineString(string define)
            {
                this.Define = define;
            }
        }

        private enum DefineType
        {
            None,
            Already,
            Being,
            Break
        }
    }
}

