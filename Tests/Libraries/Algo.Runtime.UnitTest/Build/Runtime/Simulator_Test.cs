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
            Assert.AreEqual(simulator.Error.DebugInfo.CallStackService.CallStacks.Count(), 1);
            Assert.AreEqual(simulator.Error.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables.Count, 1);
            Assert.AreEqual(simulator.Error.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "MyVar");

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
