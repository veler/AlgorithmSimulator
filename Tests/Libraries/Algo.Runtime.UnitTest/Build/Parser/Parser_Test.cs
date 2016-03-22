using System.Collections.Generic;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser;
using Algo.Runtime.Languages;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Parser
{
    [TestClass]
    public class Parser_Test
    {
        [TestMethod]
        public void ParserEnglish()
        {
            var codeDocuments = new List<CodeDocument>();

            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyFirstClass");
            codeDocuments.Add(codeDocument);

            code = "MODEL MySecondClass\n" +
                   "\n" +
                   "END MODEL\n";

            codeDocument = new CodeDocument(code, "MySecondClass");
            codeDocuments.Add(codeDocument);

            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocuments, null).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Name, "MyApp");
            Assert.AreEqual(parser.AlgorithmProgram.Classes[0].Name.ToString(), "MyFirstClass");
            Assert.AreEqual(parser.AlgorithmProgram.Classes[1].Name.ToString(), "MySecondClass");
        }

        [TestMethod]
        public void ParserFrench()
        {
            var codeDocuments = new List<CodeDocument>();

            var code = "PROGRAMME MyApp\n" +
                       "\n" +
                       "    MODELE MyFirstClass\n" +
                       "\n" +
                       "    FIN MODELE\n" +
                       "\n" +
                       "FIN PROGRAMME\n";

            var codeDocument = new CodeDocument(code, "MyFirstClass");
            codeDocuments.Add(codeDocument);

            code = "MODELE MySecondClass\n" +
                   "\n" +
                   "FIN MODELE\n";

            codeDocument = new CodeDocument(code, "MySecondClass");
            codeDocuments.Add(codeDocument);

            var parser = new Parser<AlgoBASIC>(AlgoBASIC.Culture.French);

            parser.ParseAsync(codeDocuments, null).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Name, "MyApp");
            Assert.AreEqual(parser.AlgorithmProgram.Classes[0].Name.ToString(), "MyFirstClass");
            Assert.AreEqual(parser.AlgorithmProgram.Classes[1].Name.ToString(), "MySecondClass");
        }

        [TestMethod]
        public void ParserBasicSyntaxError()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyClass kjfdghfdkjghdfkjdhk\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code);
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in '{unnamed}', line 3 : 17 - Cannot resolve this symbol.");
        }

        [TestMethod]
        public void ParserFunctionSyntaxError()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "        FUNCTION ASYNC MyMethod()\n" +
                       "\n" +
                       "        END FUNCTION\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code);
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in '{unnamed}', line 5 : 16 - Cannot resolve this symbol.");
        }

        [TestMethod]
        public void ParserDuplicatedProgram()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyClass\n" +
                       "\n" +
                       "    END MODEL\n" +
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
        public void ParserDuplicatedClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code);
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in '{unnamed}', line 7 : 4 - The class 'MyFirstClass' already exists in '{unnamed}' line 3 : 4");
        }

        [TestMethod]
        public void ParserClassInClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "    MODEL MySecondClass\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 5 : 4 - A class cannot be defined at this location. Please move this class outside of 'MyFirstClass'.");
        }

        [TestMethod]
        public void ParserProgramClosedBeforeClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "    END PROGRAM\n" +
                       "\n" +
                       "END MODEL\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 5 : 4 - A class, method or block has not been closed.");
        }

        [TestMethod]
        public void ParserClassClosedWithoutBeginClass()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 4 - Class definition is missing.");
        }

        [TestMethod]
        public void ParserVariableDeclaration()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "        VARIABLE ArrayVariable[]\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Variables.First().Name.ToString(), "NormalVariable");
            Assert.AreEqual(parser.AlgorithmProgram.Variables.First().IsArray, false);
            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members.First()).Name.ToString(), "ArrayVariable");
            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members.First()).IsArray, true);
        }

        [TestMethod]
        public void ParserFunctionDeclaration()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "        FUNCTION Main()\n" +
                       "\n" +
                       "        END FUNCTION\n" +
                       "\n" +
                       "        ASYNC FUNCTION MyMethod(arg1, arg2, arg3[])\n" +
                       "            VARIABLE test\n" +
                       "        END FUNCTION\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Classes.First().Name.ToString(), "MyFirstClass");
            Assert.AreEqual(parser.AlgorithmProgram.Classes.First().Members.First().Name.ToString(), "Main");
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Name.ToString(), "MyMethod");
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).IsAsync, true);
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Arguments[2].Name.ToString(), "arg3");
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Arguments[2].IsArray, true);
            Assert.AreEqual(((AlgorithmVariableDeclaration)((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Statements.First()).Name.ToString(), "test");
        }

        [TestMethod]
        public void ParserConstructorDeclaration()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "        FUNCTION Initialize(arg1, arg2, arg3[])\n" +
                       "\n" +
                       "        END FUNCTION\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Classes.First().Name.ToString(), "MyFirstClass");
            Assert.AreEqual(((AlgorithmClassConstructorDeclaration)parser.AlgorithmProgram.Classes.First().Members.First()).Name.ToString(), "ctor");
            Assert.AreEqual(((AlgorithmClassConstructorDeclaration)parser.AlgorithmProgram.Classes.First().Members.First()).Arguments[2].Name.ToString(), "arg3");
            Assert.AreEqual(((AlgorithmClassConstructorDeclaration)parser.AlgorithmProgram.Classes.First().Members.First()).Arguments[2].IsArray, true);
        }

        [TestMethod]
        public void ParserEntryPointError()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "        FUNCTION Main(arg1)\n" +
                       "\n" +
                       "        END FUNCTION\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 5 : 8 - The entry point method cannot take any argument.");
        }

        [TestMethod]
        public void ParserComments()
        {
            var code = "PROGRAM MyApp # comment\n" +
                       "\n" +
                       "    MODEL MyFirstClass # comment\n" +
                       "\n" +
                       "        FUNCTION Main() # comment\n" +
                       "            # comment\n" +
                       "        END FUNCTION # comment\n" +
                       "\n" +
                       "        ASYNC FUNCTION MyMethod(arg1, arg2, arg3[])\n" +
                       "\n" +
                       "        END FUNCTION\n" +
                       "\n" +
                       "    END MODEL # comment for fun\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Classes.First().Name.ToString(), "MyFirstClass");
            Assert.AreEqual(parser.AlgorithmProgram.Classes.First().Members.First().Name.ToString(), "Main");
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Name.ToString(), "MyMethod");
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).IsAsync, true);
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Arguments[2].Name.ToString(), "arg3");
            Assert.AreEqual(((AlgorithmClassMethodDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Arguments[2].IsArray, true);
        }
    }
}
