using System.Collections.Generic;
using System.Threading.Tasks;
using Algo.Runtime.Build.Runtime.Memory;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Algo.Runtime.UnitTest.Build.Runtime.Memory
{
    [TestClass]
    public class Variable_Test
    {
        class Foo
        {
            private int _integer = 10;
            public object _object;
            public List<long> _listLong;
            public long Long { get; set; }
            public string String { get; set; }
        }

        private Dictionary<object, ulong> GetTests()
        {
            // key = the value to set to the Variable.
            // value = the expected size in byte of the value in the Variable
            return new Dictionary<object, ulong>
            {
                {"a", 1}, //1B
                {"hello world", 11}, // 11B
                {new int(), 4}, // 4B
                {new long(), 8}, // 8B
                {new double(), 8}, // 8B
                {new Foo(), 16}, // 16B
                {new List<long>(), 4}, // 4B
                {new List<long>() { 1, 2, 3 }, 4 + (8 * 3) }, // 28B
                {new List<int>() { 1, 2, 3 }, 4 + (4 * 3) }, // 20B
                {new int[] { 1, 2, 3 }, 4 + (4 * 3) }, // 20B
                {new List<object>() { 1, 2, 3 }, 4 + 4 + 4 + 4}, // 16B
                {new List<object>() { new List<object>() { 1, 2, 3 } }, 4 + 4 + 4 + 4 + 4}, // 20B
                {new Foo() {String = "hello", Long = 10, _listLong = new List<long>() { 1, 2, 3 }, _object = new Foo() {_listLong = new List<long>() { 1, 2, 3 }}}
                , 93 } // 93B
            };
        }

        [TestMethod]
        public void VariableWithoutMemTrace()
        {
            Task<ulong> task;
            var variable = new Variable("myVar", debugMode: false);
            var tests = GetTests();

            foreach (var test in tests)
            {
                variable.Value = test.Key;

                task = variable.GetSizeAsync();
                task.Wait();

                Assert.AreEqual(task.Result, test.Value);
            }
        }

        [TestMethod]
        public void VariableWithMemTrace()
        {
            Task<ulong> task;
            var variable = new Variable("myVar", debugMode: true);
            var tests = GetTests();

            foreach (var test in tests)
            {
                variable.Value = test.Key; // The size is calculated asynchronously. No way to stop it.

                new System.Threading.ManualResetEvent(false).WaitOne(25); // Slowing down the test to let the time to calculate the size in background

                task = variable.GetSizeAsync();
                task.Wait();

                Assert.AreEqual(task.Result, test.Value);
            }
        }
    }
}