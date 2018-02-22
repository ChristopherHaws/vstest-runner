using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Abstractions;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 8)]

namespace VsTestRunner.Tests
{
    public class MsTestOutputHelper : ITestOutputHelper
    {
        private readonly TestContext context;
        private readonly String prefix;

        public MsTestOutputHelper(TestContext context, String prefix = null)
        {
            this.context = context;
            this.prefix = prefix ?? String.Empty;
        }

        public void WriteLine(String message)
        {
            this.context.WriteLine(this.prefix + message);
        }

        public void WriteLine(String format, params Object[] args)
        {
            this.context.WriteLine(this.prefix + format, args);
        }
    }

    [TestClass]
    public class MsTestTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        [DataRow(11)]
        public void TestMethod2(Int32 testNumber)
        {
            /*
             Findings:

             The issue doesnt occure when runnign with mstest
            */
            var logger = new MsTestOutputHelper(this.TestContext, testNumber.ToString());
            using (var runner = new VsTestRunner(logger))
            {
                var bin = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "..", "SampleTests", "bin", "Debug", "net47");
                var results = runner.InvokeVsTestForExecution(new[] { Path.Combine(bin, "SampleTests.dll") });

                Assert.AreEqual(1, results.PassedTests.Count);
            }
        }
    }
}
