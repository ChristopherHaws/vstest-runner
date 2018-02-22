using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Xunit.Abstractions;

namespace VsTestRunner.Tests
{
    public class VsTestRunner : IDisposable
    {
        private static readonly Semaphore VsTestRunnerSemaphore = new Semaphore(1, 1, nameof(VsTestRunnerSemaphore));

        private readonly ITestOutputHelper logger;
        private readonly String vstestConsolePath;

        private VsTestConsoleWrapper vstest;
        private volatile Boolean isInitialized;

        public VsTestRunner(ITestOutputHelper logger)
        {
            this.logger = logger;

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var packagePath = Path.Combine(userProfile, ".nuget", "packages", "microsoft.testplatform", "15.6.0");
            this.vstestConsolePath = Path.Combine(packagePath, "tools", "net451", "Common7", "IDE", "Extensions", "TestPlatform", "vstest.console.exe");
        }

        ~VsTestRunner()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.isInitialized)
            {
                this.vstest?.EndSession();
                this.isInitialized = false;
            }

            GC.SuppressFinalize(this);
        }

        private void EnsureInitialized(String[] testAdapterPaths)
        {
            if (this.isInitialized)
            {
                return;
            }

            try
            {
                VsTestRunnerSemaphore.WaitOne();

                this.logger.WriteLine("Starting vstest...");

                this.vstest = new VsTestConsoleWrapper(this.vstestConsolePath);
                this.vstest.StartSession();
                this.isInitialized = true;
            }
            finally
            {
                VsTestRunnerSemaphore.Release();
            }
        }

        public TestRunResults InvokeVsTestForExecution(String[] sources)
        {
            var (runSettings, testAdapters) = this.GetSettings(sources);

            this.EnsureInitialized(testAdapters);

            var results = new TestRunResults(this.logger);

            this.vstest.RunTests(sources, runSettings, results);

            return results;
        }

        private (String runSettings, String[] testAdapters) GetSettings(String[] sources)
        {
            var testAdapterPaths = sources
                .Select(x => Path.GetDirectoryName(x))
                .Distinct()
                .ToList();

            var testAdapters = testAdapterPaths
                .SelectMany(x => Directory.GetFiles(x, "*TestAdapter.dll"))
                .Distinct()
                .ToArray();

            var runSettings = $@"
<?xml version=""1.0"" encoding=""utf - 8""?>
<RunSettings>
    <DataCollectionRunSettings>
        <DataCollectors>
        </DataCollectors>
    </DataCollectionRunSettings>
    <RunConfiguration>
        <TestAdaptersPaths>{String.Join(";", testAdapterPaths)}</TestAdaptersPaths>
    </RunConfiguration>
</RunSettings>".Trim();

            return (runSettings, testAdapters);
        }
    }
}
