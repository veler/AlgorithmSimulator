using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Interpreter.Statements;
using Algo.Runtime.Build.Runtime.Memory;
using Algo.Runtime.ComponentModel.Types;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Interpreter.Expressions;
using Newtonsoft.Json;

namespace Algo.Runtime.Build.Runtime.Interpreter.Interpreter
{
    sealed internal class BlockInterpreter : Interpret
    {
        #region Properties

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
            var program = GetFirstNextParentInterpreter<ProgramInterpreter>();
            var returnStmt = false;
            var i = 0;

            while (i < Statements.Count && !returnStmt)
            {
                returnStmt = RunStatement(Statements[i]);

                if (Failed || (program != null && (program.State == SimulatorState.Stopped || program.State == SimulatorState.StoppedWithError)))
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

            TypeSwitch.Switch(
                statement,
                TypeSwitch.Case<AlgorithmConditionStatement>(stmt =>
                {
                    var condition = new Condition(MemTrace, this, stmt);
                    condition.Execute();
                    returnStmt = condition.ReturnOccured;
                }),
                TypeSwitch.Case<AlgorithmIterationStatement>(stmt =>
                {
                    var iteration = new Iteration(MemTrace, this, stmt);
                    iteration.Execute();
                    returnStmt = iteration.ReturnOccured;
                }),
                TypeSwitch.Case<AlgorithmReturnStatement>(stmt =>
                {
                    new Return(MemTrace, this, stmt).Execute();
                    returnStmt = true;
                }),
                TypeSwitch.Case<AlgorithmAssignStatement>(stmt =>
                {
                    new Assign(MemTrace, this, stmt).Execute();
                }),
                TypeSwitch.Case<AlgorithmVariableDeclaration>(stmt =>
                {
                    new VariableDeclaration(MemTrace, this, stmt).Execute();
                }),
                TypeSwitch.Case<AlgorithmExpressionStatement>(stmt =>
                {
                    new ExpressionStatement(MemTrace, this, stmt).Execute();
                }),
                TypeSwitch.Default(() =>
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this statement : '{statement.GetType().FullName}'"), GetDebugInfo())));
                }));

            return returnStmt;
        }

        internal object RunExpression(AlgorithmExpression expression)
        {
            object result = null;

            TypeSwitch.Switch(
                expression,
                TypeSwitch.Case<AlgorithmPrimitiveExpression>(expr =>
                {
                    result = new PrimitiveValue(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmPropertyReferenceExpression>(expr =>
                {
                    result = new PropertyReference(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmVariableReferenceExpression>(expr =>
                {
                    result = new VariableReference(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmClassReferenceExpression>(expr =>
                {
                    result = new ClassReference(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmThisReferenceExpression>(expr =>
                {
                    result = new ThisReference(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmInstanciateExpression>(expr =>
                {
                    result = new Instanciate(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmInvokeCoreMethodExpression>(expr =>
                {
                    result = new InvokeCoreMethod(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmInvokeMethodExpression>(expr =>
                {
                    result = new InvokeMethod(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Case<AlgorithmBinaryOperatorExpression>(expr =>
                {
                    result = new BinaryOperator(MemTrace, this, expr).Execute();
                }),
                TypeSwitch.Default(() =>
                {
                    ChangeState(this, new SimulatorStateEventArgs(new Error(new InvalidCastException($"Unable to find an interpreter for this expression : '{expression.GetType().FullName}'"), GetDebugInfo())));
                }));


            return Failed ? null : result;
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