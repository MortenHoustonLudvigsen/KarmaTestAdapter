using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.PathUtils
{
    public partial class PathUtils
    {
        [TestFixture]
        public class ReadFileText : BaseFixture
        {
            public static string FixturesDirectory
            {
                get { return Path.Combine(ProjectDir, "PathUtils", "ReadFileTextFixtures"); }
            }

            public class TestCase : KarmaTestAdapterTests.TestCase<TestCase>
            {
                public TestCase(Encoding encoding, bool hasByteOrderMark)
                {
                    Encoding = encoding;
                    HasByteOrderMark = hasByteOrderMark;
                }

                public Encoding Encoding { get; private set; }
                public bool HasByteOrderMark { get; private set; }
                public string Name { get { return string.Format("{0}{1}", Encoding.EncodingName, HasByteOrderMark ? " with BOM" : ""); } }
                public string FileName { get { return string.Format("{0}{1}.json", Encoding.BodyName, HasByteOrderMark ? "-bom" : ""); } }

                public override string TestName
                {
                    get
                    {
                        return Description.Replace('.', '-');
                    }
                }
            }

            private IEnumerable<TestCase> GetTestCases()
            {
                yield return new TestCase(new UTF8Encoding(false), false);
                yield return new TestCase(new UTF8Encoding(true), true);
                yield return new TestCase(new UnicodeEncoding(false, true), true);
                yield return new TestCase(new UnicodeEncoding(true, true), true);
            }

            private const string Text = @"{ ""Some Danish letters for good measure"": ""æøåÆØÅ"" }";

            [Test, TestCaseSource("GetShouldReadEncodedFileFixtures")]
            public void ShouldReadEncodedFile(TestCase testCase)
            {
                Directory.CreateDirectory(FixturesDirectory);
                var filePath = Path.Combine(FixturesDirectory, testCase.FileName);
                using (var writer = new StreamWriter(filePath, false, testCase.Encoding))
                {
                    writer.Write(Text);
                }
                var actual = JsTestAdapter.Helpers.PathUtils.ReadFileText(filePath);
                Assert.That(actual, Is.EqualTo(Text));
            }

            private IEnumerable<TestCase> GetShouldReadEncodedFileFixtures()
            {
                return GetTestCases()
                    .Select(f => f.SetDescription("should read file with encoding {0}", f.Name));
            }

        }
    }
}
