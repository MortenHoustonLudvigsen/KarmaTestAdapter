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
        public abstract string Output { get; }
        public abstract string ProjectName { get; }

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
            AddCategory(Expected.Name);
        }

        public Expected Expected { get; private set; }
        public override bool IsValid { get { return Expected.IsValid; } }
        public override string InvalidReason { get { return Expected.InvalidReason; } }
        public override string Output { get { return string.Join(Environment.NewLine, Expected.KarmaOutput); } }
        public override string ProjectName { get { return Expected.Name; } }

        public override ProjectTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("Expected {0}", string.Format(format, args));
        }
    }

    public class SpecTestCase : TestCase<SpecTestCase>
    {
        public SpecTestCase(Expected expected, string uniqueName, ExpectedSpec spec, Spec karmaSpec)
        {
            Expected = expected;
            UniqueName = uniqueName;
            Spec = spec;
            KarmaSpec = karmaSpec;
            AddCategory(Expected.Name);
        }

        public Expected Expected { get; private set; }
        public string UniqueName { get; private set; }
        public ExpectedSpec Spec { get; private set; }
        public Spec KarmaSpec { get; private set; }

        public override bool IsValid
        {
            get { return Spec != null && Spec.IsValid || Spec == null && KarmaSpec != null; }
        }

        public override string InvalidReason
        {
            get { return Spec != null ? Spec.InvalidReason : ""; }
        }

        public override string Output
        {
            get { return Spec != null ? Spec.InvalidReason : ""; }
        }

        public override string ProjectName { get { return Expected.Name; } }

        public override SpecTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("[{0}] {1}", UniqueName.Replace('.', '-'), string.Format(format, args));
        }
    }

    public class SpecResultTestCase : TestCase<SpecResultTestCase>
    {
        public SpecResultTestCase(Expected expected, string uniqueName, ExpectedSpec spec, Spec karmaSpec, SpecResult karmaResult)
        {
            Expected = expected;
            UniqueName = uniqueName;
            Spec = spec;
            KarmaSpec = karmaSpec;
            KarmaResult = karmaResult;
            AddCategory(Expected.Name);
            AddCategory(string.Format("Browser: {0}", KarmaResult.Name));
            AddCategory(string.Format("{0} ({1})", Expected.Name, KarmaResult.Name));
        }

        public Expected Expected { get; private set; }
        public string UniqueName { get; private set; }
        public ExpectedSpec Spec { get; private set; }
        public Spec KarmaSpec { get; private set; }
        public SpecResult KarmaResult { get; private set; }

        public override bool IsValid
        {
            get { return Spec != null && Spec.IsValid || Spec == null && KarmaSpec != null; }
        }

        public override string InvalidReason
        {
            get { return Spec != null ? Spec.InvalidReason:""; }
        }

        public override string Output
        {
            get { return Spec != null ? Spec.InvalidReason : ""; }
        }

        public override string ProjectName { get { return string.Format("{0} ({1})", Expected.Name, KarmaResult.Name.Replace('.', '-')); } }

        public override SpecResultTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("[{0}] {1}", UniqueName, string.Format(format, args));
        }
    }
}
