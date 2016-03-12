using System;
using System.Linq;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime
{
    [TestClass]
    public class Simulator_Test
    {
        public static void RunProgramWithoutDebug(AlgorithmProgram program, bool mustCrash = false)
        {
            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: false);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 4);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);
        }

        public static void RunProgramStopWithoutDebug(AlgorithmProgram program, bool mustCrash = false)
        {
            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: false);

            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();

            simulator.Stop();

            Task.Delay(TimeSpan.FromSeconds(3)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 4);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);
        }

        private AlgorithmProgram CreateBasicProgram()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("MyVar"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("MyVar"), new AlgorithmPrimitiveExpression(12)));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            return program;
        }

        [TestMethod]
        public void SimulatorStates()
        {
            var program = CreateBasicProgram();
            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 9);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[4].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[5].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[6].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[7].State, SimulatorState.Log);

            Assert.AreEqual(simulator.StateChangeHistory[8].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void SimulatorError()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("MyVar"));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("MyVar2")));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 5);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[4].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.Count(), 1);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables.Count, 1);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "MyVar");

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void SimulatorPauseResume()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(90000000))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();

            simulator.Pause();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Last().State, SimulatorState.Pause);

            simulator.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            simulator.Stop();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory.Last().State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void SimulatorBreakpointResume()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num == 2)
            //          breakpoint // do a breakpoint
            //      if (num > 1)
            //          return FirstMethod(num - 1)
            //      return num;
            // }
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Equality, new AlgorithmPrimitiveExpression(2)), new AlgorithmStatementCollection() { new AlgorithmBreakpointStatement() }, new AlgorithmStatementCollection() { }));
            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(10))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            simulator.StartAsync(debugMode: true);
            
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory[107].State, SimulatorState.PauseBreakpoint);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 10);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)2);

            simulator.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            simulator.Stop();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Log);

            Assert.AreEqual(simulator.DebugInfo, null);
            Assert.AreEqual(simulator.StateChangeHistory[135].LogMessage, "(Main) Return : '1' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[136].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void SimulatorBreakpointStop()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num == 2)
            //          breakpoint // do a breakpoint
            //      if (num > 1)
            //          return FirstMethod(num - 1)
            //      return num;
            // }
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Equality, new AlgorithmPrimitiveExpression(2)), new AlgorithmStatementCollection() { new AlgorithmBreakpointStatement() }, new AlgorithmStatementCollection() { }));
            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(10))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            simulator.StartAsync(debugMode: true);
            
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory[107].State, SimulatorState.PauseBreakpoint);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 10);
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(simulator.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)2);

            simulator.Stop();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);
            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[108].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
