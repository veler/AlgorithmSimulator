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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 6);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Primitive value : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "(Main) Return : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 7);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
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
            var incrementation = new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("i"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(1)));
            var condition = new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("i"), AlgorithmBinaryOperatorType.LessThan, new AlgorithmPrimitiveExpression(10));
            
            var stmts = new AlgorithmStatementCollection();
            stmts.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(true)));
            stmts.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(null)));

            entryPoint.Statements.Add(new AlgorithmIterationStatement(initialization, incrementation, condition, false, stmts));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(false)));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 11);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Primitive value : 'True' (type:System.Boolean)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }
    }
}
