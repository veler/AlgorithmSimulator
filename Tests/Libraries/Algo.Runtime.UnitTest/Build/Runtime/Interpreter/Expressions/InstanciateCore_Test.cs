using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class InstanciateCore_Test
    {
        [TestMethod]
        public void InstanciateCore()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression("1.0.0.0"))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 8);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);
            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Primitive value : '1.0.0.0' (type:System.String)");
            Assert.AreEqual(simulator.StateChangeHistory[6].LogMessage, "(Main) Return : '1.0.0.0' (type:System.Version)");

            Assert.AreEqual(simulator.StateChangeHistory[7].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateCoreClassNotFound()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version2"), new AlgorithmPrimitiveExpression("1.0.0.0"))));
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

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version2");

            Assert.AreEqual(simulator.StateChangeHistory[4].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "Unable to find the class 'System.Version2' because it does not exist or it is not accessible.");

            Simulator_Test.RunProgramWithoutDebug(program);
        }
        
        [TestMethod]
        public void InstanciateCoreWithArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression(1), new AlgorithmPrimitiveExpression(0), new AlgorithmPrimitiveExpression(0), new AlgorithmPrimitiveExpression(0))));
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

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[6].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[7].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[8].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[9].LogMessage, "(Main) Return : '1.0.0.0' (type:System.Version)");

            Assert.AreEqual(simulator.StateChangeHistory[10].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateCoreWithBadArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression(new DateTime()))));
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

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Primitive value : '1/1/0001 12:00:00 AM' (type:System.DateTime)");

            Assert.AreEqual(simulator.StateChangeHistory[6].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "Constructor on type 'System.Version' not found.");

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateCoreWithBadArgumentsCount()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"))));
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

            Assert.AreEqual(simulator.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");

            Assert.AreEqual(simulator.StateChangeHistory[5].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "Constructor on type 'System.Version' not found.");

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
