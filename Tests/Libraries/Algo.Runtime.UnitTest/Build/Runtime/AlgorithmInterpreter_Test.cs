using System;
using System.Linq;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime
{
    [TestClass]
    public class AlgorithmInterpreter_Test
    {
        public static void RunProgramWithoutDebug(AlgorithmProgram program, bool mustCrash = false)
        {
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: false);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 4);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            if (mustCrash)
            {
                Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.StoppedWithError);
                Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            }
            else
            {
                Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Stopped);
                Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);
            }

            algorithmInterpreter.Dispose();
        }

        public static void RunProgramStopWithoutDebug(AlgorithmProgram program, bool mustCrash = false)
        {
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: false);

            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();

            algorithmInterpreter.Stop();

            Task.Delay(TimeSpan.FromSeconds(3)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 4);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            algorithmInterpreter.Dispose();
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

        private AlgorithmProgram CreateBasicRecursivityProgramWithOneBreakpoint()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num == 4)
            //          breakpoint // do a breakpoint
            //      if (num > 1)
            //          num = FirstMethod(num - 1)
            //      return num;
            // }
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Equality, new AlgorithmPrimitiveExpression(4)), new AlgorithmStatementCollection() { new AlgorithmBreakpointStatement() }, new AlgorithmStatementCollection() { }));
            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("num"), new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(10))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            return program;
        }

        private AlgorithmProgram CreateBasicProgramWithOneBreakpoint()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            /*
                FUNCTION FirstMethod(num)
                    num = num + 10
                    RETURN num
                END FUNCTION

                FUNCTION Main()
                    BREAKPOINT // do a breakpoint
                    VARIABLE result
                    result = FirstMethod(10)
                    RETURN result
                END FUNCTION
            */
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("num"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(10))));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmBreakpointStatement());
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmInvokeMethodExpression("FirstMethod", new AlgorithmPrimitiveExpression(10))));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("result")));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            return program;
        }

        private AlgorithmProgram CreateBasicProgramWithTwoBreakpoints()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            /*
                FUNCTION FirstMethod(num)
                    num = num + 10
                    BREAKPOINT // do a breakpoint
                    RETURN num
                END FUNCTION

                FUNCTION Main()
                    BREAKPOINT // do a breakpoint
                    VARIABLE result
                    result = FirstMethod(10)
                    RETURN result
                END FUNCTION
            */
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("num"), new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Addition, new AlgorithmPrimitiveExpression(10))));
            firstMethod.Statements.Add(new AlgorithmBreakpointStatement());
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmBreakpointStatement());
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("result"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("result"), new AlgorithmInvokeMethodExpression("FirstMethod", new AlgorithmPrimitiveExpression(10))));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("result")));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            return program;
        }

        [TestMethod]
        public void AlgorithmInterpreterStates()
        {
            var program = CreateBasicProgram();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 9);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.Log);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterError()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("MyVar"));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("MyVar2")));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 5);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.Count(), 1);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables.Count, 1);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "MyVar");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void AlgorithmInterpreterPauseResume()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(90000000))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();

            algorithmInterpreter.Pause();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Last().State, AlgorithmInterpreterState.Pause);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            algorithmInterpreter.Stop();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Last().State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);
        }

        [TestMethod]
        public void AlgorithmInterpreterBreakpointResume()
        {
            var program = CreateBasicRecursivityProgramWithOneBreakpoint();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[95].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 8);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)4);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            algorithmInterpreter.Stop();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);

            Assert.AreEqual(algorithmInterpreter.DebugInfo, null);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[171].LogMessage, "(Main) Return : '1' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[172].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterBreakpointStop()
        {
            var program = CreateBasicRecursivityProgramWithOneBreakpoint();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[95].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 8);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)4);

            algorithmInterpreter.Stop();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[96].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterStepInto()
        {
            var program = CreateBasicRecursivityProgramWithOneBreakpoint();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[95].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 8);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)4);

            algorithmInterpreter.StepInto();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)4);

            algorithmInterpreter.StepInto();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)3);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[176].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterStepIntoGoOutOfMethod()
        {
            var program = CreateBasicProgramWithTwoBreakpoints();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables.Count, 0);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[20].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)20);

            algorithmInterpreter.StepInto();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[25].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "result");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)20);

            algorithmInterpreter.StepInto();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[28].LogMessage, "(Main) Return : '20' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[29].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterStepOver()
        {
            var program = CreateBasicProgramWithOneBreakpoint();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables.Count, 0);

            algorithmInterpreter.StepInto();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "result");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, null);

            algorithmInterpreter.StepOver();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[24].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "result");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)20);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[27].LogMessage, "(Main) Return : '20' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[28].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterStepOverBreakpoint()
        {
            var program = CreateBasicProgramWithTwoBreakpoints();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables.Count, 0);

            algorithmInterpreter.StepOver();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "result");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, null);

            algorithmInterpreter.StepOver();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[22].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)20);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[28].LogMessage, "(Main) Return : '20' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[29].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void AlgorithmInterpreterStepOut()
        {
            var program = CreateBasicRecursivityProgramWithOneBreakpoint();
            var algorithmInterpreter = new AlgorithmInterpreter(program);

            algorithmInterpreter.StartAsync(debugMode: true);

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[95].State, AlgorithmInterpreterState.PauseBreakpoint);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 8);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)4);

            algorithmInterpreter.StepInto();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)4);

            algorithmInterpreter.StepOut();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)1);

            algorithmInterpreter.Resume();

            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[176].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }
    }
}
