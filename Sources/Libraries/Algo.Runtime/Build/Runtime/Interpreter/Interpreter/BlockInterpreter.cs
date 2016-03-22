using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Statements;
using Algo.Runtime.Build.Runtime.Memory;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    /// <summary>
    /// Provide a sets of method to interpret a block of an algorithm
    /// </summary>
    internal sealed class BlockInterpreter : Interpret
    {
        #region Properties

        /// <summary>
        /// Gets a <see cref="InterpreterType"/> used to identify the object without reflection
        /// </summary>
        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.BlockInterpreter;

        /// <summary>
        /// Gets or sets the list of statements to interpret
        /// </summary>
        [JsonProperty]
        private AlgorithmStatementCollection Statements { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="BlockInterpreter"/>
        /// </summary>
        /// <param name="statements">the list of statements to interpret</param>
        /// <param name="debugMode">defines is the debug mode is enabled or not</param>
        /// <param name="parentProgramInterpreter">the parent program interpreter</param>
        /// <param name="parentMethodInterpreter">the parent method interpreter</param>
        /// <param name="parentBlockInterpreter">the parent block interpreter</param>
        /// <param name="parentClassInterpreter">the parent class interpreter</param>
        internal BlockInterpreter(AlgorithmStatementCollection statements, bool debugMode, ProgramInterpreter parentProgramInterpreter, MethodInterpreter parentMethodInterpreter, BlockInterpreter parentBlockInterpreter, ClassInterpreter parentClassInterpreter)
            : base(debugMode)
        {
            ParentProgramInterpreter = parentProgramInterpreter;
            ParentMethodInterpreter = parentMethodInterpreter;
            ParentBlockInterpreter = parentBlockInterpreter;
            ParentClassInterpreter = parentClassInterpreter;
            Statements = statements;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize, after the constructor, the other properties
        /// </summary>
        internal override void Initialize()
        {
            Variables = new Collection<Variable>();
        }

        /// <summary>
        /// Run all the statements of the current block
        /// </summary>
        /// <returns>Returns True is the statement was a <see cref="AlgorithmReturnStatement"/></returns>
        internal bool Run()
        {
            var stepOver = false;
            var returnStmt = false;
            var i = 0;

            while (i < Statements.Count && !returnStmt)
            {
                if (DebugMode)
                {
                    if (ParentProgramInterpreter.StepIntoOverOutWaiter != null && !ParentProgramInterpreter.StepOutSignal)
                    {
                        ChangeState(this, new AlgorithmInterpreterStateEventArgs(AlgorithmInterpreterState.PauseBreakpoint, GetDebugInfo(false)));
                        Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
                        ParentProgramInterpreter.StepIntoOverOutWaiter.WaitOne();
                    }
                    else if (ParentProgramInterpreter.Waiter != null)
                    {
                        ParentProgramInterpreter.Waiter.WaitOne();
                    }

                    if (ParentProgramInterpreter.StepOverSignal)
                    {
                        stepOver = true;
                        ParentProgramInterpreter.StepOverSignal = false;
                        ParentProgramInterpreter.FreeStepIntoOverOutWaiter();
                    }
                }

                returnStmt = RunStatement(Statements[i]);

                if (DebugMode && stepOver)
                {
                    stepOver = false;
                    if (!ParentProgramInterpreter.CancelStepOverSignal)
                    {
                        ParentProgramInterpreter.ResetStepIntoOverOutWaiter();
                    }
                    ParentProgramInterpreter.CancelStepOverSignal = false;
                }

                if (FailedOrStop || ParentProgramInterpreter.State == AlgorithmInterpreterState.Stopped || ParentProgramInterpreter.State == AlgorithmInterpreterState.StoppedWithError)
                {
                    return false;
                }

                i++;
            }

            if (ParentProgramInterpreter.StepOutSignal && GetParentInterpreter().InterpreterType == InterpreterType.MethodInterpreter)
            {
                ParentProgramInterpreter.StepOutSignal = false;
            }

            return returnStmt;
        }

        /// <summary>
        /// Execute a statement
        /// </summary>
        /// <param name="statement">The statement to interpret</param>
        /// <returns>Returns True is the statement was a <see cref="AlgorithmReturnStatement"/></returns>
        internal bool RunStatement(AlgorithmStatement statement)
        {
            var returnStmt = false;

            switch (statement.DomType)
            {
                case AlgorithmDomType.ConditionStatement:
                    var condition = new Condition(DebugMode, this, statement);
                    condition.Execute();
                    returnStmt = condition.ReturnOccured;
                    break;

                case AlgorithmDomType.IterationStatement:
                    var iteration = new Iteration(DebugMode, this, statement);
                    iteration.Execute();
                    returnStmt = iteration.ReturnOccured;
                    break;

                case AlgorithmDomType.ReturnStatement:
                    new Return(DebugMode, this, statement).Execute();
                    returnStmt = true;
                    break;

                case AlgorithmDomType.AssignStatement:
                    new Assign(DebugMode, this, statement).Execute();
                    break;

                case AlgorithmDomType.VariableDeclaration:
                    new VariableDeclaration(DebugMode, this, statement).Execute();
                    break;

                case AlgorithmDomType.ExpressionStatement:
                    new ExpressionStatement(DebugMode, this, statement).Execute();
                    break;

                case AlgorithmDomType.BreakpointStatement:
                    new Breakpoint(DebugMode, this).Execute();
                    break;

                default:
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this statement : '{statement.GetType().FullName}'"), statement), GetDebugInfo()));
                    break;
            }

            return returnStmt;
        }

        /// <summary>
        /// Execute an expression
        /// </summary>
        /// <param name="expression">The expression to interpret</param>
        /// <returns>Returns the returned value of the expression</returns>
        internal object RunExpression(AlgorithmExpression expression)
        {
            object result = null;

            switch (expression.DomType)
            {
                case AlgorithmDomType.PrimitiveExpression:
                    result = new PrimitiveValue(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.PropertyReferenceExpression:
                    result = new PropertyReference(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.VariableReferenceExpression:
                    result = new VariableReference(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.ClassReferenceExpression:
                    result = new ClassReference(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.ThisReferenceExpression:
                    result = new ThisReference(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.InstanciateExpression:
                    result = new Instanciate(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.InvokeCoreMethodExpression:
                    result = new InvokeCoreMethod(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.InvokeMethodExpression:
                    result = new InvokeMethod(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.BinaryOperatorExpression:
                    result = new BinaryOperator(DebugMode, this, expression).Execute();
                    break;

                case AlgorithmDomType.ArrayIndexerExpression:
                    result = new ArrayIndexerExpression(DebugMode, this, expression).Execute();
                    break;

                default:
                    ChangeState(this, new AlgorithmInterpreterStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this expression : '{expression.GetType().FullName}'"), expression), GetDebugInfo()));
                    break;
            }

            return FailedOrStop ? null : result;
        }

        /// <summary>
        /// Dispose the resources
        /// </summary>
        public override void Dispose()
        {
            Task.Run(() =>
            {
                Statements = null;

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
            });
        }

        #endregion
    }
}