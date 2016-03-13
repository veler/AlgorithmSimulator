using System.Collections.Generic;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Statements
{
    [TestClass]
    public class Assign_Test
    {
        [TestMethod]
        public void Assign()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("myVar"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("myVarArray", true));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("myVar"), new AlgorithmPrimitiveExpression(1)));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("myVarArray"), new AlgorithmPrimitiveExpression(new[] { "yo" })));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("myVarArray"), new AlgorithmPrimitiveExpression(new List<string> { "yo" })));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("myVarArray"), new AlgorithmPrimitiveExpression(true)));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 21);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[20].Error.Exception.Message,"The left expression wait for an array, but the right value is not an array.");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[20].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
