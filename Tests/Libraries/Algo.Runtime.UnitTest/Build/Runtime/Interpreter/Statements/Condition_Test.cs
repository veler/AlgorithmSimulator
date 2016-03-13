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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 9);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Primitive value : '2' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Doing an operation 'LessThan'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 9);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Doing an operation 'Addition'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 7);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Primitive value : '2' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Doing an operation 'Addition'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Unable to cast this number to a boolean.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
