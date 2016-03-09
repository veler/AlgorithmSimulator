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
    internal sealed class BlockInterpreter : Interpret
    {
        #region Properties

        [JsonIgnore]
        internal override InterpreterType InterpreterType => InterpreterType.BlockInterpreter;

        [JsonProperty]
        private AlgorithmStatementCollection Statements { get; set; }

        #endregion

        #region Constructors

        internal BlockInterpreter(AlgorithmStatementCollection statements, bool memTrace)
            : base(memTrace)
        {
            Statements = statements;
        }

        #endregion

        #region Methods

        internal override void Initialize()
        {
            Variables = new Collection<Variable>();
        }

        internal bool Run()
        {
            var program = (ProgramInterpreter)GetFirstNextParentInterpreter(InterpreterType.ProgramInterpreter);
            var returnStmt = false;
            var i = 0;

            while (i < Statements.Count && !returnStmt)
            {
                returnStmt = RunStatement(Statements[i]);

                if (FailedOrStop || (program != null && (program.State == SimulatorState.Stopped || program.State == SimulatorState.StoppedWithError)))
                {
                    return false;
                }

                i++;
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
                    var condition = new Condition(MemTrace, this, statement);
                    condition.Execute();
                    returnStmt = condition.ReturnOccured;
                    break;

                case AlgorithmDomType.IterationStatement:
                    var iteration = new Iteration(MemTrace, this, statement);
                    iteration.Execute();
                    returnStmt = iteration.ReturnOccured;
                    break;

                case AlgorithmDomType.ReturnStatement:
                    new Return(MemTrace, this, statement).Execute();
                    returnStmt = true;
                    break;

                case AlgorithmDomType.AssignStatement:
                    new Assign(MemTrace, this, statement).Execute();
                    break;

                case AlgorithmDomType.VariableDeclaration:
                    new VariableDeclaration(MemTrace, this, statement).Execute();
                    break;

                case AlgorithmDomType.ExpressionStatement:
                    new ExpressionStatement(MemTrace, this, statement).Execute();
                    break;

                default:
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this statement : '{statement.GetType().FullName}'"), GetDebugInfo())));
                    break;
            }

            return returnStmt;
        }

        internal object RunExpression(AlgorithmExpression expression)
        {
            object result = null;

            switch (expression.DomType)
            {
                case AlgorithmDomType.PrimitiveExpression:
                    result = new PrimitiveValue(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.PropertyReferenceExpression:
                    result = new PropertyReference(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.VariableReferenceExpression:
                    result = new VariableReference(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.ClassReferenceExpression:
                    result = new ClassReference(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.ThisReferenceExpression:
                    result = new ThisReference(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.InstanciateExpression:
                    result = new Instanciate(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.InvokeCoreMethodExpression:
                    result = new InvokeCoreMethod(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.InvokeMethodExpression:
                    result = new InvokeMethod(MemTrace, this, expression).Execute();
                    break;

                case AlgorithmDomType.BinaryOperatorExpression:
                    result = new BinaryOperator(MemTrace, this, expression).Execute();
                    break;

                default:
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this expression : '{expression.GetType().FullName}'"), GetDebugInfo())));
                    break;
            }

            return FailedOrStop ? null : result;
        }

        public override void Dispose()
        {
            Task.Run(() =>
            {
                Statements.Clear();
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