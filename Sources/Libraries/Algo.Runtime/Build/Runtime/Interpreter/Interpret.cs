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
    /// <summary>
    /// Provides a basic class for an interpreter
    /// </summary>
    internal abstract class Interpret : MemoryTraceObject, IDisposable
    {
        #region Fields

        private bool _failed;
        private bool _stopped;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a <see cref="InterpreterType"/> used to identify the object without reflection
        /// </summary>
        [JsonIgnore]
        internal abstract InterpreterType InterpreterType { get; }

        /// <summary>
        /// Gets or sets the list of variables in the current interpreter
        /// </summary>
        [JsonProperty]
        internal Collection<Variable> Variables { get; set; }

        /// <summary>
        /// Gets if this or any parent interpreter has asked to stop or has throw an error
        /// </summary>
        [JsonIgnore]
        internal bool FailedOrStop
        {
            get
            {
                return Failed || Stopped;
            }
        }

        /// <summary>
        /// Gets or sets if this or any parent interpreter has throw an error
        /// </summary>
        [JsonIgnore]
        protected bool Failed
        {
            get
            {
                SetParentProgramInterpreter();
                if (ParentProgramInterpreter == null)
                {
                    return _failed;
                }
                _failed = ParentProgramInterpreter.Failed;
                return ParentProgramInterpreter.Failed;
            }
            private set
            {
                _failed = value;
            }
        }

        /// <summary>
        /// Gets or sets if this or any parent interpreter has asked to stop
        /// </summary>
        [JsonIgnore]
        protected bool Stopped
        {
            get
            {
                SetParentProgramInterpreter();
                if (ParentProgramInterpreter == null)
                {
                    return _stopped;
                }
                _stopped = ParentProgramInterpreter.Stopped;
                return ParentProgramInterpreter.Stopped;
            }
            set
            {
                _stopped = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="ProgramInterpreter"/>
        /// </summary>
        [JsonIgnore]
        internal ProgramInterpreter ParentProgramInterpreter { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="ClassInterpreter"/>
        /// </summary>
        [JsonIgnore]
        internal ClassInterpreter ParentClassInterpreter { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="MethodInterpreter"/>
        /// </summary>
        [JsonIgnore]
        internal MethodInterpreter ParentMethodInterpreter { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="BlockInterpreter"/>
        /// </summary>
        [JsonIgnore]
        internal BlockInterpreter ParentBlockInterpreter { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the state of the algorithm interpreter has changed
        /// </summary>
        internal event AlgorithmInterpreterStateEventHandler StateChanged;

        #endregion

        #region Handlers

        /// <summary>
        /// Invoked to get the parent interpreter
        /// </summary>
        internal Func<Interpret> OnGetParentInterpreter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="Interpret"/>
        /// </summary>
        /// <param name="debugMode">defines is the debug mode is enabled or not</param>
        internal Interpret(bool debugMode)
            : base(debugMode)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize, after the constructor, the other properties
        /// </summary>
        internal abstract void Initialize();

        /// <summary>
        /// Dispose the resources
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Change the state of the current interpreter
        /// </summary>
        /// <param name="source">The source from where we changed the state (an interpreter usually)</param>
        /// <param name="e">The new state</param>
        internal virtual void ChangeState(object source, AlgorithmInterpreterStateEventArgs e)
        {
            if (e.State == AlgorithmInterpreterState.StoppedWithError)
            {
                Failed = true;
            }
            else if (e.State == AlgorithmInterpreterState.Stopped)
            {
                Stopped = true;
            }

            if (StateChanged != null)
            {
                StateChanged(source, e);
            }
        }

        /// <summary>
        /// Add a new log
        /// </summary>
        /// <param name="source">The source from where we changed the state (an interpreter usually)</param>
        /// <param name="format">the message format</param>
        /// <param name="args">the message arguments</param>
        internal void Log(object source, string format, params object[] args)
        {
            ChangeState(source, new AlgorithmInterpreterStateEventArgs(string.Format(format, args)));
        }

        /// <summary>
        /// Add a new log
        /// </summary>
        /// <param name="source">The source from where we changed the state (an interpreter usually)</param>
        /// <param name="log">the log message</param>
        internal void Log(object source, string log)
        {
            ChangeState(source, new AlgorithmInterpreterStateEventArgs(log));
        }

        /// <summary>
        /// Returns the parent interpreter
        /// </summary>
        /// <returns>Returns the parent interpreter, if exists</returns>
        internal Interpret GetParentInterpreter()
        {
            if (InterpreterType == InterpreterType.ProgramInterpreter)
            {
                return null;
            }
            if (OnGetParentInterpreter == null)
            {
                ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new NullReferenceException("OnGetParentInterpreter handler is null")), GetDebugInfo()));
                return null;
            }

            return OnGetParentInterpreter();
        }

        /// <summary>
        /// Returns the first parent interpreter that correspond to the specified <see cref="InterpreterType"/>
        /// </summary>
        /// <param name="interpreterType">The interpreter type</param>
        /// <param name="allowCurrent">Defines if we can consider the current interpreter as the next first parent</param>
        /// <returns>Returns the corresponding interpreter</returns>
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

        /// <summary>
        /// Add a variable to the current interpreter
        /// </summary>
        /// <param name="variable">The variable we wand to add</param>
        /// <param name="defaultValue">The variable default value</param>
        /// <param name="isArg">Defines if this variable is an argument of a method</param>
        internal void AddVariable(IAlgorithmVariable variable, object defaultValue = null, bool isArg = false)
        {
            if (FindVariable(variable.Name.ToString()) != null)
            {
                ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new VariableAlreadyExistsException(variable.Name.ToString())), GetDebugInfo()));
                return;
            }

            var location = string.Empty;

            if (DebugMode)
            {
                if (isArg)
                {
                    location = "method's argument";
                }
                else
                {
                    switch (InterpreterType)
                    {
                        case InterpreterType.ProgramInterpreter:
                            location = "program";
                            break;
                        case InterpreterType.ClassInterpreter:
                            location = "class";
                            break;
                        default:
                            location = "method";
                            break;
                    }
                }
            }

            if (defaultValue == null && variable.DefaultValue != null)
            {
                defaultValue = variable.DefaultValue.Value;
            }
            Variables.Add(new Variable(variable.Name.ToString(), DebugMode, defaultValue, variable.IsArray));

            if (DebugMode)
            {
                Log(this, "Variable '{0}' declared in the {1} => IsArray:{2}, DefaultValue:{3}", variable.Name, location, variable.IsArray, defaultValue == null ? "{null}" : defaultValue.ToString());
            }
        }

        /// <summary>
        /// Find the first accessible variable in this interpreter and its parent with the specified name
        /// </summary>
        /// <param name="variableName">The variable name</param>
        /// <returns>Returns the variable</returns>
        internal Variable FindVariable(string variableName)
        {
            return GetAllAccessibleVariable().FirstOrDefault(va => va.Name == variableName);
            /*  var interpreter = this;
            Variable variable;

            do
            {
                variable = interpreter.Variables.FirstOrDefault(va => va.Name == variableName);
                if (variable == null)
                {
                    interpreter = interpreter.GetParentInterpreter();
                }
            } while (variable == null && interpreter != null);

            return variable; */
        }

        /// <summary>
        /// Find the first variable in the current interpret with the specified name
        /// </summary>
        /// <param name="variableName">The variable name</param>
        /// <returns>Returns the variable</returns>
        internal Variable FindVariableInTheCurrentInterpreter(string variableName)
        {
            return Variables.FirstOrDefault(va => va.Name == variableName);
        }

        /// <summary>
        /// Returns all accessible variables from the current interpreter and its parents
        /// </summary>
        /// <returns>Returns the list of available variables</returns>
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

        /// <summary>
        /// Returns all the debug information we have at the current time
        /// </summary>
        /// <param name="clearCallStack">Defines if the user call stack must be cleared</param>
        /// <returns>Returns the current debug information</returns>
        internal DebugInfo GetDebugInfo(bool clearCallStack = true)
        {
            SetParentProgramInterpreter();
            if (ParentProgramInterpreter == null)
            {
                throw new NullReferenceException("The parent program interpreter is null.");
            }

            var debugInfo = ParentProgramInterpreter.DebugInfo;

            if (DebugMode && InterpreterType == InterpreterType.BlockInterpreter)
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

        /// <summary>
        /// Sets the parent <see cref="ProgramInterpreter"/>
        /// </summary>
        private void SetParentProgramInterpreter()
        {
            if (ParentProgramInterpreter == null)
            {
                var interpreter = GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
                if (interpreter != null)
                {
                    ParentProgramInterpreter = (ProgramInterpreter)interpreter;
                }
            }
        }

        #endregion
    }
}
