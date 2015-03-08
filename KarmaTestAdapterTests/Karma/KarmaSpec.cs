using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Karma
{
    public abstract class KarmaSpecFixture : BaseFixture
    {
        public KarmaTestAdapter.Karma.KarmaSpec Spec { get; private set; }

        public override void Init()
        {
            base.Init();
            Spec = new KarmaTestAdapter.Karma.KarmaSpec
            {
                Id = "spec1",
                Description = "Spec 2",
                UniqueName = "Suite 1:Suite 2:Spec 2",
                Suite = new[] { "suite1", "suite2" },
                Source = new KarmaTestAdapter.Karma.KarmaSource
                {
                    FileName = Path.Combine(TestProjectsDir, @"JasmineTypescriptTests\specs\singleSpec.ts"),
                    LineNumber = 8,
                    ColumnNumber = 13
                },
                Results = new[] {
                    new KarmaTestAdapter.Karma.KarmaSpecResult
                    {
                        Browser = "PhantomJS 1.9.8 (Windows 8)",
                        Success = true,
                        Skipped = false,
                        Output = "",
                        Time = 2.25,
                        Log = new string[] {  },
                        Failures = new KarmaTestAdapter.Karma.KarmaExpectation[] {  }
                    },
                    new KarmaTestAdapter.Karma.KarmaSpecResult
                    {
                        Browser = "Chrome 40.0.2214 (Windows 8.1)",
                        Success = true,
                        Skipped = false,
                        Output = "",
                        Time = 1.0657500001798326,
                        Log = new string[] {  },
                        Failures = new KarmaTestAdapter.Karma.KarmaExpectation[] {  }
                    }
                }
            };
        }
    }

    public class KarmaSpec : KarmaSpecFixture
    {
    }
}
