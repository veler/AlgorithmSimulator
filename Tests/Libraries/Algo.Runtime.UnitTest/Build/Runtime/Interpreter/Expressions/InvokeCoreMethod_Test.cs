using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class InvokeCoreMethod_Test
    {
        [TestMethod]
        public void InvokeCoreMethod()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression("System", "String"), "IsNullOrWhiteSpace", new[] { typeof(string) }, new AlgorithmPrimitiveExpression(" "))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 8);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.String.IsNullOrWhiteSpace'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.String");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : ' ' (type:System.String)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "(Main) Return : 'True' (type:System.Boolean)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeCoreMethodReferenceClassNotFound()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("MyVar"));

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression("System", "String2"), "IsNullOrWhiteSpace", new[] { typeof(string) }, new AlgorithmPrimitiveExpression(" "))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Calling core method 'System.String2.IsNullOrWhiteSpace'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Reference to the class : System.String2");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Unable to find the class 'System.String2' because it does not exist or it is not accessible.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeCoreMethodVariableReference()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("MyVar"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("MyVar"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression("1.0.0.0"))));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmVariableReferenceExpression("MyVar"), "ToString", null)));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 14);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Variable 'MyVar' declared in the method => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Assign 'MyVar' to 'new System.Version()'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Value of the variable 'MyVar' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Primitive value : '1.0.0.0' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "'MyVar' is now equal to '1.0.0.0' (type:System.Version)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Calling core method 'MyVar.ToString'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'MyVar' is '1.0.0.0' (type:System.Version)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].LogMessage, "(Main) Return : '1.0.0.0' (type:System.String)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[13].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeCoreMethodThatDoesNotExist()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression("System", "String"), "IsNullOrWhiteSpace2", new[] { typeof(string) }, new AlgorithmPrimitiveExpression(" "))));
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
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.String.IsNullOrWhiteSpace2'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.String");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "The method 'IsNullOrWhiteSpace2' does not exists in the current class or is not accessible.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeCoreMethodWithBadArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false, new AlgorithmParameterDeclaration("parm1"), new AlgorithmParameterDeclaration("param2", true));

            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("param2")));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression("System", "String"), "IsNullOrWhiteSpace", new[] { typeof(string) }, new AlgorithmPrimitiveExpression(new Action(() => { })))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.String.IsNullOrWhiteSpace'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.String");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : 'System.Action' (type:System.Action)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeCoreMethodWithBadArgumentsCount()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false, new AlgorithmParameterDeclaration("param1"));

            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("param1")));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression("System", "String"), "IsNullOrWhiteSpace", new[] { typeof(string) }, new AlgorithmPrimitiveExpression(" "), new AlgorithmPrimitiveExpression("HelloWorld"))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 8);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.String.IsNullOrWhiteSpace'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.String");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : ' ' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : 'HelloWorld' (type:System.String)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "There is a method 'IsNullOrWhiteSpace' in the class 'System.String', but it does not have 2 argument(s).");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        private AlgorithmProgram GetAsyncProgram(bool awaitCall)
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();

            var invoke = new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression("System.Threading.Tasks", "Task"), "Delay", new[] { typeof(int) }, new AlgorithmPrimitiveExpression(50));
            invoke.Await = awaitCall;

            entryPoint.Statements.Add(new AlgorithmReturnStatement(invoke));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            return program;
        }


        [TestMethod]
        public void InvokeCoreMethodAsync()
        {
            var program = GetAsyncProgram(false);

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 8);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.Threading.Tasks.Task.Delay'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.Threading.Tasks.Task");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : '50' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "(Main) Return : 'System.Threading.Tasks.Task+DelayPromise' (type:System.Threading.Tasks.Task+DelayPromise)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeCoreMethodAsyncAwait()
        {
            var program = GetAsyncProgram(true);

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 8);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.Threading.Tasks.Task.Delay'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.Threading.Tasks.Task");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : '50' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "(Main) Return : {null}");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        public static Task<string> TaskTest()
        {
            return Task.Run(() => "Hello");
        }

        [TestMethod]
        public void InvokeCoreMethodAsyncAwaitResult()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();

            var invoke = new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression(typeof(InvokeCoreMethod_Test)), "TaskTest", new Type[0]);
            invoke.Await = true;

            entryPoint.Statements.Add(new AlgorithmReturnStatement(invoke));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions.InvokeCoreMethod_Test.TaskTest'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions.InvokeCoreMethod_Test");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "(Main) Return : 'Hello' (type:System.String)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeCoreMethodAwaitButNotAsync()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();

            var invoke = new AlgorithmInvokeCoreMethodExpression(new AlgorithmClassReferenceExpression(typeof(Debug)), "WriteLine", new[] { typeof(string) }, new AlgorithmPrimitiveExpression("Hello"));
            invoke.Await = true;

            entryPoint.Statements.Add(new AlgorithmReturnStatement(invoke));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling core method 'System.Diagnostics.Debug.WriteLine'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the class : System.Diagnostics.Debug");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : 'Hello' (type:System.String)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "The method 'System.Diagnostics.Debug.WriteLine' is not awaitable because this method does not has the property IsAsync on true.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
