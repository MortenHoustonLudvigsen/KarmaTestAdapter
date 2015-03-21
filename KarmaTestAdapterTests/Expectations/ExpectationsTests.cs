using JsTestAdapter.Logging;
using KarmaTestAdapter;
using KarmaTestAdapter.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations.ExpectationsTests
{
    public class ExpectationsTests : BaseFixture
    {
        private static string ExpectationsDir = Path.GetFullPath(Path.Combine(Globals.AssemblyDirectory, @"..\..\..\TestProjects"));

        private static Expectations CreateExpectations()
        {
            return new Expectations(ExpectationsDir);
        }

        private static Lazy<Expectations> _expectations = new Lazy<Expectations>(CreateExpectations);

        public static void Reset()
        {
            _expectations = new Lazy<Expectations>(CreateExpectations);
        }

        public Expectations Expectations { get { return _expectations.Value; } }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Reset();
        }

        [Test, TestCaseSource("GetProjectShouldBeValidData")]
        public async void ProjectShouldBeValid(ProjectTestCase testCase)
        {
            Console.WriteLine(await testCase.GetOutput());
            Assert.That(testCase.IsValid, testCase.InvalidReason);
        }

        private IEnumerable<ProjectTestCase> GetProjectShouldBeValidData()
        {
            return Expectations.GetProjectTestCases()
                .Select(t => t.SetDescription("should be valid"));
        }

        [Test, TestCaseSource("GetProjectShouldNotHaveUnexpectedKarmaSpecsData")]
        public async void ProjectShouldNotHaveUnexpectedKarmaSpecs(ProjectTestCase testCase)
        {
            var unexpectedKarmaSpecs = await testCase.Expected.GetUnexpectedKarmaSpecs();
            Console.WriteLine(unexpectedKarmaSpecs.Format());
            Assert.That(!unexpectedKarmaSpecs.Any(), "{0} unexpected karma specs", unexpectedKarmaSpecs.Count());
        }

        private IEnumerable<ProjectTestCase> GetProjectShouldNotHaveUnexpectedKarmaSpecsData()
        {
            return Expectations.GetProjectTestCases()
                .Select(t => t.SetDescription("should not have unexpected karma specs"));
        }

        [Test, TestCaseSource("GetSpecShouldBeValidData")]
        public void SpecShouldBeValid(SpecTestCase testCase)
        {
            Assert.That(testCase.IsValid, testCase.InvalidReason);
        }

        private IEnumerable<SpecTestCase> GetSpecShouldBeValidData()
        {
            return Expectations.GetSpecTestCases()
                .Select(t => t.SetDescription("should be valid"));
        }

        [Test, TestCaseSource("GetShouldHaveKarmaSpecData")]
        public async void ShouldHaveKarmaSpec(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
        }

        private IEnumerable<SpecTestCase> GetShouldHaveKarmaSpecData()
        {
            return Expectations.GetSpecTestCases()
                .Select(t => t.SetDescription("should have karma spec"));
        }

        [Test, TestCaseSource("GetShouldHaveResultsData")]
        public async void ShouldHaveResults(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Results.Any(), Is.True);
        }

        private IEnumerable<SpecTestCase> GetShouldHaveResultsData()
        {
            return Expectations.GetSpecTestCases()
                .Select(t => t.SetDescription("should have results"));
        }

        [Test, TestCaseSource("GetShouldHaveFileNameData")]
        public async void ShouldHaveFileName(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Source, Is.Not.Null, "Source missing");
            Assert.That(karmaSpec.Source.FileName, Is.SamePath(testCase.Spec.FileName));
        }

        private IEnumerable<SpecTestCase> GetShouldHaveFileNameData()
        {
            return Expectations.GetSpecTestCases()
                .Where(t => !string.IsNullOrWhiteSpace(t.Spec.FileName))
                .Select(t => t.SetDescription("should have file name '{0}'", t.Spec.FileName));
        }

        [Test, TestCaseSource("GetShouldHaveLineNumberData")]
        public async void ShouldHaveLineNumber(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Source, Is.Not.Null, "Source missing");
            Assert.That(karmaSpec.Source.LineNumber, new BetweenConstraint(testCase.Spec.LineNumberFrom, testCase.Spec.LineNumberTo), "Line number");
        }

        private IEnumerable<SpecTestCase> GetShouldHaveLineNumberData()
        {
            return Expectations.GetSpecTestCases()
                .Where(t => t.Spec.LineNumberFrom.HasValue && t.Spec.LineNumberTo.HasValue)
                .Select(t => t.SetDescription("should have line number between {0} and {1}", t.Spec.LineNumberFrom, t.Spec.LineNumberTo));
        }

        [Test, TestCaseSource("GetShouldSucceedData")]
        public async void ShouldSucceed(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Results, Is.Not.Null, "Karma spec results missing");
            Assert.That(karmaSpec.Results.All(r => r.Success), "Some results did not succeed");
        }

        private IEnumerable<SpecTestCase> GetShouldSucceedData()
        {
            return Expectations.GetSpecTestCases()
                .Where(t => t.Spec.Success)
                .Select(t => t.SetDescription("should succeed"));
        }

        [Test, TestCaseSource("GetShouldFailData")]
        public async void ShouldFail(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Results, Is.Not.Null, "Karma spec results missing");
            Assert.That(karmaSpec.Results.All(r => !r.Success), "Some results did not fail");
        }

        private IEnumerable<SpecTestCase> GetShouldFailData()
        {
            return Expectations.GetSpecTestCases()
                .Where(t => !t.Spec.Success)
                .Select(t => t.SetDescription("should fail"));
        }

        [Test, TestCaseSource("GetShouldHaveStackTraceData")]
        public async void ShouldHaveStackTrace(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Results, Is.Not.Null, "Karma spec results missing");
            foreach (var result in karmaSpec.Results)
            {
                string actualStack = null;
                if (result.Failures != null && result.Failures.Any())
                {
                    actualStack = string.Join(Environment.NewLine, result.Failures.First().stack.Take(testCase.Spec.Stack.Count()));
                }
                Assert.That(actualStack, Is.EqualTo(testCase.Spec.StackTrace), "Stack trace incorrect for {0}", result.Name);
            }
        }

        private IEnumerable<SpecTestCase> GetShouldHaveStackTraceData()
        {
            return Expectations.GetSpecTestCases()
                .Where(t => !t.Spec.Success)
                .Select(t => t.SetDescription("should have stack trace"));
        }

        [Test, TestCaseSource("GetShouldHaveOutputData")]
        public async void ShouldHaveOutput(SpecTestCase testCase)
        {
            var karmaSpec = await testCase.GetKarmaSpec();
            Console.WriteLine(karmaSpec.Format());
            Assert.That(karmaSpec, Is.Not.Null, "Karma spec missing");
            Assert.That(karmaSpec.Results, Is.Not.Null, "Karma spec results missing");
            foreach (var result in karmaSpec.Results)
            {
                Assert.That(result.Output, Is.StringMatching(testCase.Spec.Output), "Output incorrect for {0}", result.Name);
            }
        }

        private IEnumerable<SpecTestCase> GetShouldHaveOutputData()
        {
            return Expectations.GetSpecTestCases()
                .Where(t => !string.IsNullOrWhiteSpace(t.Spec.Output))
                .Select(t => t.SetDescription("should have output"));
        }
    }
}
