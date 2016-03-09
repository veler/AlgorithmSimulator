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

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 21);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            
            Assert.AreEqual(simulator.StateChangeHistory[20].Error.Exception.Message,"The left expression wait for an array, but the right value is not an array.");
            Assert.AreEqual(simulator.StateChangeHistory[20].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
