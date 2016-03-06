using System;
using System.Diagnostics;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json.Linq;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class BinaryOperator_Test
    {
        [TestMethod]
        public void BinaryOperatorMultiple()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var2"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(1)));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var2"), new AlgorithmPrimitiveExpression(2)));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("var1"), AlgorithmBinaryOperatorType.Equals, new AlgorithmVariableReferenceExpression("var2")), AlgorithmBinaryOperatorType.LogicalOr, new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("var1"), AlgorithmBinaryOperatorType.LessThan, new AlgorithmVariableReferenceExpression("var2")))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 25);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[24].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void BinaryOperatorDivideByZero()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(123), AlgorithmBinaryOperatorType.Division, new AlgorithmPrimitiveExpression(0))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 10);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[9].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "Attempted to divide by zero.");

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void BinaryOperatorNullValue()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(null), AlgorithmBinaryOperatorType.Equals, new AlgorithmPrimitiveExpression(2))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();
            
            Assert.AreEqual(simulator.StateChangeHistory.Count, 11);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[10].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void BinaryOperatorIncompatibleTypes()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(new JObject()), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(new DateTime()))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 10);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[9].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "Operator 'GreaterThan' cannot be applied to operands of type 'Newtonsoft.Json.Linq.JObject' and 'System.DateTime'");

            Simulator_Test.RunProgramWithoutDebug(program);
        }
        private void PlayOperator(AlgorithmBinaryOperatorType operatorType, ref AlgorithmEntryPointMethod entryPoint)
        {
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("var1"), operatorType, new AlgorithmVariableReferenceExpression("var2"))));
        }

        [TestMethod]
        public void BinaryOperator()
        {
            var time = new Stopwatch();
            time.Start();


            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var2"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(3.14)));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var2"), new AlgorithmPrimitiveExpression(5)));

            PlayOperator(AlgorithmBinaryOperatorType.Addition, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Subtraction, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Division, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Multiply, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Modulus, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.GreaterThan, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.GreaterThanOrEqual, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.LessThan, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.LessThanOrEqual, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Equals, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Equality, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Inequality, ref entryPoint);

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            time.Stop();
            time.Reset();
            time.Start();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            time.Stop();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 87);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[86].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
