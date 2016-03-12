using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Debugger.CallStack;
using Algo.Runtime.Build.Runtime.Memory;
using Algo.Runtime.Build.Runtime.Utils;
using Algo.Runtime.ComponentModel;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    internal sealed class MethodInterpreter : Interpret
    {
        #region Fields

        private object _returnedValue;
        private CallStackService _callStackService;

        #endregion

        #region Properties

        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.MethodInterpreter;

        [JsonProperty]
        internal AlgorithmObject MethodDeclaration { get; set; }

        [JsonProperty]
        internal bool Done { get; set; }

        [JsonProperty]
        internal Guid StacktraceId { get; set; }

        [JsonProperty]
        internal object ReturnedValue
        {
            get
            {
                return _returnedValue;
            }
            set
            {
                if (value is AlgorithmExpression || value is AlgorithmStatement)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new Exception("A method's returned value must not be an AlgorithmObject")), GetDebugInfo()));
                    return;
                }
                _returnedValue = value;
            }
        }

        #endregion

        #region Handlers

        internal Action<MethodInterpreter> OnDone;

        #endregion

        #region Constructors

        internal MethodInterpreter(AlgorithmObject methodDecl, bool memTrace)
            : base(memTrace)
        {
            MethodDeclaration = methodDecl;
        }

        #endregion

        #region Methods

        internal override void Initialize()
        {
            Variables = new Collection<Variable>();
            _callStackService = ((ProgramInterpreter)GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter)).DebugInfo.CallStackService;
        }

        internal void Run(bool awaitCall, IReadOnlyList<object> argumentValues, Guid stackTraceId)
        {
            if (MethodDeclaration._arguments.Count != argumentValues.Count)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException(MethodDeclaration._name.ToString(), $"There is a method '{MethodDeclaration._name}' in the class '{((ClassInterpreter)GetFirstNextParentInterpreter(InterpreterType.ClassInterpreter)).ClassDeclaration.Name}', but it does not have {argumentValues.Count} argument(s).")), GetDebugInfo()));
                return;
            }

            var mustClearStackAtTheEnd = false;

            if (stackTraceId == Guid.Empty)
            {
                mustClearStackAtTheEnd = true;
                stackTraceId = GenerateNewStackTraceId();
            }

            if (MethodDeclaration._isAsync)
            {
                if (awaitCall)
                {
                    RunAsync(argumentValues, mustClearStackAtTheEnd).Wait();
                }
                else
                {
                    RunAsync(argumentValues, mustClearStackAtTheEnd);
                }
            }
            else
            {
                RunSync(argumentValues, stackTraceId, mustClearStackAtTheEnd);
            }
        }

        private Task RunAsync(IReadOnlyList<object> argumentValues, bool mustClearStackAtTheEnd)
        {
            return Task.Run(() => RunSync(argumentValues, GenerateNewStackTraceId(), mustClearStackAtTheEnd));
        }

        private void RunSync(IReadOnlyList<object> argumentValues, Guid stackTraceId, bool mustClearStackAtTheEnd)
        {
            if (_callStackService.StackTraceCallCount.ContainsKey(stackTraceId))
            {
                if (_callStackService.StackTraceCallCount[stackTraceId] > Consts.CallStackSize)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new StackOverflowException($"You called too many (more than {Consts.CallStackSize}) methods in the same thread.")), GetParentInterpreter().GetDebugInfo()));
                    return;
                }
                _callStackService.StackTraceCallCount[stackTraceId] = (short)(_callStackService.StackTraceCallCount[stackTraceId] + 1);
            }
            else
            {
                _callStackService.StackTraceCallCount.Add(stackTraceId, 0);
                if (MemTrace)
                {
                    _callStackService.CallStacks.Add(new CallStack(stackTraceId));
                }
            }

            StacktraceId = stackTraceId;
            if (MemTrace)
            {
                var classReference = new AlgorithmClassReferenceExpression(((ClassInterpreter)GetFirstNextParentInterpreter(InterpreterType.ClassInterpreter)).ClassDeclaration.Name.ToString());
                var callStack = _callStackService.CallStacks.Single(cs => cs.TaceId == stackTraceId);
                var arguments = new List<AlgorithmExpression>();
                foreach (var argumentValue in argumentValues)
                {
                    arguments.Add(new AlgorithmPrimitiveExpression(argumentValue));
                }
                var call = new Call(classReference, new AlgorithmInvokeMethodExpression(classReference, MethodDeclaration._name.ToString(), arguments.ToArray()));
                callStack.Stack.Push(call);
            }

            var program = (ProgramInterpreter)GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
            var block = new BlockInterpreter(MethodDeclaration._statements, MemTrace);
            block.OnGetParentInterpreter += new Func<MethodInterpreter>(() => this);
            block.StateChanged += ChangeState;
            block.Initialize();

            for (var i = 0; i < MethodDeclaration._arguments.Count; i++)
            {
                var argDecl = MethodDeclaration._arguments[i];
                var argValue = argumentValues[i];

                if (!(argValue is string) && argValue is IEnumerable && !argDecl.IsArray)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException(argDecl.Name.ToString(), $"The argument's value '{argDecl.Name}' must not be an array of values.")), GetParentInterpreter().GetDebugInfo()));
                    return;
                }
                if ((!(argValue is IEnumerable) || argValue is string) && argDecl.IsArray)
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new BadArgumentException(argDecl.Name.ToString(), $"The argument's value '{argDecl.Name}' must be an array of values.")), GetParentInterpreter().GetDebugInfo()));
                    return;
                }

                block.AddVariable(argDecl, argValue, true);
            }

            if (FailedOrStop)
            {
                return;
            }

            block.Run();
            block.StateChanged -= ChangeState;
            block.Dispose();

            if (mustClearStackAtTheEnd)
            {
                _callStackService.StackTraceCallCount.Remove(stackTraceId);
                if (MemTrace && !Failed)
                {
                    _callStackService.CallStacks.Remove(_callStackService.CallStacks.Single(callStack => callStack.TaceId == stackTraceId));
                }
            }

            if (MemTrace && !Failed && !mustClearStackAtTheEnd)
            {
                _callStackService.CallStacks.Single(callStack => callStack.TaceId == stackTraceId).Stack.Pop();
            }

            Done = true;
            OnDone(this);
        }

        private Guid GenerateNewStackTraceId()
        {
            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            } while (_callStackService.StackTraceCallCount.ContainsKey(guid));
            return guid;
        }

        public override void Dispose()
        {
            Task.Run(() =>
            {
                if (Variables != null)
                {
                    foreach (var variable in Variables)
                    {
                        var value = variable.Value as IDisposable;
                        if (value != null)
                        {
                            value.Dispose();
                        }
                    }
                    Variables.Clear();
                }
                Variables = null;

                MethodDeclaration = null;
            });
        }

        #endregion
    }
}
