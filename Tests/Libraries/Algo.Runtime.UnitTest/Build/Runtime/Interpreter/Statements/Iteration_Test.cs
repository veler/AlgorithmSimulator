using System;
using System.Linq;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Statements
{
    [TestClass]
    public class Iteration_Test
    {
        [TestMethod]
        public void IterationWhileDo()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("y")
            {
                DefaultValue = new AlgorithmPrimitiveExpression(0)
            });

            var initialization = new AlgorithmVariableDeclaration("i")
            {
                DefaultValue = new AlgorithmPrimitiveExpression(0)
            };
            var incrementation = new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("i"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(1)));
            var condition = new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.LessThan, new AlgorithmPrimitiveExpression(10));

            var stmts = new AlgorithmStatementCollection();
            stmts.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("y"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("y"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(10))));

            entryPoint.Statements.Add(new AlgorithmIterationStatement(initialization, incrementation, condition, false, stmts));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("y")));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 109);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[107].LogMessage, "(Main) Return : '10' (type:System.Int32)");

            Assert.AreEqual(simulator.StateChangeHistory[108].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void IterationDoWhile()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("y")
            {
                DefaultValue = new AlgorithmPrimitiveExpression(0)
            });

            var initialization = new AlgorithmVariableDeclaration("i")
            {
                DefaultValue = new AlgorithmPrimitiveExpression(0)
            };
            var incrementation = new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("i"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(1)));
            var condition = new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.LessThan, new AlgorithmPrimitiveExpression(10));

            var stmts = new AlgorithmStatementCollection();
            stmts.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("y"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("y"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(10))));

            entryPoint.Statements.Add(new AlgorithmIterationStatement(initialization, incrementation, condition, true, stmts));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("y")));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 106);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[104].LogMessage, "(Main) Return : '10' (type:System.Int32)");

            Assert.AreEqual(simulator.StateChangeHistory[105].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
        
        [TestMethod]
        public void IterationInfiniteLoop()
        {
            // while (true) {
            //      object myVar;
            // }

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmIterationStatement(null, null, new AlgorithmPrimitiveExpression(true), false, new AlgorithmStatementCollection() { new AlgorithmVariableDeclaration("myVar") }));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(1)));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);
            
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();

            simulator.Stop();

            Task.Delay(TimeSpan.FromSeconds(3)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Last().State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void IterationExceptionInside()
        {
            // while (true) {
            //      object myVar;
            //      object myVar;
            // }

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmIterationStatement(null, null, new AlgorithmPrimitiveExpression(true), false, new AlgorithmStatementCollection() { new AlgorithmVariableDeclaration("myVar"), new AlgorithmVariableDeclaration("myVar") }));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(1)));

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

            Assert.AreEqual(simulator.StateChangeHistory[5].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
