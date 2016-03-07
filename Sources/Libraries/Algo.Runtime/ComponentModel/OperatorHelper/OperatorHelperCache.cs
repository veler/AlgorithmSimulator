using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Algo.Runtime.Build.AlgorithmDOM.DOM;

namespace Algo.Runtime.ComponentModel.OperatorHelper
{
    internal static class OperatorHelperCache
    {
        #region Structures

        internal class Method
        {
            internal MethodInfo MethodInfo { get; set; }
            internal Type[] ParametersType { get; set; }
        }

        #endregion

        #region Fields

        private static List<Method> _methods;

        #endregion

        #region Methods

        internal static void Initialize()
        {
            _methods = new List<Method>();
            foreach (var methodInfo in typeof(OperatorHelper).GetRuntimeMethods().ToList())
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 2)
                {
                    continue;
                }

                var method = new Method
                {
                    MethodInfo = methodInfo,
                    ParametersType = new[] {parameters[0].ParameterType, parameters[1].ParameterType}
                };

                _methods.Add(method);
            }
        }

        internal static void ClearCache()
        {
            _methods.Clear();
            _methods = null;
        }

        internal static MethodInfo GetOperator(AlgorithmBinaryOperatorType operatorType, Type leftType, Type rightType)
        {
            var operatorTypeString = operatorType.ToString();
            var method = _methods.FirstOrDefault(m => m.MethodInfo.Name == operatorTypeString && m.ParametersType[0] == leftType && m.ParametersType[1] == rightType);
            if (method == null)
            {
                return null;
            }
            return method.MethodInfo;
        }

        #endregion
    }

}
