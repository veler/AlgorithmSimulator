using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Memory;
using Algo.Runtime.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM;
using Algo.Runtime.Build.Runtime.Debugger.CallStack;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    /// <summary>
    /// Represents the instance of a class in a algorithm at runtime
    /// </summary>
    internal sealed class ClassInterpreter : Interpret
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="InterpreterType"/> used to identify the object without reflection
        /// </summary>
        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.ClassInterpreter;

        /// <summary>
        /// Gets or sets the class declaration
        /// </summary>
        [JsonProperty]
        internal AlgorithmClassDeclaration ClassDeclaration { get; private set; }

        /// <summary>
        /// Gets or sets if the current <see cref="ClassInterpreter"/> is an instance
        /// </summary>
        [JsonProperty]
        internal bool IsInstance { get; private set; }

        /// <summary>
        /// Gets or sets the list of the constructors interpreter
        /// </summary>
        [JsonProperty]
        internal Collection<MethodInterpreter> Constructors { get; private set; }

        /// <summary>
        /// Gets or sets the list of the methods interpreter. It does not includes the constructors.
        /// </summary>
        [JsonProperty]
        internal Collection<MethodInterpreter> Methods { get; private set; }

        /// <summary>
        /// Gets or sets the program entry point interpreter, if it is in the current class
        /// </summary>
        [JsonProperty]
        internal MethodInterpreter EntryPoint { get; private set; }

        #endregion

        #region Handlers

        /// <summary>
        /// Raised when a method or a block is done
        /// </summary>
        internal Action<ClassInterpreter> OnDone;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="ClassInterpreter"/>
        /// </summary>
        /// <param name="classDeclaration">The class declaration</param>
        /// <param name="debugMode">Defines if the debug mode is enabled</param>
        internal ClassInterpreter(AlgorithmClassDeclaration classDeclaration, bool debugMode)
            : base(debugMode)
        {
            ClassDeclaration = classDeclaration;
        }

        #endregion

        #region Methods          

        /// <summary>
        /// Initialize, after the constructor, the other properties
        /// </summary>
        internal override void Initialize()
        {
            AlgorithmClassMember member;
            var i = 0;

            IsInstance = true;
            Variables = new Collection<Variable>();
            Constructors = new Collection<MethodInterpreter>();
            Methods = new Collection<MethodInterpreter>();

            while (i < ClassDeclaration.Members.Count && !FailedOrStop)
            {
                member = ClassDeclaration.Members[i];

                switch (member.DomType)
                {
                    case AlgorithmDomType.ClassPropertyDeclaration:
                        AddVariable((IAlgorithmVariable)member);
                        break;

                    case AlgorithmDomType.ClassConstructorDeclaration:
                        if (Constructors.Any(c => c.MethodDeclaration._arguments.Count == member._arguments.Count))
                        {
                            ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new IdenticalConstructorsException(ClassDeclaration.Name.ToString(), "A class should not have multiple constructors with the same number of arguments.")), GetDebugInfo()));
                        }
                        else
                        {
                            Constructors.Add(new MethodInterpreter(member, DebugMode));
                        }
                        break;

                    case AlgorithmDomType.EntryPointMethod:
                        EntryPoint = new MethodInterpreter(member, DebugMode);
                        break;

                    case AlgorithmDomType.ClassMethodDeclaration:
                        Methods.Add(new MethodInterpreter(member, DebugMode));
                        break;
                }

                i++;
            }
        }

        /// <summary>
        /// Create a new instance of the current class interpreter.
        /// </summary>
        /// <returns>returns the new object</returns>
        internal ClassInterpreter CreateNewInstance()
        {
            var instance = new ClassInterpreter(ClassDeclaration, DebugMode);
            return instance;
        }

        /// <summary>
        /// Call the constructor of the class, if exists.
        /// </summary>
        /// <param name="arguments">the argument values</param>
        internal void CreateNewInstanceCallConstructors(Collection<object> arguments)
        {
            if (arguments == null)
            {
                arguments = new Collection<object>();
            }

            var constructor = FindCorrespondingConstructor(arguments.Count);

            if (constructor == null)
            {
                if (Constructors.Count > 0)
                {
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException("ctor", $"There is no constructor with {arguments.Count} argument(s) in the class '{ClassDeclaration.Name}'.")), GetDebugInfo()));
                }
            }
            else
            {
                if (DebugMode)
                {
                    Log(this, $"Calling a constructor of '{ClassDeclaration.Name}'");
                }
                constructor = new MethodInterpreter(constructor.MethodDeclaration, DebugMode);
                constructor.StateChanged += ChangeState;
                constructor.OnGetParentInterpreter += new Func<ClassInterpreter>(() => this);
                constructor.OnDone += new Action<MethodInterpreter>((met) =>
                {
                    met.Dispose();
                    met.StateChanged -= ChangeState;
                });
                constructor.Initialize();
                constructor.Run(false, arguments, Guid.Empty);
            }
        }

        /// <summary>
        /// Find the constructor which correspond to the given argument count
        /// </summary>
        /// <param name="argumentCount">The number of argument to search</param>
        /// <returns>Returns null if the constructor has not be found</returns>
        private MethodInterpreter FindCorrespondingConstructor(int argumentCount)
        {
            MethodInterpreter constructor = null;
            var i = 0;

            while (i < Constructors.Count && constructor == null)
            {
                if (Constructors[i].MethodDeclaration._arguments.Count == argumentCount)
                {
                    constructor = Constructors[i];
                }
                i++;
            }

            return constructor;
        }

        /// <summary>
        /// Invoke the specified method
        /// </summary>
        /// <param name="callerInterpreter">The caller interpreter (usually a block)</param>
        /// <param name="invokeExpression">The invoke expression</param>
        /// <param name="argumentValues">The list of argument values</param>
        /// <param name="parentMethodInterpreter">The parent method interpret from where the invocation happened</param>
        /// <param name="callStackService">The user call stack service</param>
        /// <returns>Returns the result of the method</returns>
        internal object InvokeMethod(Interpret callerInterpreter, AlgorithmExpression invokeExpression, Collection<object> argumentValues, MethodInterpreter parentMethodInterpreter, CallStackService callStackService)
        {
            var methodName = invokeExpression._methodName.ToString();
            var method = Methods.FirstOrDefault(m => m.MethodDeclaration._name.ToString() == methodName);

            if (method == null)
            {
                callerInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotFoundException(methodName, $"The method '{methodName}' does not exists in the current class or is not accessible."), ClassDeclaration), callerInterpreter.GetDebugInfo()));
                return null;
            }

            if (!method.MethodDeclaration._isAsync && invokeExpression._await)
            {
                callerInterpreter.ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new MethodNotAwaitableException(methodName), method.MethodDeclaration), callerInterpreter.GetDebugInfo()));
                return null;
            }

            Guid stackTraceId;
            var isAsync = method.MethodDeclaration._isAsync;

            if (parentMethodInterpreter == null)
            {
                stackTraceId = Guid.Empty;
            }
            else
            {
                stackTraceId = parentMethodInterpreter.StacktraceId;
                if (DebugMode)
                {
                    var callStack = callStackService.CallStacks.Single(cs => cs.TaceId == stackTraceId);
                    var call = callStack.Stack.Pop();
                    if (call != null)
                    {
                        call.Variables = callerInterpreter.GetAllAccessibleVariable().DeepClone();
                        callStack.Stack.Push(call);
                    }
                }
            }

            method = new MethodInterpreter(method.MethodDeclaration, DebugMode);
            method.StateChanged += ChangeState;
            method.OnGetParentInterpreter += new Func<ClassInterpreter>(() => this);
            method.OnDone += new Action<MethodInterpreter>((met) =>
             {
                 met.Dispose();
                 met.StateChanged -= ChangeState;
             });
            method.Initialize();
            method.Run(invokeExpression._await, argumentValues, stackTraceId);

            if (isAsync && !invokeExpression._await)
            {
                return null;
            }
            return method.ReturnedValue;
        }

        /// <summary>
        /// Dispose the resources
        /// </summary>
        public override void Dispose()
        {
            Task.Run(() =>
            {
                OnDone(this);

                ClassDeclaration = null;

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

                if (Constructors != null)
                {
                    Constructors.Clear();
                }
                Constructors = null;

                if (Methods != null)
                {
                    Methods.Clear();
                }
                Methods = null;

                EntryPoint = null;
            });
        }

        #endregion
    }
}
