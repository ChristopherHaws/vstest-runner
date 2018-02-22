using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace VsTestRunner.Tests
{
    public abstract class XUnitTestsBase
    {
        private readonly ITestOutputHelper logger;

        public XUnitTestsBase(ITestOutputHelper logger) => this.logger = logger;

        [Fact]
        public void TestMethod2()
        {
            /*
             Findings:

             The issue occures based on the xunit setting maxParallelThreads.
                 When you run < maxParallelThreads tests in parallel, they work.
                 When you run >= maxParallelThreads tests in parallel, they fail.
             */
            using (var runner = new VsTestRunner(this.logger))
            {
                var bin = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "..", "SampleTests", "bin", "Debug", "net47");
                runner.InvokeVsTestForExecution(new[] { Path.Combine(bin, "SampleTests.dll") });
            }
        }
    }

    public class XUnitTest1 : XUnitTestsBase { public XUnitTest1(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest2 : XUnitTestsBase { public XUnitTest2(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest3 : XUnitTestsBase { public XUnitTest3(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest4 : XUnitTestsBase { public XUnitTest4(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest5 : XUnitTestsBase { public XUnitTest5(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest6 : XUnitTestsBase { public XUnitTest6(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest7 : XUnitTestsBase { public XUnitTest7(ITestOutputHelper logger) : base(logger) { } }
    public class XUnitTest8 : XUnitTestsBase { public XUnitTest8(ITestOutputHelper logger) : base(logger) { } }
}
