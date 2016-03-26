using System.Collections.Generic;
using System.Linq;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Parser;
using Algo.Runtime.Languages;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Parser
{
    [TestClass]
    public class Parser_Variables_Test
    {
        [TestMethod]
        public void ParserVariableDeclaration()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable\n" +
                       "\n" +
                       "    VARIABLE NormalVariableString = \"Hello World\" # this is a comment\n" +
                       "\n" +
                       "    VARIABLE NormalVariableCharacter = \'A\'\n" +
                       "\n" +
                       "    VARIABLE NormalVariableNumber = -123.4\n" +
                       "\n" +
                       "    MODEL MyFirstClass\n" +
                       "\n" +
                       "        VARIABLE ArrayVariable[]\n" +
                       "\n" +
                       "        VARIABLE ArrayVariableInitialized[] = [\"item1\", \"item2\"]\n" +
                       "\n" +
                       "        VARIABLE ArrayVariableInitialized2[] = [  \"item1\"   , [\"item2_item1\", \"item2_item2\"   ]   ]\n" +
                       "\n" +
                       "    END MODEL\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.AlgorithmProgram.Variables[0].Name.ToString(), "NormalVariable");
            Assert.AreEqual(parser.AlgorithmProgram.Variables[0].IsArray, false);
            Assert.AreEqual(parser.AlgorithmProgram.Variables[0].DefaultValue, null);

            Assert.AreEqual(parser.AlgorithmProgram.Variables[1].Name.ToString(), "NormalVariableString");
            Assert.AreEqual(parser.AlgorithmProgram.Variables[1].IsArray, false);
            Assert.AreEqual(parser.AlgorithmProgram.Variables[1].DefaultValue.Value, "Hello World");

            Assert.AreEqual(parser.AlgorithmProgram.Variables[2].Name.ToString(), "NormalVariableCharacter");
            Assert.AreEqual(parser.AlgorithmProgram.Variables[2].IsArray, false);
            Assert.AreEqual(parser.AlgorithmProgram.Variables[2].DefaultValue.Value, "A");

            Assert.AreEqual(parser.AlgorithmProgram.Variables[3].Name.ToString(), "NormalVariableNumber");
            Assert.AreEqual(parser.AlgorithmProgram.Variables[3].IsArray, false);
            Assert.AreEqual(parser.AlgorithmProgram.Variables[3].DefaultValue.Value, (decimal)-123.4);

            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[0]).Name.ToString(), "ArrayVariable");
            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[0]).IsArray, true);
            var val = (List<object>)((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[0]).DefaultValue.Value;
            Assert.AreEqual(val.Count, 0);

            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).Name.ToString(), "ArrayVariableInitialized");
            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).IsArray, true);
            val = (List<object>)((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[1]).DefaultValue.Value;
            Assert.AreEqual(val.Count, 2);
            Assert.AreEqual(val[0], "item1");
            Assert.AreEqual(val[1], "item2");

            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[2]).Name.ToString(), "ArrayVariableInitialized2");
            Assert.AreEqual(((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[2]).IsArray, true);
            val = (List<object>)((AlgorithmClassPropertyDeclaration)parser.AlgorithmProgram.Classes.First().Members[2]).DefaultValue.Value;
            Assert.AreEqual(val.Count, 2);
            Assert.AreEqual(val[0], "item1");
            Assert.AreEqual(((List<object>)val[1])[0], "item2_item1");
            Assert.AreEqual(((List<object>)val[1])[1], "item2_item2");
        }

        [TestMethod]
        public void ParserVariableDeclarationMethodExpression()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable = Test()\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 27 - Cannot resolve this symbol.");
        }

        [TestMethod]
        public void ParserVariableDeclarationArrayBadDefaultValue()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable[] = \"Hello World\"\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 4 - An array value is expected for 'NormalVariable'");
        }

        [TestMethod]
        public void ParserVariableDeclarationSyntaxError()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable = |\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 27 - Cannot resolve this symbol.");
        }

        [TestMethod]
        public void ParserVariableDeclarationArraySyntaxError()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable[] = [ 1, 2\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 29 - Cannot resolve this symbol.");
        }

        [TestMethod]
        public void ParserVariableDeclarationArraySyntaxError2()
        {
            var code = "PROGRAM MyApp\n" +
                       "\n" +
                       "    VARIABLE NormalVariable[] = [ 1 | 2 ]\n" +
                       "\n" +
                       "END PROGRAM\n";

            var codeDocument = new CodeDocument(code, "MyApp");
            var parser = new Parser<AlgoBASIC>();

            parser.ParseAsync(codeDocument).Wait();

            Assert.AreEqual(parser.Error.Message, "Syntax error in 'MyApp', line 3 : 4 - String, charachter or number expected.");
        }
    }
}
