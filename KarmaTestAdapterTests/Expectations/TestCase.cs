using KarmaTestAdapter.Karma;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public abstract class TestCase<TTestCase> : ITestCaseData
        where TTestCase : TestCase<TTestCase>
    {
        public abstract bool IsValid { get; }
        public abstract string Output { get; }
        public object[] Arguments { get { return new object[] { this }; } }

        private List<string> _categories = new List<string>();
        public IEnumerable<string> Categories { get { return _categories; } }
        public Type ExpectedException { get { return null; } }
        public string ExpectedExceptionName { get { return null; } }
        public bool Explicit { get { return false; } }
        public bool HasExpectedResult { get { return false; } }
        public string IgnoreReason { get { return null; } }
        public bool Ignored { get { return false; } }
        public object Result { get { return null; } }

        public abstract string ProjectName { get; }
        public string Description { get; private set; }

        public string TestName
        {
            get { return string.Format("{0}.{1}", ProjectName, Description.Replace('.', '-')); }
        }

        public void AddCategory(string category)
        {
            _categories.Add(category);
        }

        public virtual TTestCase SetDescription(string format, params object[] args)
        {
            Description = string.Format(format, args);
            return (TTestCase)this;
        }

        public override string ToString()
        {
            return Description;
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
        public override string Output { get { return string.Join(Environment.NewLine, Expected.KarmaOutput); } }
        public override string ProjectName { get { return Expected.Name; } }

        public override ProjectTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("Expected {0}", string.Format(format, args));
        }
    }

    public class SpecTestCase : TestCase<SpecTestCase>
    {
        public SpecTestCase(Expected expected, string uniqueName, ExpectedSpec spec, KarmaSpec karmaSpec)
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
        public KarmaSpec KarmaSpec { get; private set; }

        public override bool IsValid
        {
            get { return Spec != null && Spec.IsValid || Spec == null && KarmaSpec != null; }
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
        public SpecResultTestCase(Expected expected, string uniqueName, ExpectedSpec spec, KarmaSpec karmaSpec, KarmaSpecResult karmaResult)
        {
            Expected = expected;
            UniqueName = uniqueName;
            Spec = spec;
            KarmaSpec = karmaSpec;
            KarmaResult = karmaResult;
            AddCategory(Expected.Name);
            AddCategory(string.Format("Browser: {0}", KarmaResult.Browser));
            AddCategory(string.Format("{0} ({1})", Expected.Name, KarmaResult.Browser));
        }

        public Expected Expected { get; private set; }
        public string UniqueName { get; private set; }
        public ExpectedSpec Spec { get; private set; }
        public KarmaSpec KarmaSpec { get; private set; }
        public KarmaSpecResult KarmaResult { get; private set; }

        public override bool IsValid
        {
            get { return Spec != null && Spec.IsValid || Spec == null && KarmaSpec != null; }
        }

        public override string Output
        {
            get { return Spec != null ? Spec.InvalidReason : ""; }
        }

        public override string ProjectName { get { return string.Format("{0} ({1})", Expected.Name, KarmaResult.Browser.Replace('.', '-')); } }

        public override SpecResultTestCase SetDescription(string format, params object[] args)
        {
            return base.SetDescription("[{0}] {1}", UniqueName, string.Format(format, args));
        }
    }
}
