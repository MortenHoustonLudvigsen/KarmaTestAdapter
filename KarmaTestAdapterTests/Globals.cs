using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.GlobalsTests
{
    public class Globals : BaseFixture
    {
        public class IsTest : BaseFixture
        {
            [Test]
            public void ShouldBeTrue()
            {
                Assert.That(KarmaTestAdapter.Globals.IsTest, Is.True);
            }
        }

        public class AssemblyDirectory : BaseFixture
        {
            [Test]
            public void ShouldBeTestDirectory()
            {
                Assert.That(KarmaTestAdapter.Globals.AssemblyDirectory, IsTestPath(TestDir));
            }
        }

        public class RootDirectory : BaseFixture
        {
            [Test]
            public void ShouldBeKarmaDirectoryIfTest()
            {
                Assert.That(KarmaTestAdapter.Globals.RootDirectory, IsTestPath(SolutionDir, @"KarmaTestAdapter"));
            }

            [Test]
            public void ShouldBeTestDirectoryIfNotTest()
            {
                KarmaTestAdapter.Globals.IsTest = false;
                Assert.That(KarmaTestAdapter.Globals.RootDirectory, IsTestPath(TestDir));
            }
        }

        public class LibDirectory : BaseFixture
        {
            [Test]
            public void ShouldBeInKarmaDirectoryIfTest()
            {
                Assert.That(KarmaTestAdapter.Globals.LibDirectory, IsTestPath(SolutionDir, @"KarmaTestAdapter\KarmaTestServer"));
            }

            [Test]
            public void ShouldBeInTestDirectoryIfNotTest()
            {
                KarmaTestAdapter.Globals.IsTest = false;
                Assert.That(KarmaTestAdapter.Globals.LibDirectory, IsTestPath(TestDir, "KarmaTestServer"));
            }
        }
    }
}
