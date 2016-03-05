using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.Runtime.Debugger.CallStack;
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
        private ProgramInterpreter parentProgram;

        #endregion

        #region Properties

        internal Collection<Variable> Variables { get; set; }

        [JsonProperty]
        internal Call Call { get; private set; }

        [JsonIgnore]
        internal bool Failed
        {
            get
            {
                if (parentProgram == null)
                {
                    parentProgram = GetFirstNextParentInterpreter<ProgramInterpreter>();
                }
                if (parentProgram == null)
                {
                    return _failed;
                }
                _failed = parentProgram.Failed;
                return parentProgram.Failed;
            }
            private set
            {
                _failed = value;
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

            if (StateChanged != null)
            {
                StateChanged(source, e);
            }
        }

        internal void Log(object source, string format, params object[] args)
        {
            if (MemTrace)
            {
                ChangeState(source, new SimulatorStateEventArgs(string.Format(format, args)));
            }
        }

        internal void Log(object source, string log)
        {
            if (MemTrace)
            {
                ChangeState(source, new SimulatorStateEventArgs(log));
            }
        }

        internal Interpret GetParentInterpreter()
        {
            if (this is ProgramInterpreter)
            {
                return null;
            }
            if (OnGetParentInterpreter == null)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new NullReferenceException("OnGetParentInterpreter handler is null"), GetDebugInfo())));
                return null;
            }

            return OnGetParentInterpreter();
        }

        internal T GetFirstNextParentInterpreter<T>(bool allowCurrent = true) where T : Interpret
        {
            if (this is ProgramInterpreter)
            {
                return null;
            }

            var val = this as T;
            if (allowCurrent && val != null)
            {
                return val;
            }

            Interpret parent = this;

            do
            {
                parent = parent.GetParentInterpreter();
            } while (parent != null && parent.GetType() != typeof(T));

            return (T)parent;
        }

        internal void AddVariable(IAlgorithmVariable variable, object defaultValue = null, bool isArg = false)
        {
            if (FindVariable(variable.Name.ToString()) != null)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new VariableAlreadyExistsException(variable.Name.ToString()), GetDebugInfo())));
                return;
            }

            string location;

            if (isArg)
            {
                location = "method's argument";
            }
            else if (this is ProgramInterpreter)
            {
                location = "program";
            }
            else if (this is ClassInterpreter)
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

            Log(this, "Variable '{0}' declared in the {1} => IsArray:{2}, DefaultValue:{3}", variable.Name, location, variable.IsArray, defaultValue == null ? "{null}" : defaultValue.ToString());
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

        internal void UpdateCallStack()
        {
            if (!MemTrace)
            {
                return;
            }

            Call = new Call(GetAllAccessibleVariable().DeepClone());
        }

        internal DebugInfo GetDebugInfo()
        {
            var interpreter = this;
            var callStack = new CallStack();

            UpdateCallStack();

            do
            {
                callStack.Add(interpreter.Call);
                interpreter = interpreter.GetParentInterpreter();
            } while (interpreter != null);

            var debugInfo = new DebugInfo(callStack);
            return debugInfo;
        }

        #endregion
    }
}
