namespace PortableSimulator.Build.Simulator.Interpreter.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using PortableSimulator.Build.AlgorithmDOM.DOM;
    using PortableSimulator.ComponentModel;

    class InvokeCoreMethod : InterpretExpression<AlgorithmInvokeCoreMethodExpression>
    {
        #region Constructors

        public InvokeCoreMethod(AlgorithmInvokeCoreMethodExpression expression)
            : base(expression)
        {
        }

        #endregion

        #region Methods

        public override object Execute()
        {
            var type = Type.GetType(this.Expression.Class.ToString());
            if (type != null)
            {
                var method = type.GetRuntimeMethod(this.Expression.MethodName, this.Expression.ArgumentsTypes);
                var arguments = new List<object>();

                foreach (var argument in this.Expression.Arguments)
                {
                    TypeSwitch.Do(argument,
                    TypeSwitch.Case<AlgorithmVariableReferenceExpression>(arg => arguments.Add(new VariableReference(arg).Execute())), // AlgoVariableReferenceExpression
                    TypeSwitch.Default(() => arguments.Add(argument))); // Default
                }

                return method.Invoke(null, arguments.ToArray());
            }
            return null;
        }

        #endregion
    }
}
