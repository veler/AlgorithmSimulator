using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Statements
{
    [TestClass]
    public class Return_Test
    {
        [TestMethod]
        public void Return()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 6);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Primitive value : '123' (type:System.Int32)");

            Assert.AreEqual(simulator.StateChangeHistory[4].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "(Main) Return : '123' (type:System.Int32)");

            Assert.AreEqual(simulator.StateChangeHistory[5].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void ReturnInCondition()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            var stmts = new AlgorithmStatementCollection();
            stmts.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(true)));
            stmts.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(null)));

            entryPoint.Statements.Add(new AlgorithmConditionStatement(new AlgorithmPrimitiveExpression(true), stmts, null));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(false)));

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

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(simulator.StateChangeHistory[6].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void ReturnInIteration()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();
            
            var initialization = new AlgorithmVariableDeclaration("i")
            {
                DefaultValue = new AlgorithmPrimitiveExpression(0)
            };
            var incrementation = new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("i"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.Add, new AlgorithmPrimitiveExpression(1)));
            var condition = new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.LessThan, new AlgorithmPrimitiveExpression(10));
            
            var stmts = new AlgorithmStatementCollection();
            stmts.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(true)));
            stmts.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(null)));

            entryPoint.Statements.Add(new AlgorithmIterationStatement(initialization, incrementation, condition, false, stmts));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(false)));

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
            
            Assert.AreEqual(simulator.StateChangeHistory[8].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(simulator.StateChangeHistory[9].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(simulator.StateChangeHistory[10].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
