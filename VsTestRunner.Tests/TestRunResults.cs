using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Xunit.Abstractions;

namespace VsTestRunner.Tests
{
    public class TestRunResults : ITestRunEventsHandler
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TestRunResults(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public IList<TestResult> PassedTests { get; } = new List<TestResult>();
        public IList<TestResult> FailedTests { get; } = new List<TestResult>();
        public IList<TestResult> SkippedTests { get; } = new List<TestResult>();
        public TimeSpan ElapsedTime { get; private set; }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            switch (level)
            {
                case TestMessageLevel.Informational:
                    this.testOutputHelper.WriteLine($"INFO: {message}");
                    break;
                case TestMessageLevel.Warning:
                    this.testOutputHelper.WriteLine($"WARN: {message}");
                    break;
                case TestMessageLevel.Error:
                    this.testOutputHelper.WriteLine($"ERROR: {message}");
                    break;
                default:
                    this.testOutputHelper.WriteLine($"TRACE: {message}");
                    break;
            }
        }

        public void HandleRawMessage(string message)
        {
            this.testOutputHelper.WriteLine($"RAW: {message}");
        }

        public void HandleTestRunComplete(TestRunCompleteEventArgs testRunCompleteArgs, TestRunChangedEventArgs lastChunkArgs, ICollection<AttachmentSet> runContextAttachments, ICollection<string> executorUris)
        {
            HandleTestRunStatsChange(lastChunkArgs);
            this.ElapsedTime = testRunCompleteArgs.ElapsedTimeInRunningTests;
        }

        public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
        {
            if (testRunChangedArgs != null)
            {
                foreach (var testResult in testRunChangedArgs.NewTestResults)
                {
                    switch (testResult.Outcome)
                    {
                        case TestOutcome.Passed:
                            this.PassedTests.Add(testResult);
                            break;
                        case TestOutcome.Failed:
                            this.FailedTests.Add(testResult);
                            break;
                        case TestOutcome.Skipped:
                            this.SkippedTests.Add(testResult);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo)
        {
            return 0;
        }
    }
}
