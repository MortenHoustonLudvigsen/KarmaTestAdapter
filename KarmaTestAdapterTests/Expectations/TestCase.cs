using JsTestAdapter.TestServerClient;
using KarmaTestAdapter.Karma;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public abstract class TestCase<TTestCase> : KarmaTestAdapterTests.TestCase<TTestCase>
        where TTestCase : TestCase<TTestCase>
    {
        public abstract bool IsValid { get; }
        public abstract string InvalidReason { get; }
        public abstract string ProjectName { get; }

        public virtual async Task<string> GetOutput()
        {
            return await Task.FromResult<string>(null);
        }

        public override string TestName
        {
            get { return string.Format("{0}.{1}", ProjectName, Description.Replace('.', '-')); }
        }
    }

    public class ProjectTestCase : TestCase<ProjectTestCase>
    {
        public ProjectTestCase(Expected expected)
        {
            Expected = expected;
            AddCategory(Expected.ProjectName);
        }

        public Expected Expected { get; private set; }
        public override bool IsValid { get { return Expected.IsValid; } }
        public override string InvalidReason { get { return Expected.InvalidReason; } }
        public override string ProjectName { get { return Expected.ProjectName; } }

        public override async Task<string> GetOutput()
        {
            return string.Join(Environment.NewLine, await Expected.GetKarmaOutput());
        }

        public override ProjectTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("Expected {0}", string.Format(format, args));
        }
    }

    public class SpecTestCase : TestCase<SpecTestCase>
    {
        public SpecTestCase(Expected expected, ExpectedSpec spec)
        {
            Expected = expected;
            Spec = spec;
            AddCategory(Expected.ProjectName);
        }

        public Expected Expected { get; private set; }
        public ExpectedSpec Spec { get; private set; }

        public async Task<Spec> GetKarmaSpec()
        {
            return await Expected.GetKarmaSpec(Spec.FullyQualifiedName);
        }

        public override bool IsValid
        {
            get { return Spec.IsValid; }
        }

        public override string InvalidReason
        {
            get { return Spec.InvalidReason; }
        }

        public override async Task<string> GetOutput()
        {
            return await Task.FromResult(Spec.InvalidReason);
        }

        public override string ProjectName { get { return Expected.ProjectName; } }

        public override SpecTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("[{0}] {1}", Spec.FullyQualifiedName.Replace('.', '-'), string.Format(format, args));
        }
    }
}
