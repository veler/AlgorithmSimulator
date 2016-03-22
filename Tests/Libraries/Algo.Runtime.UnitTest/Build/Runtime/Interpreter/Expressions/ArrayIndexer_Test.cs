using System;
using System.Collections.Generic;
using System.Diagnostics;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json.Linq;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class ArrayIndexer_Test
    {
        [TestMethod]
        public void ArrayIndexer()
        {
            /*

            FUNCTION Main()
                VARIABLE result
                VARIABLE var1[] = { "item1", "item2 }
                result = var1[1]
                var1[0] = result
                return var1[0]
            END FUNCTION

            */

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1", true));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(new List<object>() { "item1", "item2" })));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmArrayIndexerExpression(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(1))));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmArrayIndexerExpression(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(0)), new AlgorithmVariableReferenceExpression("result")));

            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmArrayIndexerExpression(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(0))));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);
            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 26);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[24].LogMessage, "(Main) Return : 'item2' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[25].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void ArrayIndexerWithExpression()
        {
            /*

            FUNCTION Main()
                VARIABLE result = 1
                VARIABLE var1[] = { "item1", "item2 }
                result = var1[result]
                return result
            END FUNCTION

            */

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1", true));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmPrimitiveExpression(1)));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(new List<object>() { "item1", "item2" })));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmArrayIndexerExpression(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmVariableReferenceExpression("result"))));

            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("result")));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);
            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 22);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[20].LogMessage, "(Main) Return : 'item2' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[21].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void ArrayIndexerOutOfIndex()
        {
            /*

            FUNCTION Main()
                VARIABLE var1[] = { "item1", "item2 }
                return var1[2]
            END FUNCTION

            */

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1", true));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(new List<object>() { "item1", "item2" })));

            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmArrayIndexerExpression(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(2))));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);
            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 12);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].Error.Exception.Message, "Unable to get the item number '2' because the limit of the array is '1'.");

            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void ArrayMethodAndProperties()
        {
            /*

            FUNCTION Main()
                VARIABLE var1[]
                var1.Add("item1")
                return var1.Count
            END FUNCTION

            */

            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1", true) { DefaultValue = new AlgorithmPrimitiveExpression(new List<object>()) });

            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmVariableReferenceExpression("var1"), "Add", new[] { typeof(object) }, new AlgorithmPrimitiveExpression("item1"))));

            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPropertyReferenceExpression(new AlgorithmVariableReferenceExpression("var1"), "Count")));

            firstClass.Members.Add(entryPoint);
            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);
            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 13);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "(Main) Return : '1' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }
    }
}
