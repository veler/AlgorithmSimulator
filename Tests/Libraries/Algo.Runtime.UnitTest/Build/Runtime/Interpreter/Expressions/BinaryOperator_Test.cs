using System;
using System.Diagnostics;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json.Linq;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class BinaryOperator_Test
    {
        [TestMethod]
        public void BinaryOperatorMultiple()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var2"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(1)));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var2"), new AlgorithmPrimitiveExpression(2)));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("var1"), AlgorithmBinaryOperatorType.Equals, new AlgorithmVariableReferenceExpression("var2")), AlgorithmBinaryOperatorType.LogicalOr, new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("var1"), AlgorithmBinaryOperatorType.LessThan, new AlgorithmVariableReferenceExpression("var2")))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 25);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[24].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void BinaryOperatorDivideByZero()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(123), AlgorithmBinaryOperatorType.Division, new AlgorithmPrimitiveExpression(0))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 10);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.InnerException.Message, "Attempted to divide by zero.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void BinaryOperatorNullValue()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(null), AlgorithmBinaryOperatorType.Equals, new AlgorithmPrimitiveExpression(2))));

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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void BinaryOperatorIncompatibleTypes()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmPrimitiveExpression(new JObject()), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(new DateTime()))));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 10);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Operator 'GreaterThan' cannot be applied to operands of type 'Newtonsoft.Json.Linq.JObject' and 'System.DateTime'");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
        private void PlayOperator(AlgorithmBinaryOperatorType operatorType, ref AlgorithmEntryPointMethod entryPoint)
        {
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("var1"), operatorType, new AlgorithmVariableReferenceExpression("var2"))));
        }

        [TestMethod]
        public void BinaryOperator()
        {
            var time = new Stopwatch();
            time.Start();


            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var1"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("var2"));
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));

            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var1"), new AlgorithmPrimitiveExpression(3.14)));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("var2"), new AlgorithmPrimitiveExpression(5)));

            PlayOperator(AlgorithmBinaryOperatorType.Addition, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Subtraction, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Division, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Multiply, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Modulus, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.GreaterThan, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.GreaterThanOrEqual, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.LessThan, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.LessThanOrEqual, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Equals, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Equality, ref entryPoint);
            PlayOperator(AlgorithmBinaryOperatorType.Inequality, ref entryPoint);

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            time.Stop();
            time.Reset();
            time.Start();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            time.Stop();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 87);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[86].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }
    }
}
