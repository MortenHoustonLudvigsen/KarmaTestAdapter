using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.FileTests
{
    public partial class TestResults
    {
        public class FileTestsHelper : Helper<KarmaTestAdapter.TestResults.File, KarmaTestAdapter.TestResults.Files>
        {
            public override KarmaTestAdapter.TestResults.Files CreateParent()
            {
                return CreateKarma().Files;
            }

            public override KarmaTestAdapter.TestResults.File CreateItem()
            {
                return CreateParent().Skip(2).First();
            }
        }

        public class EmptyFileTestsHelper : FileTestsHelper
        {
            public override KarmaTestAdapter.TestResults.File CreateItem()
            {
                return new KarmaTestAdapter.TestResults.File(CreateParent(), null);
            }
        }

        public partial class File : FileTestsHelper.Tests<FileTestsHelper>
        {
            public partial class Empty : FileTestsHelper.Tests<EmptyFileTestsHelper>
            {
            }
        }
    }
}
