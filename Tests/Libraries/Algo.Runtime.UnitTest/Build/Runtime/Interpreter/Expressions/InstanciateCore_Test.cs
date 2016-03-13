using System;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Interpreter.Expressions
{
    [TestClass]
    public class InstanciateCore_Test
    {
        [TestMethod]
        public void InstanciateCore()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression("1.0.0.0"))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : '1.0.0.0' (type:System.String)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "(Main) Return : '1.0.0.0' (type:System.Version)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateCoreClassNotFound()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version2"), new AlgorithmPrimitiveExpression("1.0.0.0"))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version2");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Unable to find the class 'System.Version2' because it does not exist or it is not accessible.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
        
        [TestMethod]
        public void InstanciateCoreWithArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression(1), new AlgorithmPrimitiveExpression(0), new AlgorithmPrimitiveExpression(0), new AlgorithmPrimitiveExpression(0))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : '1' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[7].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[8].LogMessage, "Primitive value : '0' (type:System.Int32)");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[9].LogMessage, "(Main) Return : '1.0.0.0' (type:System.Version)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[10].State, AlgorithmInterpreterState.Stopped);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.Stopped);

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program);
        }

        [TestMethod]
        public void InstanciateCoreWithBadArguments()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"), new AlgorithmPrimitiveExpression(new DateTime()))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].LogMessage, "Primitive value : '1/1/0001 12:00:00 AM' (type:System.DateTime)");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[6].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Constructor on type 'System.Version' not found.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }

        [TestMethod]
        public void InstanciateCoreWithBadArgumentsCount()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            var entryPoint = new AlgorithmEntryPointMethod();
            entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression("System", "Version"))));
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

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[3].LogMessage, "Reference to the class : System.Version");
            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[4].LogMessage, "Creating a new instance of 'System.Version'");

            Assert.AreEqual(algorithmInterpreter.StateChangeHistory[5].State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.State, AlgorithmInterpreterState.StoppedWithError);
            Assert.AreEqual(algorithmInterpreter.Error.Exception.Message, "Constructor on type 'System.Version' not found.");

            AlgorithmInterpreter_Test.RunProgramWithoutDebug(program, true);
        }
    }
}
