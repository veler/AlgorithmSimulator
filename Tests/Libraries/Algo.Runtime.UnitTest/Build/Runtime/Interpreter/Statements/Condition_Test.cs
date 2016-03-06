using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Statements
{
    [TestClass]
    public class Condition_Test
    {
        [TestMethod]
        public void ConditionBool()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();
            
            entryPoint.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(1), AlgorithmBinaryOperatorType.LessThan, new AlgorithmPrimitiveExpression(2)), new AlgorithmStatementCollection() {new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(true))}, new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(false)) }));
            
            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 9);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            
            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Primitive value : '2' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Doing an operation 'LessThan'");
            Assert.AreEqual(simulator.StateChangeHistory[6].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(simulator.StateChangeHistory[7].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(simulator.StateChangeHistory[8].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void ConditionInt()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(1), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(0)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(true)) }, new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(false)) }));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 9);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Doing an operation 'Addition'");
            Assert.AreEqual(simulator.StateChangeHistory[6].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(simulator.StateChangeHistory[7].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(simulator.StateChangeHistory[8].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
        
        [TestMethod]
        public void ConditionNotBool()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(1), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(2)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(true)) }, new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(false)) }));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 7);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Primitive value : '2' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Doing an operation 'Addition'");

            Assert.AreEqual(simulator.StateChangeHistory[6].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "Unable to cast this number to a boolean.");

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
