using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class PropertyReference_Test
    {
        [TestMethod]
        public void PropertyReferenceAlgorithmClass()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"))));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmPropertyReferenceExpression(new AlgorithmVariableReferenceExpression("var1"), "Property1"), AlgorithmBinaryOperatorType.Add, new AlgorithmPropertyReferenceExpression(new AlgorithmThisReferenceExpression(), "Property2"))));

            firstClass.Members.Add(entryPoint);
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("Property2") { DefaultValue = new AlgorithmPrimitiveExpression(123) });

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("Property1") { DefaultValue = new AlgorithmPrimitiveExpression(123) });
            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 20);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            
            Assert.AreEqual(simulator.StateChangeHistory[18].LogMessage, "(Main) Return : '246' (type:System.Int32)");

            Assert.AreEqual(simulator.StateChangeHistory[19].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void PropertyReferenceCore()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression("1.0.0.0"))));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPropertyReferenceExpression(new AlgorithmVariableReferenceExpression("var1"), "Major")));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 15);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[13].LogMessage, "(Main) Return : '1' (type:System.Int32)");

            Assert.AreEqual(simulator.StateChangeHistory[14].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
