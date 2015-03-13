using KarmaTestAdapter;
using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public class Expectations
    {
        public Expectations(string directory)
        {
            BaseDirectory = directory;
            Expected = GetExpected(directory).ToList();
        }

        public string BaseDirectory { get; private set; }
        public IEnumerable<Expected> Expected { get; set; }

        public IEnumerable<ProjectTestCase> GetProjectTestCases()
        {
            var testCases = Expected
                .Select(e => e.GetProjectTestCase())
                .ToList();
            return testCases;
        }

        public IEnumerable<SpecTestCase> GetSpecTestCases()
        {
            var testCases = Expected
                .SelectMany(e => e.GetSpecTestCases())
                .ToList();
            return testCases;
        }

        public IEnumerable<SpecResultTestCase> GetSpecResultTestCases()
        {
            var testCases = Expected
                .SelectMany(e => e.GetSpecResultTestCases())
                .ToList();
            return testCases;
        }

        private IEnumerable<Expected> GetExpected(string directory, string name = null)
        {
            if (Path.GetFileName(directory) == "node_modules")
            {
                yield break;
            }

            var file = Path.Combine(directory, "Expected.json");
            if (File.Exists(file))
            {
                yield return new Expected(name, file, BaseDirectory);
            }

            foreach (var childDirectory in Directory.GetDirectories(directory))
            {
                var childName = Path.GetFileName(childDirectory);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    childName = name + "/" + childName;
                }
                foreach (var expected in GetExpected(childDirectory, childName))
                {
                    yield return expected;
                }
            }
        }
    }
}
