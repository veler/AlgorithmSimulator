using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Memory;
using Algo.Runtime.ComponentModel;
using Algo.Runtime.ComponentModel.Types;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    /// <summary>
    /// Represents the instance of a class in a algorithm at runtime
    /// </summary>
    sealed internal class ClassInterpreter : Interpret
    {
        #region Properties

        [JsonProperty]
        internal AlgorithmClassDeclaration ClassDeclaration { get; private set; }

        [JsonProperty]
        internal bool IsInstance { get; private set; }

        [JsonProperty]
        internal Collection<MethodInterpreter> Constructors { get; private set; }

        [JsonProperty]
        internal Collection<MethodInterpreter> Methods { get; private set; }

        [JsonProperty]
        internal MethodInterpreter EntryPoint { get; private set; }

        #endregion

        #region Handlers

        internal Action<ClassInterpreter> OnDone;

        #endregion

        #region Constructors

        internal ClassInterpreter(AlgorithmClassDeclaration classDecl, bool memTrace)
            : base(memTrace)
        {
            ClassDeclaration = classDecl;
        }

        #endregion

        #region Methods          

        internal override void Initialize()
        {
            AlgorithmClassMember member;
            var i = 0;

            IsInstance = true;
            Variables = new Collection<Variable>();
            Constructors = new Collection<MethodInterpreter>();
            Methods = new Collection<MethodInterpreter>();

            while (i < ClassDeclaration.Members.Count && !Failed)
            {
                member = ClassDeclaration.Members[i];
                TypeSwitch.Switch(
                    member,
                    TypeSwitch.Case<AlgorithmClassPropertyDeclaration>(property =>
                    {
                        AddVariable(property);
                    }),
                    TypeSwitch.Case<AlgorithmClassConstructorDeclaration>(ctor =>
                    {
                        if (Constructors.Any(c => c.MethodDeclaration.Arguments.Count == ctor.Arguments.Count))
                        {
                            ChangeState(this, new SimulatorStateEventArgs(new Error(new IdenticalConstructorsException(ClassDeclaration.Name.ToString(), "A class should not have multiple constructors with the same number of arguments."), GetDebugInfo())));
                        }
                        else
                        {
                            Constructors.Add(new MethodInterpreter(ctor, MemTrace));
                        }
                    }),
                    TypeSwitch.Case<AlgorithmEntryPointMethod>(entryPoint =>
                    {
                        EntryPoint = new MethodInterpreter(entryPoint, MemTrace);
                    }),
                    TypeSwitch.Case<AlgorithmClassMethodDeclaration>(method =>
                    {
                        Methods.Add(new MethodInterpreter(method, MemTrace));
                    }));
                i++;
            }
        }

        internal ClassInterpreter CreateNewInstance()
        {
            var instance = new ClassInterpreter(ClassDeclaration, MemTrace);
            return instance;
        }

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
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException("ctor", $"There is no constructor with {arguments.Count} argument(s) in the class '{ClassDeclaration.Name}'."), GetDebugInfo())));
                }
            }
            else
            {
                Log(this, $"Calling a constructor of '{ClassDeclaration.Name}'");
                constructor = new MethodInterpreter(constructor.MethodDeclaration, MemTrace);
                constructor.StateChanged += ChangeState;
                constructor.OnGetParentInterpreter += new Func<ClassInterpreter>(() => this);
                constructor.OnDone += new Action<MethodInterpreter>((met) =>
                {
                    met.Dispose();
                    met.StateChanged -= ChangeState;
                });
                constructor.Initialize();
                constructor.UpdateCallStack();
                constructor.Run(false, arguments);
            }
        }

        private MethodInterpreter FindCorrespondingConstructor(int argumentCount)
        {
            MethodInterpreter constructor = null;
            var i = 0;

            while (i < Constructors.Count && constructor == null)
            {
                if (Constructors[i].MethodDeclaration.Arguments.Count == argumentCount)
                {
                    constructor = Constructors[i];
                }
                i++;
            }

            return constructor;
        }

        internal ClassInterpreter CloneInstance()
        {
            if (!IsInstance)
            {
                ChangeState(this, new SimulatorStateEventArgs(new Error(new NoInstanceReferenceException("Unable to clone a class not instancied."), GetDebugInfo())));
                return null;
            }
            return this.DeepClone();
        }

        internal object CallMethod(Interpret callerInterpreter, AlgorithmInvokeMethodExpression invokeExpression, Collection<object> argumentValues)
        {
            var methodName = invokeExpression.MethodName.ToString();
            var method = Methods.FirstOrDefault(
                m => m.MethodDeclaration.Name.ToString() == methodName);

            if (method == null)
            {
                callerInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotFoundException(methodName, $"The method '{methodName}' does not exists in the current class or is not accessible."), callerInterpreter.GetDebugInfo())));
                return null;
            }

            if (!method.MethodDeclaration.IsAsync && invokeExpression.Await)
            {
                callerInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new MethodNotAwaitableException(methodName), callerInterpreter.GetDebugInfo())));
                return null;
            }

            var isAsync = method.MethodDeclaration.IsAsync;
            method = new MethodInterpreter(method.MethodDeclaration, MemTrace);
            method.StateChanged += ChangeState;
            method.OnGetParentInterpreter += new Func<ClassInterpreter>(() => this);
            method.OnDone += new Action<MethodInterpreter>((met) =>
             {
                 met.Dispose();
                 met.StateChanged -= ChangeState;
             });
            method.Initialize();
            method.UpdateCallStack();
            method.Run(invokeExpression.Await, argumentValues);

            if (isAsync && !invokeExpression.Await)
            {
                return null;
            }
            return method.ReturnedValue;
        }

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
