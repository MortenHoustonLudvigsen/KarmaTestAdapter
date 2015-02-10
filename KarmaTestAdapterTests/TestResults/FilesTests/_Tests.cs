using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.FilesTests
{
    public partial class TestResults
    {
        public class FilesTestsHelper : Helper<KarmaTestAdapter.TestResults.Files>
        {
            public override KarmaTestAdapter.TestResults.Files CreateItem()
            {
                return CreateKarma().Files;
            }
        }

        public class EmptyFilesTestsHelper : FilesTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Files CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Files(CreateKarma(), null);
            }
        }

        public partial class Files : FilesTestsHelper.Tests<FilesTestsHelper>
        {
            public partial class Empty : FilesTestsHelper.Tests<EmptyFilesTestsHelper>
            {
            }
        }
    }
}
