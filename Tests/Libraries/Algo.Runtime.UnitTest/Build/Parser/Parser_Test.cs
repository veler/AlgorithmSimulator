using System.Collections.Generic;
using Algo.Runtime.Build.Parser;
using Algo.Runtime.Languages;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Parser
{
    [TestClass]
    public class Parser_Test
    {
        [TestMethod]
        public void SmallBasicParser()
        {
            var codeDocuments = new List<CodeDocument>();

            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    CLASS MyFirstClass\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyFirstClass");
            codeDocuments.Add(codeDocument);

            code = "CLASS MySecondClass\n" +
                   "\n" +
                   "END CLASS\n";

            codeDocument = new CodeDocument(code, "MySecondClass");
            codeDocuments.Add(codeDocument);

            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocuments, null).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Name, "MyApp");
            Assert.AreEqual(parser.AlgorithmProgram.Classes[0].Name.ToString(), "MyFirstClass");
            Assert.AreEqual(parser.AlgorithmProgram.Classes[1].Name.ToString(), "MySecondClass");
        }

        [TestMethod]
        public void DuplicatedProgram()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    CLASS MyClass\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "END PROGRAM\n" +
                       "\n" +
                       "PROGRAM MyDuplicatedApp\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code);
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in '{unnamed}', line 9 : 0 - A program can be defined only one time.");
        }

        [TestMethod]
        public void DuplicatedClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    CLASS MyFirstClass\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "    CLASS MyFirstClass\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code);
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in '{unnamed}', line 7 : 4 - The class 'MyFirstClass' already exists in '{unnamed}' line 3 : 4");
        }

        [TestMethod]
        public void ClassInClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    CLASS MyFirstClass\n" +
                       "\n" +
                       "    CLASS MySecondClass\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 5 : 4 - A class cannot be defined at this location. Please move this class outside of 'MyFirstClass'.");
        }

        [TestMethod]
        public void ProgramClosedBeforeClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    CLASS MyFirstClass\n" +
                       "\n" +
                       "    END PROGRAM\n" +
                       "\n" +
                       "END CLASS\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 5 : 4 - A class, method or block has not been closed.");
        }

        [TestMethod]
        public void ClassClosedWithoutBeginClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    END CLASS\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 4 - Class definition is missing.");
        }
    }
}
