using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;
using Algo.Runtime.ComponentModel;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter
{
    internal abstract class Interpret : MemoryTraceObject, IDisposable
    {
        #region Fields

        private bool _failed;
        private bool _stopped;
        private ProgramInterpreter _parentProgram;

        #endregion

        #region Properties

        [JsonIgnore]
        internal abstract InterpreterType InterpreterType { get; }

        [JsonProperty]
        internal Collection<Variable> Variables { get; set; }

        [JsonIgnore]
        internal bool FailedOrStop
        {
            get
            {
                return Failed || Stopped;
            }
        }

        [JsonIgnore]
        protected bool Failed
        {
            get
            {
                SetParentProgramInterpreter();
                if (_parentProgram == null)
                {
                    return _failed;
                }
                _failed = _parentProgram.Failed;
                return _parentProgram.Failed;
            }
            private set
            {
                _failed = value;
            }
        }

        [JsonIgnore]
        protected bool Stopped
        {
            get
            {
                SetParentProgramInterpreter();
                if (_parentProgram == null)
                {
                    return _stopped;
                }
                _stopped = _parentProgram.Stopped;
                return _parentProgram.Stopped;
            }
            set
            {
                _stopped = value;
            }
        }

        #endregion

        #region Events

        internal event SimulatorStateEventHandler StateChanged;

        #endregion

        #region Handlers

        internal Func<Interpret> OnGetParentInterpreter;

        #endregion

        #region Constructors

        internal Interpret(bool memTrace)
            : base(memTrace)
        {
        }

        #endregion

        #region Methods

        internal abstract void Initialize();

        public abstract void Dispose();

        internal virtual void ChangeState(object source, SimulatorStateEventArgs e)
        {
            if (e.State == SimulatorState.StoppedWithError)
            {
                Failed = true;
            }
            else if (e.State == SimulatorState.Stopped)
            {
                Stopped = true;
            }

            if (StateChanged != null)
            {
                StateChanged(source, e);
            }
        }

        internal void Log(object source, string format, params object[] args)
        {
            ChangeState(source, new SimulatorStateEventArgs(string.Format(format, args)));
        }

        internal void Log(object source, string log)
        {
            ChangeState(source, new SimulatorStateEventArgs(log));
        }

        internal Interpret GetParentInterpreter()
        {
            if (InterpreterType == InterpreterType.ProgramInterpreter)
            {
                return null;
            }
            if (OnGetParentInterpreter == null)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new NullReferenceException("OnGetParentInterpreter handler is null")), GetDebugInfo()));
                return null;
            }

            return OnGetParentInterpreter();
        }

        internal Interpret GetFirstNextParentInterpreter(InterpreterType interpreterType, bool allowCurrent = true)
        {
            if (InterpreterType == InterpreterType.ProgramInterpreter)
            {
                return null;
            }

            if (allowCurrent && InterpreterType == interpreterType)
            {
                return this;
            }

            var parent = this;

            do
            {
                parent = parent.GetParentInterpreter();
            } while (parent != null && parent.InterpreterType != interpreterType);

            return parent;
        }

        internal void AddVariable(IAlgorithmVariable variable, object defaultValue = null, bool isArg = false)
        {
            if (MemTrace && FindVariable(variable.Name.ToString()) != null)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new VariableAlreadyExistsException(variable.Name.ToString())), GetDebugInfo()));
                return;
            }

            string location;

            if (isArg)
            {
                location = "method's argument";
            }
            else if (InterpreterType == InterpreterType.ProgramInterpreter)
            {
                location = "program";
            }
            else if (InterpreterType == InterpreterType.ClassInterpreter)
            {
                location = "class";
            }
            else
            {
                location = "method";
            }

            if (defaultValue == null && variable.DefaultValue != null)
            {
                defaultValue = variable.DefaultValue.Value;
            }
            Variables.Add(new Variable(variable.Name.ToString(), MemTrace, defaultValue, variable.IsArray));

            if (MemTrace)
            {
                Log(this, "Variable '{0}' declared in the {1} => IsArray:{2}, DefaultValue:{3}", variable.Name, location, variable.IsArray, defaultValue == null ? "{null}" : defaultValue.ToString());
            }
        }

        internal Variable FindVariable(string variableName)
        {
            var interpreter = this;
            Variable variable;

            do
            {
                variable = interpreter.Variables.FirstOrDefault(va => va.Name == variableName);
                if (variable == null)
                {
                    interpreter = interpreter.GetParentInterpreter();
                }
            } while (variable == null && interpreter != null);

            return variable;
        }

        internal Variable FindVariableInTheCurrentInterpreter(string variableName)
        {
            return Variables.FirstOrDefault(va => va.Name == variableName);
        }

        internal ReadOnlyCollection<Variable> GetAllAccessibleVariable()
        {
            var variables = new List<Variable>();
            var interpreter = this;

            do
            {
                variables.AddRange(interpreter.Variables);
                interpreter = interpreter.GetParentInterpreter();
            } while (interpreter != null);

            return variables.AsReadOnly();
        }

        internal DebugInfo GetDebugInfo(bool clearCallStack = true)
        {
            SetParentProgramInterpreter();
            if (_parentProgram == null)
            {
                throw new NullReferenceException("The parent program interpreter is null.");
            }

            var debugInfo = _parentProgram.DebugInfo;

            if (MemTrace && this is BlockInterpreter)
            {
                var interpreter = GetFirstNextParentInterpreter(InterpreterType.MethodInterpreter);
                if (interpreter != null)
                {
                    var methodInterpreter = (MethodInterpreter)interpreter;
                    var callStack = debugInfo.CallStackService.CallStacks.Single(cs => cs.TaceId == methodInterpreter.StacktraceId);
                    var call = callStack.Stack.Pop();
                    if (call != null)
                    {
                        call.Variables = GetAllAccessibleVariable().DeepClone();
                        callStack.Stack.Push(call);
                    }
                }
            }

            if (clearCallStack)
            {
                debugInfo.CallStackService.CallCount = 0;
                debugInfo.CallStackService.StackTraceCallCount.Clear();
            }

            return debugInfo;
        }

        private void SetParentProgramInterpreter()
        {
            if (_parentProgram == null)
            {
                var interpreter = GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
                if (interpreter != null)
                {
                    _parentProgram = (ProgramInterpreter)interpreter;
                }
            }
        }

        #endregion
    }
}
