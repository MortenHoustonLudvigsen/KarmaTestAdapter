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
    [AttributeUsage(AttributeTargets.Class)]
    public class ExpectationsFixtureAttribute : TestActionAttribute
    {
        public override void BeforeTest(TestDetails details)
        {
        }

        public override void AfterTest(TestDetails details)
        {
            if (details.IsSuite)
            {
                ExpectationsTests.Reset();
            }
        }

        public override ActionTargets Targets
        {
            get { return ActionTargets.Suite; }
        }
    }

    [ExpectationsFixture]
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

        [Test, TestCaseSource("GetProjectShouldBeValidData")]
        public void ProjectShouldBeValid(ProjectTestCase testCase)
        {
            Assert.That(testCase.IsValid, testCase.InvalidReason);
            Logger.Info(testCase.Output);
        }

        private IEnumerable<ProjectTestCase> GetProjectShouldBeValidData()
        {
            return Expectations.GetProjectTestCases()
                .Select(t => t.SetDescription("should be valid"));
        }

        [Test, TestCaseSource("GetSpecShouldBeValidData")]
        public void SpecShouldBeValid(SpecTestCase testCase)
        {
            Assert.That(testCase.IsValid, testCase.Output);
        }

        private IEnumerable<SpecTestCase> GetSpecShouldBeValidData()
        {
            return Expectations.GetSpecTestCases()
                .Select(t => t.SetDescription("should be valid"));
        }

        [Test, TestCaseSource("GetShouldHaveSpecData")]
        public void ShouldHaveSpec(SpecTestCase testCase)
        {
            Assert.That(testCase.Spec, Is.Not.Null);
        }

        private IEnumerable<SpecTestCase> GetValidTestCases(bool expectSpec = true, bool expectKarmaSpec = true)
        {
            var testCases = Expectations.GetSpecTestCases().Where(t => t.IsValid);
            if (expectSpec)
            {
                testCases = testCases.Where(t => t.Spec != null);
            }
            if (expectKarmaSpec)
            {
                testCases = testCases.Where(t => t.KarmaSpec != null);
            }
            return testCases;
        }

        private IEnumerable<SpecTestCase> GetShouldHaveSpecData()
        {
            return GetValidTestCases(expectSpec: false)
                .Select(t => t.SetDescription("should have spec"));
        }

        [Test, TestCaseSource("GetShouldHaveKarmaSpecData")]
        public void ShouldHaveKarmaSpec(SpecTestCase testCase)
        {
            Assert.That(testCase.KarmaSpec, Is.Not.Null, "Karma spec missing");
        }

        private IEnumerable<SpecTestCase> GetShouldHaveKarmaSpecData()
        {
            return GetValidTestCases(expectKarmaSpec: false)
                .Select(t => t.SetDescription("should have karma spec"));
        }

        [Test, TestCaseSource("GetShouldHaveResultsData")]
        public void ShouldHaveResults(SpecTestCase testCase)
        {
            Assert.That(testCase.KarmaSpec.Results.Any(), Is.True);
        }

        private IEnumerable<SpecTestCase> GetShouldHaveResultsData()
        {
            return GetValidTestCases()
                .Select(t => t.SetDescription("should have results"));
        }

        [Test, TestCaseSource("GetShouldHaveFileNameData")]
        public void ShouldHaveFileName(SpecTestCase testCase)
        {
            Assert.That(testCase.KarmaSpec.Source, Is.Not.Null, "Source missing");
            Assert.That(testCase.KarmaSpec.Source.FileName, Is.SamePath(testCase.Spec.FileName));
        }

        private IEnumerable<SpecTestCase> GetShouldHaveFileNameData()
        {
            return GetValidTestCases()
                .Where(t => !string.IsNullOrWhiteSpace(t.Spec.FileName))
                .Select(t => t.SetDescription("should have file name '{0}'", t.Spec.FileName));
        }

        [Test, TestCaseSource("GetShouldHaveLineNumberData")]
        public void ShouldHaveLineNumber(SpecTestCase testCase)
        {
            Assert.That(testCase.KarmaSpec.Source, Is.Not.Null, "Source missing");
            Assert.That(testCase.KarmaSpec.Source.LineNumber, new BetweenConstraint(testCase.Spec.LineNumberFrom, testCase.Spec.LineNumberTo), "Line number");
        }

        private IEnumerable<SpecTestCase> GetShouldHaveLineNumberData()
        {
            return GetValidTestCases()
                .Where(t => t.Spec.LineNumberFrom.HasValue && t.Spec.LineNumberTo.HasValue)
                .Select(t => t.SetDescription("should have line number between {0} and {1}", t.Spec.LineNumberFrom, t.Spec.LineNumberTo));
        }

        [Test, TestCaseSource("GetShouldFailData")]
        public void ShouldFail(SpecResultTestCase testCase)
        {
            Assert.That(testCase.KarmaResult.Success, Is.False);
        }

        private IEnumerable<SpecResultTestCase> GetShouldFailData()
        {
            return Expectations.GetSpecResultTestCases()
                .Where(t => !t.Spec.Success)
                .Select(t => t.SetDescription("should fail"));
        }

        [Test, TestCaseSource("GetShouldHaveStackTraceData")]
        public void ShouldHaveStackTrace(SpecResultTestCase testCase)
        {
            string actualStack = null;
            if (testCase.KarmaResult.Failures != null && testCase.KarmaResult.Failures.Any())
            {
                actualStack = string.Join(Environment.NewLine, testCase.KarmaResult.Failures.First().stack.Take(testCase.Spec.Stack.Count()));
            }
            Assert.That(actualStack, Is.EqualTo(testCase.Spec.StackTrace));
        }

        private IEnumerable<SpecResultTestCase> GetShouldHaveStackTraceData()
        {
            return Expectations.GetSpecResultTestCases()
                .Where(t => !t.Spec.Success)
                .Select(t => t.SetDescription("should have stack trace"));
        }

        [Test, TestCaseSource("GetShouldSucceedData")]
        public void ShouldSucceed(SpecResultTestCase testCase)
        {
            Assert.That(testCase.KarmaResult.Success, Is.True);
        }

        private IEnumerable<SpecResultTestCase> GetShouldSucceedData()
        {
            return Expectations.GetSpecResultTestCases()
                .Where(t => t.Spec.Success)
                .Select(t => t.SetDescription("should succeed"));
        }

        [Test, TestCaseSource("GetShouldHaveOutputData")]
        public void ShouldHaveOutput(SpecResultTestCase testCase)
        {
            Assert.That(testCase.KarmaResult.Output, Is.StringMatching(testCase.Spec.Output));
        }

        private IEnumerable<SpecResultTestCase> GetShouldHaveOutputData()
        {
            return Expectations.GetSpecResultTestCases()
                .Where(t => !string.IsNullOrWhiteSpace(t.Spec.Output))
                .Select(t => t.SetDescription("should have output"));
        }
    }
}
