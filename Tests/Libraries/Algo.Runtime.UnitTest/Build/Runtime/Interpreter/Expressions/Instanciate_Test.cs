using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class Instanciate_Test
    {
        [TestMethod]
        public void Instanciate()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 7);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "(Main) Return : 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateClassNotFound()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Unable to find the class 'SecondClass' because it does not exist or it is not accessible.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InstanciateWithConstructors()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("field1"));

            var ctor = new AlgorithmClassConstructorDeclaration();
            ctor.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("field1"), new AlgorithmPrimitiveExpression(123)));

            secondClass.Members.Add(ctor);

            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 13);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Variable 'field1' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Calling a constructor of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Assign 'field1' to ''123' (type:System.Int32)'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Value of the variable 'field1' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "'field1' is now equal to '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "(Main) Return : 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateWithArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"), new AlgorithmPrimitiveExpression(123))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("field1"));

            var ctor = new AlgorithmClassConstructorDeclaration(new AlgorithmParameterDeclaration("arg1"));
            ctor.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("field1"), new AlgorithmVariableReferenceExpression("arg1")));

            secondClass.Members.Add(ctor);

            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 15);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Variable 'field1' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Calling a constructor of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'arg1' declared in the method's argument => IsArray:False, DefaultValue:123");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "Assign 'field1' to 'arg1'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Value of the variable 'field1' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'arg1' is '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].LogMessage, "'field1' is now equal to '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[13].LogMessage, "(Main) Return : 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[14].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateWithBadArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"), new AlgorithmPrimitiveExpression(123))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("field1"));

            var ctor = new AlgorithmClassConstructorDeclaration(new AlgorithmParameterDeclaration("arg1", true));
            ctor.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("field1"), new AlgorithmVariableReferenceExpression("arg1")));

            secondClass.Members.Add(ctor);

            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 9);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Variable 'field1' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Calling a constructor of 'SecondClass'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InstanciateWithBadArgumentsCount()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("field1"));

            var ctor = new AlgorithmClassConstructorDeclaration(new AlgorithmParameterDeclaration("arg1"));
            ctor.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("field1"), new AlgorithmVariableReferenceExpression("arg1")));

            secondClass.Members.Add(ctor);

            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 7);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Variable 'field1' declared in the class => IsArray:False, DefaultValue:{null}");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "There is no constructor with 0 argument(s) in the class 'SecondClass'.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InstanciateWithSeveralConstructors()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"), new AlgorithmPrimitiveExpression(123))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassPropertyDeclaration("field1"));
            secondClass.Members.Add(new AlgorithmClassConstructorDeclaration());

            var ctor = new AlgorithmClassConstructorDeclaration(new AlgorithmParameterDeclaration("arg1"));
            ctor.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("field1"), new AlgorithmVariableReferenceExpression("arg1")));

            secondClass.Members.Add(ctor);

            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 15);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Variable 'field1' declared in the class => IsArray:False, DefaultValue:{null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Calling a constructor of 'SecondClass'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Variable 'arg1' declared in the method's argument => IsArray:False, DefaultValue:123");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "Assign 'field1' to 'arg1'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].LogMessage, "Value of the variable 'field1' is {null}");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[11].LogMessage, "Value of the variable 'arg1' is '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[12].LogMessage, "'field1' is now equal to '123' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[13].LogMessage, "(Main) Return : 'Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter' (type:Algo.Runtime.Build.Runtime.Interpreter.Interpreter.ClassInterpreter)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[14].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateWithIdenticalConstructors()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("SecondClass"))));
            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);

            var secondClass = new AlgorithmClassDeclaration("SecondClass");
            secondClass.Members.Add(new AlgorithmClassConstructorDeclaration());
            secondClass.Members.Add(new AlgorithmClassConstructorDeclaration());

            program.Classes.Add(secondClass);

            program.UpdateEntryPointPath();

            var algorithmInterpreter = new AlgorithmInterpreter(program);

            var task = algorithmInterpreter.StartAsync(debugMode: true);

            task.Wait();

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory.Count, 6);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[0].State, AlgorithmInterpreterState.Ready);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[1].State, AlgorithmInterpreterState.Preparing);
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[2].State, AlgorithmInterpreterState.Running);

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : SecondClass");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'SecondClass'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "A class should not have multiple constructors with the same number of arguments.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
