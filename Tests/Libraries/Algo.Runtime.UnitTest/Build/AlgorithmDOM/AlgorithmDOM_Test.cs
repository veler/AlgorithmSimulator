using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;

namespace Algo.Runtime.UnitTest.Build.AlgorithmDOM
{
    [TestClass]
    public class AlgorithmDOM_Test
    {
        [TestMethod]
        public void AlgorithmIdentifier()
        {
            Assert.IsNotNull(new AlgorithmIdentifier("VariableName"));
            Assert.IsNotNull(new AlgorithmIdentifier(" Variable Name "));
            Assert.ThrowsException<ArgumentException>(() => { new AlgorithmIdentifier(""); });
            Assert.ThrowsException<ArgumentException>(() => { new AlgorithmIdentifier(" "); });
            Assert.ThrowsException<ArgumentException>(() => { new AlgorithmIdentifier("     "); });

            var identifier = new AlgorithmIdentifier(" Variable Name ");
            Assert.AreEqual(identifier.ToString(), " Variable Name ");
        }

        [TestMethod]
        public void AlgorithmObject()
        {
            var idList = new List<string>();

            for (var i = 0; i < 1000; i++)
            {
                var algoObject = new AlgorithmExpressionStatement();
                idList.Add(algoObject.Id.ToString());
            }

            for (int i = 0; i < idList.Count; i++)
            {
                for (int j = 0; j < idList.Count; j++)
                {
                    if (i != j)
                        Assert.AreNotEqual(idList[i], idList[j]);
                }
            }
        }

        [TestMethod]
        public void AlgorithmProgram()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");
            var secondClass = new AlgorithmClassDeclaration("SecondClass");

            program.Classes.Add(firstClass);
            program.Classes.Add(secondClass);

            Assert.ThrowsException<RankException>(() => { program.UpdateEntryPointPath(); });
            Assert.IsNull(program.EntryPointPath);
            Assert.AreEqual(program.GetEntryPointMethodCount(), 0);
            Assert.IsNull(program.GetEntryPointMethod());

            secondClass.Members.Add(new AlgorithmEntryPointMethod());

            program.UpdateEntryPointPath();

            Assert.AreEqual(program.EntryPointPath, "SecondClass");
            Assert.AreEqual(program.GetEntryPointMethodCount(), 1);
            Assert.IsNotNull(program.GetEntryPointMethod());

            secondClass.Members.Add(new AlgorithmEntryPointMethod());

            Assert.ThrowsException<RankException>(() => { program.UpdateEntryPointPath(); });
            Assert.AreEqual(program.EntryPointPath, "SecondClass");
            Assert.AreEqual(program.GetEntryPointMethodCount(), 2);
            Assert.IsNotNull(program.GetEntryPointMethod());
        }

        [TestMethod]
        public void AlgorithmClassReferenceExpression()
        {
            Assert.IsNotNull(new AlgorithmClassReferenceExpression());
            Assert.IsNotNull(new AlgorithmClassReferenceExpression("MyClass"));
            Assert.IsNotNull(new AlgorithmClassReferenceExpression("Namespace.namespace", "MyClass"));

            Assert.ThrowsException<ArgumentException>(() => { new AlgorithmClassReferenceExpression("Namespace. namespace", "MyClass"); });
            Assert.ThrowsException<ArgumentException>(() => { new AlgorithmClassReferenceExpression("Namespace.namespace", " "); });

            var classRef = new AlgorithmClassReferenceExpression("Namespace.namespace", "MyClass");
            Assert.AreEqual(classRef.ToString(), "Namespace.namespace.MyClass");
        }
    }
}
