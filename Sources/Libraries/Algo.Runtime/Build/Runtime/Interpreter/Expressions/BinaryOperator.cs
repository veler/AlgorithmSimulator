using System;
using System.Collections.Generic;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime.Debugger;
using Algo.Runtime.Build.Runtime.Debugger.Exceptions;
using Algo.Runtime.Build.Runtime.Interpreter.Interpreter;

namespace Algo.Runtime.Build.Runtime.Interpreter.Expressions
{
    static internal class BinaryOperatorStatic
    {
        /// <summary>
        /// Just for better performances after, we call it a first time in <see cref="ProgramInterpreter"/>
        /// </summary>
        internal static readonly Dictionary<string, Func<dynamic, dynamic, object>> OperatorsMethodsNames = new Dictionary<string, Func<dynamic, dynamic, object>>()
        {
            {"Add"               , (a, b) => a + b },       // +
            {"Subtract"          , (a, b) => a - b },       // -
            {"Multiply"          , (a, b) => a * b },       // *
            {"Divide"            , (a, b) => a / b },       // /
            {"Modulus"           , (a, b) => a % b },       // %
            {"BitwiseOr"         , (a, b) => a | b },       // |
            {"BitwiseAnd"        , (a, b) => a & b },       // &
            {"Or"                , (a, b) => a || b },      // ||
            {"And"               , (a, b) => a && b },      // &&
            {"LessThan"          , (a, b) => a < b },       // <
            {"LessThanOrEqual"   , (a, b) => a <= b },      // <=
            {"GreaterThan"       , (a, b) => a > b },       // >
            {"GreaterThanOrEqual", (a, b) => a >= b },      // >=
            {"IdentityEquality"  , (a, b) => a == b },      // a == b
            {"IdentityInequality", (a, b) => a != b },      // a != b
            {"ValueEquality"     , (a, b) =>                // a.Equals(b)
                                   {
                                       if (a == null)
                                       {
                                           if (b == null)
                                           {
                                               return true; // null == null
                                           }
                                           return b.Equals(null);
                                       }
                                      return a.Equals(b);
                                   }
            }
        };
    }

    sealed internal class BinaryOperator : InterpretExpression<AlgorithmBinaryOperatorExpression>
    {
        #region Fields

        #endregion

        #region Constructors

        internal BinaryOperator(bool memTrace, BlockInterpreter parentInterpreter, AlgorithmBinaryOperatorExpression expression)
            : base(memTrace, parentInterpreter, expression)
        {
        }

        #endregion

        #region Methods

        internal override object Execute()
        {
            object left;
            object right;
            Func<dynamic, dynamic, dynamic> operatorMethod;
            
            left = ParentInterpreter.RunExpression(Expression.LeftExpression);
            
            if (ParentInterpreter.Failed)
            {
                return null;
            }

            right = ParentInterpreter.RunExpression(Expression.RightExpression);

            if (ParentInterpreter.Failed)
            {
                return null;
            }

            operatorMethod = BinaryOperatorStatic.OperatorsMethodsNames[Expression.Operator.ToString()];

            ParentInterpreter.Log(this, $"Doing an operation '{Expression.Operator}'");

            try
            {
                return operatorMethod(left, right);
            }
            catch (Exception ex)
            {
                ParentInterpreter.ChangeState(this, new SimulatorStateEventArgs(new Error(new OperatorNotFoundException(Expression.Operator.ToString(), ex.Message), ParentInterpreter.GetDebugInfo())));
                return null;
            }
        }

        #endregion
    }
}
