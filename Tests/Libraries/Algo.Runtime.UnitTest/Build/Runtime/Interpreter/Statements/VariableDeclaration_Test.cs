using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Statements
{
    [TestClass]
    public class VariableDeclaration_Test
    {
        [TestMethod]
        public void VariableDeclaration()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var firstmethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            var entryPoint = new AlgorithmEntryPointMethod();

            firstmethod.Arguments.Add(new AlgorithmParameterDeclaration("methodArg"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("methodVar"));
            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeMethodExpression("FirstMethod", new AlgorithmPrimitiveExpression(123))));
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("classField"));
            program.Variables.Add(new AlgorithmVariableDeclaration("publicVar"));

            firstClass.Members.Add(firstmethod);
            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 11);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);

            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[2].LogMessage, "Variable 'publicVar' declared in the program => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Running);
            
            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Variable 'classField' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(simulator.StateChangeHistory[5].LogMessage, "Variable 'methodVar' declared in the method => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(simulator.StateChangeHistory[6].LogMessage, "Calling method 'This.FirstMethod'");
            Assert.AreEqual(simulator.StateChangeHistory[7].LogMessage, "Reference to the current instance : FirstClass");
            Assert.AreEqual(simulator.StateChangeHistory[8].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(simulator.StateChangeHistory[9].LogMessage, "Variable 'methodArg' declared in the method's argument => IsArray:False, DefaultValue:123");

            Assert.AreEqual(simulator.StateChangeHistory[10].State, SimulatorState.Stopped);
            Assert.AreEqual(simulator.State, SimulatorState.Stopped);

            Simulator_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void VariableWithTheSameName()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var firstmethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            var entryPoint = new AlgorithmEntryPointMethod();

            firstmethod.Arguments.Add(new AlgorithmParameterDeclaration("methodArg"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("publicVar"));
            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeMethodExpression("FirstMethod", new AlgorithmPrimitiveExpression(123))));
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("classField"));
            program.Variables.Add(new AlgorithmVariableDeclaration("publicVar"));

            firstClass.Members.Add(firstmethod);
            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var simulator = new Simulator(program);

            var task = simulator.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(simulator.StateChangeHistory.Count, 6);
            Assert.AreEqual(simulator.StateChangeHistory[0].State, SimulatorState.Ready);
            Assert.AreEqual(simulator.StateChangeHistory[1].State, SimulatorState.Preparing);

            Assert.AreEqual(simulator.StateChangeHistory[2].State, SimulatorState.Log);
            Assert.AreEqual(simulator.StateChangeHistory[2].LogMessage, "Variable 'publicVar' declared in the program => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(simulator.StateChangeHistory[3].State, SimulatorState.Running);

            Assert.AreEqual(simulator.StateChangeHistory[4].LogMessage, "Variable 'classField' declared in the class => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(simulator.StateChangeHistory[5].State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.State, SimulatorState.StoppedWithError);
            Assert.AreEqual(simulator.Error.Exception.Message, "The variable 'publicVar' already exists in the program, class, method or block of the algorithm and cannot be declared again.");

            Simulator_Test.RunProgramWithoutDebug(program);
        }
    }
}
