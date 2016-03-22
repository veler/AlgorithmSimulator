using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class InvokeMethod_Test
    {
        [TestMethod]
        public void InvokeMethod()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod")));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling method 'This.FirstMethod'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the current instance : FirstClass");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "(FirstMethod) Return : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "(Main) Return : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeMethodReferenceClassNotFound()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("MyVar"));

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmVariableReferenceExpression("MyVar"), "FirstMethod")));
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
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Calling method 'MyVar.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Value of the variable 'MyVar' is {null}");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "It looks like the reference class does not exists.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeMethodVariableReference()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("MyVar"));

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("MyVar"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("FirstClass"))));
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmVariableReferenceExpression("MyVar"), "FirstMethod")));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 16);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Assign 'MyVar' to 'new FirstClass()'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Value of the variable 'MyVar' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Reference to the class : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Creating a new instance of 'FirstClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "'MyVar' is now equal to 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Calling method 'MyVar.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'MyVar' is 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[13].LogMessage, "(FirstMethod) Return : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[14].LogMessage, "(Main) Return : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[15].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeMethodThatDoesNotExist()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "MethodThatDoesNotExists")));
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
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling method 'This.MethodThatDoesNotExists'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.Log);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the current instance : FirstClass");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "The method 'MethodThatDoesNotExists' does not exists in the current class or is not accessible.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeMethodWithArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false, new AlgorithmParameterDeclaration("parm1"), new AlgorithmParameterDeclaration("param2"));

            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("param2")));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmThisReferenceExpression(), new AlgorithmPrimitiveExpression("Hello World!"))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling method 'This.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the current instance : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Reference to the current instance : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : 'Hello World!' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Variable 'parm1' declared in the method's argument => IsArray:False, DefaultValue:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'param2' declared in the method's argument => IsArray:False, DefaultValue:Hello World!");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "Value of the variable 'param2' is 'Hello World!' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "(FirstMethod) Return : 'Hello World!' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "(Main) Return : 'Hello World!' (type:System.String)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeMethodWithBadArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false, new AlgorithmParameterDeclaration("parm1"), new AlgorithmParameterDeclaration("param2", true));

            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("param2")));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmThisReferenceExpression(), new AlgorithmPrimitiveExpression("Hello World!"))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling method 'This.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the current instance : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Reference to the current instance : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : 'Hello World!' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Variable 'parm1' declared in the method's argument => IsArray:False, DefaultValue:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeMethodWithBadArgumentsCount()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false, new AlgorithmParameterDeclaration("param1"));

            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("param1")));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod")));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Calling method 'This.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Reference to the current instance : FirstClass");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "There is a method 'FirstMethod' in the class 'FirstClass', but it does not have 0 argument(s).");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        private AlgorithmProgram GetAsyncProgram(bool awaitCall)
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("MyVar"));

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", true);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("MyVar"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("FirstClass"))));

            var invoke = new AlgorithmInvokeMethodExpression(new AlgorithmVariableReferenceExpression("MyVar"), "FirstMethod");
            invoke.Await = awaitCall;
            entryPoint.Statements.Add(new AlgorithmReturnStatement(invoke));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            return program;
        }

        [TestMethod]
        public void InvokeMethodAsync()
        {
            var program = GetAsyncProgram(false);

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Assign 'MyVar' to 'new FirstClass()'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Value of the variable 'MyVar' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Reference to the class : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Creating a new instance of 'FirstClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "'MyVar' is now equal to 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Calling method 'MyVar.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'MyVar' is 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].LogMessage, "(Main) Return : {null}");

            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeMethodAsyncAwait()
        {
            var program = GetAsyncProgram(true);

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 16);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Assign 'MyVar' to 'new FirstClass()'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Value of the variable 'MyVar' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Reference to the class : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Creating a new instance of 'FirstClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "'MyVar' is now equal to 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Calling method 'MyVar.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'MyVar' is 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[13].LogMessage, "(FirstMethod) Return : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[14].LogMessage, "(Main) Return : '123' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[15].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeMethodAwaitButNotAsync()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            firstClass.Members.Add(new AlgorithmClassPropertyDeclaration("MyVar"));

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmPrimitiveExpression(123)));
            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("MyVar"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("FirstClass"))));

            var invoke = new AlgorithmInvokeMethodExpression(new AlgorithmVariableReferenceExpression("MyVar"), "FirstMethod");
            invoke.Await = true;
            entryPoint.Statements.Add(new AlgorithmReturnStatement(invoke));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Assign 'MyVar' to 'new FirstClass()'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Value of the variable 'MyVar' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Reference to the class : FirstClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Creating a new instance of 'FirstClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'MyVar' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "'MyVar' is now equal to 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Calling method 'MyVar.FirstMethod'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'MyVar' is 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeMethodRecursivity()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num > 1)
            //          num = FirstMethod(num - 1)
            //      return num;
            // }
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection()
            {
                new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("num"), new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1))))
            }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(5000))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 70000);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[69998].LogMessage, "(Main) Return : '1' (type:System.Int32)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[69999].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InvokeMethodRecursivityStackOverflow()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num > 1)
            //          return FirstMethod(num - 1)
            //      return num;
            // }
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
            task.Wait();
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 90016);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[90015].Error.Exception.Message, "You called too many (more than 10000) methods in the same thread.");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[90015].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeMethodRecursivityException()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num == 2)
            //          object num; // throw an error because "num" already exists.   
            //      else if (num > 1)
            //          return FirstMethod(num - 1)
            //      return num;
            // }
            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Equality, new AlgorithmPrimitiveExpression(2)), new AlgorithmStatementCollection() { new AlgorithmVariableDeclaration("num") }, new AlgorithmStatementCollection() { new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null) }));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(10))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 107);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);
            
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.Count, 10);
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Name, "num");
            Assert.AreEqual(algorithmInterpreter.DebugInfo.CallStackService.CallStacks.First().Stack.First().Variables[0].Value, (long)2);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[106].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InvokeMethodRecursivityStop()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // FirstMethod(num)
            // {
            //      if (num > 1)
            //          return FirstMethod(num - 1)
            //      return num;
            // }
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
            
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();

            algorithmInterpreter.Stop();

            Task.Delay(TimeSpan.FromSeconds(3)).Wait();
            
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Last().State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);
            
            AlgorithmInterpreter_Test.RunProgramStopWithoutDebug(program);
        }
    }
}
