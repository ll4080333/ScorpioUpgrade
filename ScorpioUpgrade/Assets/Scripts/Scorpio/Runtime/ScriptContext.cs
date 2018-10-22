namespace Scorpio.Runtime
{
    using Scorpio;
    using Scorpio.CodeDom;
    using Scorpio.CodeDom.Temp;
    using Scorpio.Compiler;
    using Scorpio.Exception;
    using Scorpio.Function;
    using Scorpio.Variable;
    using System;
    using System.Collections.Generic;

    public class ScriptContext
    {
        private Executable_Block m_block;
        private bool m_Break;
        private bool m_Continue;
        private int m_InstructionCount;
        private bool m_Over;
        private ScriptContext m_parent;
        private ScriptObject m_returnObject;
        private Script m_script;
        private ScriptInstruction m_scriptInstruction;
        private ScriptInstruction[] m_scriptInstructions;
        private Dictionary<string, ScriptObject> m_variableDictionary;

        public ScriptContext(Script script, ScriptExecutable scriptExecutable) : this(script, scriptExecutable, null, Executable_Block.None)
        {
        }

        public ScriptContext(Script script, ScriptExecutable scriptExecutable, ScriptContext parent) : this(script, scriptExecutable, parent, Executable_Block.None)
        {
        }

        public ScriptContext(Script script, ScriptExecutable scriptExecutable, ScriptContext parent, Executable_Block block)
        {
            this.m_script = script;
            this.m_parent = parent;
            this.m_block = block;
            this.m_variableDictionary = new Dictionary<string, ScriptObject>();
            if (scriptExecutable != null)
            {
                this.m_scriptInstructions = scriptExecutable.ScriptInstructions;
                this.m_InstructionCount = this.m_scriptInstructions.Length;
            }
        }

        private void ApplyVariableObject(string name)
        {
            if (!this.m_variableDictionary.ContainsKey(name))
            {
                this.m_variableDictionary.Add(name, this.m_script.Null);
            }
        }

        public ScriptObject Execute()
        {
            this.Reset();
            int num = 0;
            while (num < this.m_InstructionCount)
            {
                this.m_scriptInstruction = this.m_scriptInstructions[num++];
                this.ExecuteInstruction();
                if (this.IsExecuted)
                {
                    break;
                }
            }
            return this.m_returnObject;
        }

        private ScriptObject Execute(ScriptExecutable executable)
        {
            if (executable == null)
            {
                return null;
            }
            this.Reset();
            ScriptInstruction[] scriptInstructions = executable.ScriptInstructions;
            int num = 0;
            int length = scriptInstructions.Length;
            while (num < length)
            {
                this.m_scriptInstruction = scriptInstructions[num++];
                this.ExecuteInstruction();
                if (this.IsExecuted)
                {
                    break;
                }
            }
            return this.m_returnObject;
        }

        private void ExecuteInstruction()
        {
            //UnityEngine.Debug.Log(string.Format("<color=yellow>Execute : {0}</color>",this.m_scriptInstruction.opcode.ToString()));
            switch (this.m_scriptInstruction.opcode)
            {
                case Opcode.MOV:
                    this.ProcessMov();
                    return;

                case Opcode.VAR:
                    this.ProcessVar();
                    return;

                case Opcode.CALL_BLOCK:
                    this.ProcessCallBlock();
                    return;

                case Opcode.CALL_IF:
                    this.ProcessCallIf();
                    return;

                case Opcode.CALL_FOR:
                    this.ProcessCallFor();
                    return;

                case Opcode.CALL_FORSIMPLE:
                    this.ProcessCallForSimple();
                    return;

                case Opcode.CALL_FOREACH:
                    this.ProcessCallForeach();
                    return;

                case Opcode.CALL_WHILE:
                    this.ProcessCallWhile();
                    return;

                case Opcode.CALL_SWITCH:
                    this.ProcessCallSwitch();
                    return;

                case Opcode.CALL_TRY:
                    this.ProcessTry();
                    return;

                case Opcode.CALL_FUNCTION:
                    this.ProcessCallFunction();
                    return;

                case Opcode.THROW:
                    this.ProcessThrow();
                    return;

                case Opcode.RESOLVE:
                    this.ProcessResolve();
                    return;

                case Opcode.RET:
                    this.ProcessRet();
                    return;

                case Opcode.BREAK:
                    this.ProcessBreak();
                    return;

                case Opcode.CONTINUE:
                    this.ProcessContinue();
                    return;
            }
        }

        private object GetMember(CodeMember member)
        {
            if (member.Type != MEMBER_TYPE.VALUE)
            {
                return this.ResolveOperand(member.MemberObject).KeyValue;
            }
            return member.MemberValue;
        }

        private ScriptObject GetVariable(CodeMember member)
        {
            ScriptObject obj2 = null;
            if (member.Parent == null)
            {
                string memberValue = (string) member.MemberValue;
                ScriptObject variableObject = this.GetVariableObject(memberValue);
                obj2 = (variableObject == null) ? this.m_script.GetValue(memberValue) : variableObject;
                obj2.Name = memberValue;
            }
            else
            {
                ScriptObject obj4 = this.ResolveOperand(member.Parent);
                this.m_script.SetStackInfo(member.StackInfo);
                if (member.Type == MEMBER_TYPE.VALUE)
                {
                    object key = member.MemberValue;
                    obj2 = obj4.GetValue(key);
                    obj2.Name = obj4.Name + "." + key.ToString();
                }
                else
                {
                    object keyValue = this.ResolveOperand(member.MemberObject).KeyValue;
                    obj2 = obj4.GetValue(keyValue);
                    obj2.Name = obj4.Name + "." + keyValue.ToString();
                }
            }
            if (obj2 == null)
            {
                throw new ExecutionException(this.m_script, "GetVariable member is error");
            }
            if (member.Calc == CALC.NONE)
            {
                return obj2;
            }
            ScriptNumber number = obj2 as ScriptNumber;
            if (number == null)
            {
                throw new ExecutionException(this.m_script, "++或者--只能应用于Number类型");
            }
            return number.Calc(member.Calc);
        }

        private ScriptObject GetVariableObject(string name)
        {
            if (this.m_variableDictionary.ContainsKey(name))
            {
                return this.m_variableDictionary[name];
            }
            if (this.m_parent != null)
            {
                return this.m_parent.GetVariableObject(name);
            }
            return null;
        }

        public void Initialize(Dictionary<string, ScriptObject> variable)
        {
            foreach (KeyValuePair<string, ScriptObject> pair in variable)
            {
                this.m_variableDictionary[pair.Key] = pair.Value;
            }
        }

        private void Initialize(string name, ScriptObject obj)
        {
            this.m_variableDictionary.Add(name, obj);
        }

        private void InvokeBreak(CodeObject bre)
        {
            this.m_Break = true;
            if (!this.SupportBreak())
            {
                if (this.m_parent == null)
                {
                    throw new ExecutionException(this.m_script, "当前模块不支持break语法");
                }
                this.m_parent.InvokeBreak(bre);
            }
        }

        private void InvokeContinue(CodeObject con)
        {
            this.m_Continue = true;
            if (!this.SupportContinue())
            {
                if (this.m_parent == null)
                {
                    throw new ExecutionException(this.m_script, "当前模块不支持continue语法");
                }
                this.m_parent.InvokeContinue(con);
            }
        }

        private void InvokeReturnValue(ScriptObject value)
        {
            this.m_Over = true;
            if (this.SupportReturnValue())
            {
                this.m_returnObject = value;
            }
            else
            {
                this.m_parent.InvokeReturnValue(value);
            }
        }

        private ScriptArray ParseArray(CodeArray array)
        {
            ScriptArray array2 = this.m_script.CreateArray();
            foreach (CodeObject obj2 in array.Elements)
            {
                array2.Add(this.ResolveOperand(obj2).Assign());
            }
            return array2;
        }

        private ScriptObject ParseAssign(CodeAssign assign)
        {
            if (assign.AssignType == Scorpio.Compiler.TokenType.Assign)
            {
                ScriptObject variable = this.ResolveOperand(assign.value);
                this.SetVariable(assign.member, variable);
                return variable;
            }
            return this.GetVariable(assign.member).AssignCompute(assign.AssignType, this.ResolveOperand(assign.value));
        }

        private ScriptObject ParseCall(CodeCallFunction scriptFunction, bool needRet)
        {
            ScriptObject obj2 = this.ResolveOperand(scriptFunction.Member);
            int parametersCount = scriptFunction.ParametersCount;
            ScriptObject[] parameters = new ScriptObject[parametersCount];
            for (int i = 0; i < parametersCount; i++)
            {
                parameters[i] = this.ResolveOperand(scriptFunction.Parameters[i]).Assign();
            }
            this.m_script.PushStackInfo();
            object obj3 = obj2.Call(parameters);
            this.m_script.PopStackInfo();
            if (!needRet)
            {
                return null;
            }
            return this.m_script.CreateObject(obj3);
        }

        private void ParseCallBlock(CodeCallBlock block)
        {
            new ScriptContext(this.m_script, block.Executable, this).Execute();
        }

        private ScriptObject ParseEval(CodeEval eval)
        {
            ScriptString str = this.ResolveOperand(eval.EvalObject) as ScriptString;
            if (str == null)
            {
                throw new ExecutionException(this.m_script, "Eval参数必须是一个字符串");
            }
            return this.m_script.LoadString("", str.Value, this, false);
        }

        private ScriptFunction ParseFunction(CodeFunction func)
        {
            return func.Func.Create().SetParentContext(this);
        }

        private ScriptObject ParseOperate(CodeOperator operate)
        {
            Scorpio.Compiler.TokenType @operator = operate.Operator;
            ScriptObject obj2 = this.ResolveOperand(operate.Left);
            switch (@operator)
            {
                case Scorpio.Compiler.TokenType.Plus:
                {
                    ScriptObject obj3 = this.ResolveOperand(operate.Right);
                    if ((obj2 is ScriptString) || (obj3 is ScriptString))
                    {
                        return new ScriptString(this.m_script, obj2.ToString() + obj3.ToString());
                    }
                    return obj2.Compute(@operator, obj3);
                }
                case Scorpio.Compiler.TokenType.Minus:
                case Scorpio.Compiler.TokenType.Multiply:
                case Scorpio.Compiler.TokenType.Divide:
                case Scorpio.Compiler.TokenType.Modulo:
                case Scorpio.Compiler.TokenType.InclusiveOr:
                case Scorpio.Compiler.TokenType.Combine:
                case Scorpio.Compiler.TokenType.XOR:
                case Scorpio.Compiler.TokenType.Shi:
                case Scorpio.Compiler.TokenType.Shr:
                    return obj2.Compute(@operator, this.ResolveOperand(operate.Right));

                case Scorpio.Compiler.TokenType.Or:
                    if (!obj2.LogicOperation())
                    {
                        return this.m_script.CreateBool(this.ResolveOperand(operate.Right).LogicOperation());
                    }
                    return this.m_script.True;

                case Scorpio.Compiler.TokenType.And:
                    if (obj2.LogicOperation())
                    {
                        return this.m_script.CreateBool(this.ResolveOperand(operate.Right).LogicOperation());
                    }
                    return this.m_script.False;

                case Scorpio.Compiler.TokenType.Equal:
                    return this.m_script.CreateBool(obj2.Equals(this.ResolveOperand(operate.Right)));

                case Scorpio.Compiler.TokenType.NotEqual:
                    return this.m_script.CreateBool(!obj2.Equals(this.ResolveOperand(operate.Right)));

                case Scorpio.Compiler.TokenType.Greater:
                case Scorpio.Compiler.TokenType.GreaterOrEqual:
                case Scorpio.Compiler.TokenType.Less:
                case Scorpio.Compiler.TokenType.LessOrEqual:
                    return this.m_script.CreateBool(obj2.Compare(@operator, this.ResolveOperand(operate.Right)));
            }
            throw new ExecutionException(this.m_script, "不支持的运算符 " + @operator);
        }

        private ScriptObject ParseRegion(CodeRegion region)
        {
            return this.ResolveOperand(region.Context);
        }

        private ScriptObject ParseScriptObject(CodeScriptObject obj)
        {
            return obj.Object;
        }

        private ScriptTable ParseTable(CodeTable table)
        {
            ScriptContext context = new ScriptContext(this.m_script, null, this, Executable_Block.None);
            ScriptTable table2 = this.m_script.CreateTable();

            //todo
            //table2.Name = table.StackInfo.Breviary.Substring(table.StackInfo.Breviary.LastIndexOf('/')+1);
            //UnityEngine.Debug.Log(table2.Name);
            foreach (ScriptScriptFunction function in table.Functions)
            {
                table2.SetValue(function.Name, function);
                context.SetVariableForce(function.Name, function);
            }
            foreach (CodeTable.TableVariable variable in table.Variables)
            {
                ScriptObject obj2 = context.ResolveOperand(variable.value);
                table2.SetValue(variable.key, obj2);
                context.SetVariableForce(variable.key.ToString(), obj2);
            }
            return table2;
        }

        private ScriptObject ParseTernary(CodeTernary ternary)
        {
            if (!this.ResolveOperand(ternary.Allow).LogicOperation())
            {
                return this.ResolveOperand(ternary.False);
            }
            return this.ResolveOperand(ternary.True);
        }

        private bool ProcessAllow(TempCondition con)
        {
            if ((con.Allow != null) && !this.ResolveOperand(con.Allow).LogicOperation())
            {
                return false;
            }
            return true;
        }

        private void ProcessBreak()
        {
            this.InvokeBreak(this.m_scriptInstruction.operand0);
        }

        private void ProcessCallBlock()
        {
            this.ParseCallBlock((CodeCallBlock) this.m_scriptInstruction.operand0);
        }

        private void ProcessCallFor()
        {
            CodeFor @for = (CodeFor) this.m_scriptInstruction.operand0;
            ScriptContext parent = new ScriptContext(this.m_script, null, this, Executable_Block.For);
            parent.Execute(@for.BeginExecutable);
            while ((@for.Condition == null) || parent.ResolveOperand(@for.Condition).LogicOperation())
            {
                ScriptContext context2 = new ScriptContext(this.m_script, @for.BlockExecutable, parent, Executable_Block.For);
                context2.Execute();
                if (context2.IsOver)
                {
                    return;
                }
                parent.Execute(@for.LoopExecutable);
            }
            return;
        }

        private void ProcessCallForeach()
        {
            object obj3;
            CodeForeach @foreach = (CodeForeach) this.m_scriptInstruction.operand0;
            ScriptObject obj2 = this.ResolveOperand(@foreach.LoopObject);
            if (!(obj2 is ScriptFunction))
            {
                throw new ExecutionException(this.m_script, "foreach函数必须返回一个ScriptFunction");
            }
            ScriptFunction function = (ScriptFunction) obj2;
        Label_003E:
            obj3 = function.Call();
            if (obj3 != null)
            {
                ScriptContext context = new ScriptContext(this.m_script, @foreach.BlockExecutable, this, Executable_Block.Foreach);
                context.Initialize(@foreach.Identifier, this.m_script.CreateObject(obj3));
                context.Execute();
                if (!context.IsOver)
                {
                    goto Label_003E;
                }
            }
        }

        private void ProcessCallForSimple()
        {
            int num3;
            CodeForSimple simple = (CodeForSimple) this.m_scriptInstruction.operand0;
            ScriptNumber number = this.ResolveOperand(simple.Begin) as ScriptNumber;
            if (number == null)
            {
                throw new ExecutionException(this.m_script, "forsimple 初始值必须是number");
            }
            ScriptNumber number2 = this.ResolveOperand(simple.Finished) as ScriptNumber;
            if (number2 == null)
            {
                throw new ExecutionException(this.m_script, "forsimple 最大值必须是number");
            }
            int num = number.ToInt32();
            int num2 = number2.ToInt32();
            if (simple.Step != null)
            {
                ScriptNumber number3 = this.ResolveOperand(simple.Step) as ScriptNumber;
                if (number3 == null)
                {
                    throw new ExecutionException(this.m_script, "forsimple Step必须是number");
                }
                num3 = number3.ToInt32();
            }
            else
            {
                num3 = 1;
            }
            for (double i = num; i <= num2; i += num3)
            {
                ScriptContext context = new ScriptContext(this.m_script, simple.BlockExecutable, this, Executable_Block.For);
                context.Initialize(simple.Identifier, new ScriptNumberDouble(this.m_script, i));
                context.Execute();
                if (context.IsOver)
                {
                    return;
                }
            }
        }

        private void ProcessCallFunction()
        {
            this.ParseCall((CodeCallFunction) this.m_scriptInstruction.operand0, false);
        }

        private void ProcessCallIf()
        {
            CodeIf @if = (CodeIf) this.m_scriptInstruction.operand0;
            if (this.ProcessAllow(@if.If))
            {
                this.ProcessCondition(@if.If);
            }
            else
            {
                foreach (TempCondition condition in @if.ElseIf)
                {
                    if (this.ProcessAllow(condition))
                    {
                        this.ProcessCondition(condition);
                        return;
                    }
                }
                if ((@if.Else != null) && this.ProcessAllow(@if.Else))
                {
                    this.ProcessCondition(@if.Else);
                }
            }
        }

        private void ProcessCallSwitch()
        {
            CodeSwitch switch2 = (CodeSwitch) this.m_scriptInstruction.operand0;
            ScriptObject obj2 = this.ResolveOperand(switch2.Condition);
            bool flag = false;
            foreach (TempCase @case in switch2.Cases)
            {
                foreach (CodeObject obj3 in @case.Allow)
                {
                    if (this.ResolveOperand(obj3).Equals(obj2))
                    {
                        flag = true;
                        new ScriptContext(this.m_script, @case.Executable, this, Executable_Block.Switch).Execute();
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (!flag && (switch2.Default != null))
            {
                new ScriptContext(this.m_script, switch2.Default.Executable, this, Executable_Block.Switch).Execute();
            }
        }

        private void ProcessCallWhile()
        {
            CodeWhile @while = (CodeWhile) this.m_scriptInstruction.operand0;
            TempCondition condition = @while.While;
            ScriptContext context = new ScriptContext(m_script,condition.Executable,this,Executable_Block.While);
            TempCondition con = @while.While;
            do
            {
                if (!this.ProcessAllow(con))
                {
                    return;
                }
                new ScriptContext(this.m_script, con.Executable, this, Executable_Block.While).Execute();
            }
            while (!context.IsOver);
        }

        private void ProcessCondition(TempCondition condition)
        {
            new ScriptContext(this.m_script, condition.Executable, this, condition.Block).Execute();
        }

        private void ProcessContinue()
        {
            this.InvokeContinue(this.m_scriptInstruction.operand0);
        }

        private void ProcessMov()
        {
            this.SetVariable(this.m_scriptInstruction.operand0 as CodeMember, this.ResolveOperand(this.m_scriptInstruction.operand1));
        }

        private void ProcessResolve()
        {
            this.ResolveOperand(this.m_scriptInstruction.operand0);
        }

        private void ProcessRet()
        {
            if (this.m_scriptInstruction.operand0 == null)
            {
                this.InvokeReturnValue(null);
            }
            else
            {
                this.InvokeReturnValue(this.ResolveOperand(this.m_scriptInstruction.operand0));
            }
        }

        private void ProcessThrow()
        {
            throw new InteriorException(this.ResolveOperand(((CodeThrow) this.m_scriptInstruction.operand0).obj));
        }

        private void ProcessTry()
        {
            CodeTry @try = (CodeTry) this.m_scriptInstruction.operand0;
            try
            {
                new ScriptContext(this.m_script, @try.TryExecutable, this).Execute();
            }
            catch (InteriorException exception)
            {
                ScriptContext context = new ScriptContext(this.m_script, @try.CatchExecutable, this);
                context.Initialize(@try.Identifier, exception.obj);
                context.Execute();
            }
            catch (Exception exception2)
            {
                ScriptContext context2 = new ScriptContext(this.m_script, @try.CatchExecutable, this);
                context2.Initialize(@try.Identifier, this.m_script.CreateObject(exception2));
                context2.Execute();
            }
        }

        private void ProcessVar()
        {
            this.ApplyVariableObject(this.m_scriptInstruction.opvalue);
        }

        private void Reset()
        {
            this.m_returnObject = null;
            this.m_Over = false;
            this.m_Break = false;
            this.m_Continue = false;
        }

        private ScriptObject ResolveOperand(CodeObject value)
        {
            this.m_script.SetStackInfo(value.StackInfo);
            ScriptObject obj2 = this.ResolveOperand_impl(value);
            if (value.Not)
            {
                return this.m_script.CreateBool(!obj2.LogicOperation());
            }
            if (value.Minus)
            {
                ScriptNumber number = obj2 as ScriptNumber;
                if (number == null)
                {
                    throw new ExecutionException(this.m_script, "Script Object Type [" + obj2.Type + "] is cannot use [-] sign");
                }
                return number.Minus();
            }
            if (!value.Negative)
            {
                return obj2;
            }
            ScriptNumber number2 = obj2 as ScriptNumber;
            if (number2 == null)
            {
                throw new ExecutionException(this.m_script, "Script Object Type [" + obj2.Type + "] is cannot use [~] sign");
            }
            return number2.Negative();
        }

        private ScriptObject ResolveOperand_impl(CodeObject value)
        {
            if (value is CodeScriptObject)
            {
                return this.ParseScriptObject((CodeScriptObject) value);
            }
            if (value is CodeRegion)
            {
                return this.ParseRegion((CodeRegion) value);
            }
            if (value is CodeFunction)
            {
                return this.ParseFunction((CodeFunction) value);
            }
            if (value is CodeCallFunction)
            {
                return this.ParseCall((CodeCallFunction) value, true);
            }
            if (value is CodeMember)
            {
                return this.GetVariable((CodeMember) value);
            }
            if (value is CodeArray)
            {
                return this.ParseArray((CodeArray) value);
            }
            if (value is CodeTable)
            {
                return this.ParseTable((CodeTable) value);
            }
            if (value is CodeOperator)
            {
                return this.ParseOperate((CodeOperator) value);
            }
            if (value is CodeTernary)
            {
                return this.ParseTernary((CodeTernary) value);
            }
            if (value is CodeAssign)
            {
                return this.ParseAssign((CodeAssign) value);
            }
            if (value is CodeEval)
            {
                return this.ParseEval((CodeEval) value);
            }
            return this.m_script.Null;
        }

        private void SetVariable(CodeMember member, ScriptObject variable)
        {
            if (member.Parent == null)
            {
                string memberValue = (string) member.MemberValue;
                if (!this.SetVariableObject(memberValue, variable))
                {
                    this.m_script.SetObjectInternal(memberValue, variable);
                }
            }
            else
            {
                this.ResolveOperand(member.Parent).SetValue(this.GetMember(member), variable);
            }
        }

        private void SetVariableForce(string name, ScriptObject obj)
        {
            this.m_variableDictionary[name] = obj.Assign();
        }

        private bool SetVariableObject(string name, ScriptObject obj)
        {
            if (this.m_variableDictionary.ContainsKey(name))
            {
                this.m_variableDictionary[name] = obj.Assign();
                return true;
            }
            return ((this.m_parent != null) && this.m_parent.SetVariableObject(name, obj));
        }

        private bool SupportBreak()
        {
            if ((this.m_block != Executable_Block.For) && (this.m_block != Executable_Block.Foreach))
            {
                return (this.m_block == Executable_Block.While);
            }
            return true;
        }

        private bool SupportContinue()
        {
            if ((this.m_block != Executable_Block.For) && (this.m_block != Executable_Block.Foreach))
            {
                return (this.m_block == Executable_Block.While);
            }
            return true;
        }

        private bool SupportReturnValue()
        {
            if (this.m_block != Executable_Block.Function)
            {
                return (this.m_block == Executable_Block.Context);
            }
            return true;
        }

        private bool IsExecuted
        {
            get
            {
                if (!this.m_Break && !this.m_Over)
                {
                    return this.m_Continue;
                }
                return true;
            }
        }

        private bool IsOver
        {
            get
            {
                if (!this.m_Break)
                {
                    return this.m_Over;
                }
                return true;
            }
        }
    }
}

