using KarmaTestAdapter;
using KarmaTestAdapter.Logging;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests
{
    [TestFixture]
    public abstract class BaseFixture
    {
        public readonly IKarmaLogger Logger;

        public BaseFixture()
        {
            Logger = new TestKarmaLogger(message => Console.WriteLine("[{0:0.0}] {1}", Elapsed, message));
        }

        [SetUp]
        public virtual void Init()
        {
            _startTime = DateTime.Now;
            Globals.IsTest = true;
        }

        protected DateTime _startTime = DateTime.Now;
        protected double Elapsed { get { return (DateTime.Now - _startTime).TotalMilliseconds; } }

        protected string TestDir
        {
            get { return Path.GetFullPath(TestContext.CurrentContext.TestDirectory); }
        }

        protected string ProjectDir
        {
            get { return Path.GetFullPath(Path.Combine(TestDir, @"..\..")); }
        }

        protected string SolutionDir
        {
            get { return Path.GetDirectoryName(ProjectDir); }
        }

        protected string TestProjectsDir
        {
            get { return Path.Combine(SolutionDir, "TestProjects"); }
        }

        protected class TestPathConstraint : SamePathConstraint
        {
            public TestPathConstraint(string expected)
                : base(GetPath(expected))
            {
            }

            private static string GetPath(string relativePath)
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    return Path.GetFullPath(TestContext.CurrentContext.TestDirectory);
                }
                return Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, relativePath));
            }
        }

        protected static SamePathConstraint IsTestPath(params string[] expected)
        {
            return new SamePathConstraint(Path.Combine(expected));
        }
    }
}
