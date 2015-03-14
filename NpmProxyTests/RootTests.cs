using NpmProxy;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpmProxyTests
{
    [TestFixture]
    public class RootTests
    {
        private Npm _npm = new Npm();

        [Test]
        public void NpmRootGlobalShouldNotBeNull()
        {
            var root = _npm.Root(true);
            Assert.That(root, Is.Not.Null);
        }

        [Test]
        public void NpmRootGlobalShouldBeCorrect()
        {
            var root = _npm.Root(true);
            Assert.That(root, Is.SamePath(Path.Combine(Environment.GetEnvironmentVariable("AppData"), "npm", "node_modules")));
        }
    }
}
