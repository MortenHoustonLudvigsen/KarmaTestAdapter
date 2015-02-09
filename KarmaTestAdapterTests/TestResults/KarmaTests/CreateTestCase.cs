using KarmaTestAdapter;
using KarmaTestAdapter.TestResults;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.KarmaTests
{
    partial class TestResults
    {
        partial class Karma
        {
            private static class CreateTestCaseExpected
            {
                public const string FullyQualifiedName = "C:.Git.KarmaTestAdapter.karma-vs-reporter.test.testfiles.TestFile2.Simple tests 2 / Nested tests.should be, that 3 + 12 = 23#276";
                public static readonly Uri ExecutorUri = Globals.ExecutorUri;
                public const string Source = "Source";
                public const string DisplayName = "Simple tests 2 Nested tests should be, that 3 + 12 = 23";
                public const int Index = 276;
                public const string CodeFilePath = @"C:\Git\KarmaTestAdapter\karma-vs-reporter\test\testfiles\TestFile2.ts";
                public const int LineNumber = 9;
            }

            private TestCase CreateTestCase()
            {
                return KarmaTestAdapter.TestResults.Karma.CreateTestCase(Item.Files.AllTests.First(), "Source");
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return not null")]
            public void CreateTestCaseShouldReturnNotNull()
            {
                Assert.NotNull(CreateTestCase());
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct FullyQualifiedName")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectFullyQualifiedName()
            {
                Assert.Equal(CreateTestCaseExpected.FullyQualifiedName, CreateTestCase().FullyQualifiedName);
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct ExecutorUri")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectExecutorUri()
            {
                Assert.Equal(CreateTestCaseExpected.ExecutorUri, CreateTestCase().ExecutorUri);
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct Source")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectSource()
            {
                Assert.Equal(CreateTestCaseExpected.Source, CreateTestCase().Source);
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct DisplayName")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectDisplayName()
            {
                Assert.Equal(CreateTestCaseExpected.DisplayName, CreateTestCase().DisplayName);
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct Index")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectIndex()
            {
                Assert.Equal(CreateTestCaseExpected.Index, CreateTestCase().GetPropertyValue<int>(Globals.FileIndexProperty, 0));
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct CodeFilePath")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectCodeFilePath()
            {
                Assert.Equal(CreateTestCaseExpected.CodeFilePath, CreateTestCase().CodeFilePath);
            }

            [Fact(DisplayName = "CreateTestCase(<test>, <source>) should return TestCase with correct LineNumber")]
            public void CreateTestCaseShouldReturnTestCaseWithCorrectLineNumber()
            {
                Assert.Equal(CreateTestCaseExpected.LineNumber, CreateTestCase().LineNumber);
            }
        }
    }
}
