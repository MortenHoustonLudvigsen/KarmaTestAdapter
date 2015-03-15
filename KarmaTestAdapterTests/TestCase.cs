using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests
{
    public abstract class TestCase<TTestCase> : ITestCaseData
        where TTestCase : TestCase<TTestCase>
    {
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

        public string Description { get; private set; }

        public abstract string TestName { get; }

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
}
