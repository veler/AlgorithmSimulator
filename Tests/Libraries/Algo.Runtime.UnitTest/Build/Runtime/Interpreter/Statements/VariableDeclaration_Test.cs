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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 11);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].LogMessage, "Variable 'publicVar' declared in the program => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Running);
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Variable 'classField' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Variable 'methodVar' declared in the method => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Calling method 'This.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Reference to the current instance : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "Variable 'methodArg' declared in the method's argument => IsArray:False, DefaultValue:123");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 6);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].LogMessage, "Variable 'publicVar' declared in the program => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Variable 'classField' declared in the class => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "The variable 'publicVar' already exists in the program, class, method or block of the algorithm and cannot be declared again.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
