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
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmPropertyReferenceExpression(new AlgorithmVariableReferenceExpression("var1"), "Property1"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPropertyReferenceExpression(new AlgorithmThisReferenceExpression(), "Property2"))));

            firstClass.Members.Add(entryPoint);
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("Property2") { DefaultValue = new AlgorithmPrimitiveExpression(123) });

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("Property1") { DefaultValue = new AlgorithmPrimitiveExpression(123) });
            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 20);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[18].LogMessage, "(Main) Return : '246' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[19].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 15);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[13].LogMessage, "(Main) Return : '1' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[14].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }
    }
}
