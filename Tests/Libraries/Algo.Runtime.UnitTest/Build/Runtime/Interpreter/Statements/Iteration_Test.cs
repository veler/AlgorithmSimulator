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

            /*
            FUNCTION Main()
                y = 0
                i = 0
                DO WHILE (i < 10)
                    y = y + 10
                    i = i + 1
                LOOP
                RETURN y
            END FUNCTION           
            */

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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 163);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[161].LogMessage, "(Main) Return : '100' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[162].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void IterationDoWhile()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            /*
            FUNCTION Main()
                y = 0
                i = 0
                DO
                    y = y + 10
                    i = i + 1
                LOOP WHILE (i < 10)
                RETURN y
            END FUNCTION           
            */

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

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 160);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[158].LogMessage, "(Main) Return : '100' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[159].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }
        
        [TestMethod]
        public void IterationInfiniteLoop()
        {
            // DO WHILE (true)
            //      object myVar
            // LOOP

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmIterationStatement(null, null, new AlgorithmPrimitiveExpression(true), false, new AlgorithmStatementCollection() { new AlgorithmVariableDeclaration("myVar") }));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(1)));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);
            
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();

            algorithmInterpreter.Stop();

            Task.Delay(TimeSpan.FromSeconds(3)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Last().State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);
        }

        [TestMethod]
        public void IterationExceptionInside()
        {
            // DO WHILE (true)
            //      object myVar;
            //      object myVar;
            // LOOP

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmIterationStatement(null, null, new AlgorithmPrimitiveExpression(true), false, new AlgorithmStatementCollection() { new AlgorithmVariableDeclaration("myVar"), new AlgorithmVariableDeclaration("myVar") }));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(1)));

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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
