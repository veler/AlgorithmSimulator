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
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    /// <summary>
    /// Provide a sets of method to interpret a method in an algorithm
    /// </summary>
    internal sealed class MethodInterpreter : Interpret
    {
        #region Fields

        private object _returnedValue;
        private CallStackService _callStackService;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a <see cref="InterpreterType"/> used to identify the object without reflection
        /// </summary>
        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.MethodInterpreter;

        /// <summary>
        /// Gets or sets the method declaration
        /// </summary>
        [JsonProperty]
        internal AlgorithmObject MethodDeclaration { get; set; }

        /// <summary>
        /// Gets or sets a value that defines if the execution is done or not.
        /// </summary>
        [JsonProperty]
        internal bool Done { get; set; }

        /// <summary>
        /// Gets or sets the user stack trace id
        /// </summary>
        [JsonProperty]
        internal Guid StacktraceId { get; set; }

        /// <summary>
        /// Gets or sets the returned value of the method
        /// </summary>
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
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new Exception("A method's returned value must not be an AlgorithmObject")), GetDebugInfo()));
                    return;
                }
                _returnedValue = value;
            }
        }

        #endregion

        #region Handlers
        
        /// <summary>
        /// Raised when a method or a block is done
        /// </summary>
        internal Action<MethodInterpreter> OnDone;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="MethodInterpreter"/>
        /// </summary>
        /// <param name="methodDeclaration">the method declaration</param>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        internal MethodInterpreter(AlgorithmObject methodDeclaration, bool debugMode)
            : base(debugMode)
        {
            MethodDeclaration = methodDeclaration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize, after the constructor, the other properties
        /// </summary>
        internal override void Initialize()
        {
            Variables = new Collection<Variable>();
            ParentProgramInterpreter = (ProgramInterpreter)GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
            ParentClassInterpreter = (ClassInterpreter)GetFirstNextParentInterpreter(InterpreterType.ClassInterpreter);
            _callStackService = ParentProgramInterpreter.DebugInfo.CallStackService;
        }

        /// <summary>
        /// Run the method
        /// </summary>
        /// <param name="awaitCall">defines if we should wait an asynchronous method</param>
        /// <param name="argumentValues">the list of argument values</param>
        /// <param name="stackTraceId">the user stack trace id</param>
        internal void Run(bool awaitCall, IReadOnlyList<object> argumentValues, Guid stackTraceId)
        {
            if (MethodDeclaration._arguments.Count != argumentValues.Count)
            {
                ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException(MethodDeclaration._name.ToString(), $"There is a method '{MethodDeclaration._name}' in the class '{ParentClassInterpreter.ClassDeclaration.Name}', but it does not have {argumentValues.Count} argument(s)."), MethodDeclaration), GetDebugInfo()));
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

        /// <summary>
        /// Execute a method in a new thread
        /// </summary>
        /// <param name="argumentValues">the list of argument values</param>
        /// <param name="mustClearStackAtTheEnd">defines if the user call stack must be cleared at the end of this call</param>
        /// <returns>Returns a task associated to the execution</returns>
        private Task RunAsync(IReadOnlyList<object> argumentValues, bool mustClearStackAtTheEnd)
        {
            return Task.Run(() => RunSync(argumentValues, GenerateNewStackTraceId(), mustClearStackAtTheEnd));
        }

        /// <summary>
        /// Execute a method in the current thread
        /// </summary>
        /// <param name="argumentValues">the list of argument values</param>
        /// <param name="stackTraceId">the user stack trace id</param>
        /// <param name="mustClearStackAtTheEnd">defines if the user call stack must be cleared at the end of this call</param>
        private void RunSync(IReadOnlyList<object> argumentValues, Guid stackTraceId, bool mustClearStackAtTheEnd)
        {
            if (_callStackService.StackTraceCallCount.ContainsKey(stackTraceId))
            {
                if (_callStackService.StackTraceCallCount[stackTraceId] > Consts.CallStackSize)
                {
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new StackOverflowException($"You called too many (more than {Consts.CallStackSize}) methods in the same thread.")), GetParentInterpreter().GetDebugInfo()));
                    return;
                }
                _callStackService.StackTraceCallCount[stackTraceId] = (short)(_callStackService.StackTraceCallCount[stackTraceId] + 1);
            }
            else
            {
                _callStackService.StackTraceCallCount.Add(stackTraceId, 0);
                if (DebugMode)
                {
                    _callStackService.CallStacks.Add(new CallStack(stackTraceId));
                }
            }

            StacktraceId = stackTraceId;
            if (DebugMode)
            {
                var classReference = new AlgorithmClassReferenceExpression(ParentClassInterpreter.ClassDeclaration.Name.ToString());
                var callStack = _callStackService.CallStacks.Single(cs => cs.TaceId == stackTraceId);
                var arguments = new List<AlgorithmExpression>();
                foreach (var argumentValue in argumentValues)
                {
                    arguments.Add(new AlgorithmPrimitiveExpression(argumentValue));
                }
                var call = new Call(classReference, new AlgorithmInvokeMethodExpression(classReference, MethodDeclaration._name.ToString(), arguments.ToArray()));
                callStack.Stack.Push(call);
            }
            
            var block = new BlockInterpreter(MethodDeclaration._statements, DebugMode, ParentProgramInterpreter, this, null, ParentClassInterpreter);
            block.OnGetParentInterpreter += new Func<MethodInterpreter>(() => this);
            block.StateChanged += ChangeState;
            block.Initialize();

            for (var i = 0; i < MethodDeclaration._arguments.Count; i++)
            {
                var argDecl = MethodDeclaration._arguments[i];
                var argValue = argumentValues[i];

                if (!(argValue is string) && argValue is IEnumerable && !argDecl.IsArray)
                {
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new BadArgumentException(argDecl.Name.ToString(), $"The argument's value '{argDecl.Name}' must not be an array of values."), MethodDeclaration), GetParentInterpreter().GetDebugInfo()));
                    return;
                }
                if ((!(argValue is IEnumerable) || argValue is string) && argDecl.IsArray)
                {
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new BadArgumentException(argDecl.Name.ToString(), $"The argument's value '{argDecl.Name}' must be an array of values."), MethodDeclaration), GetParentInterpreter().GetDebugInfo()));
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
                if (DebugMode && !Failed)
                {
                    _callStackService.CallStacks.Remove(_callStackService.CallStacks.Single(callStack => callStack.TaceId == stackTraceId));
                }
            }

            if (DebugMode && !Failed && !mustClearStackAtTheEnd)
            {
                _callStackService.CallStacks.Single(callStack => callStack.TaceId == stackTraceId).Stack.Pop();
            }

            Done = true;
            OnDone(this);
        }

        /// <summary>
        /// Generate a new user stack trace id
        /// </summary>
        /// <returns>Returns a new <see cref="Guid"/></returns>
        private Guid GenerateNewStackTraceId()
        {
            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            } while (_callStackService.StackTraceCallCount.ContainsKey(guid));
            return guid;
        }

        /// <summary>
        /// Dispose the resources
        /// </summary>
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
