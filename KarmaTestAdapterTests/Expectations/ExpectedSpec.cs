using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public class ExpectedSpec
    {
        public ExpectedSpec(JObject spec)
        {
            try
            {
                Description = (string)spec["Description"];
                UniqueName = (string)spec["UniqueName"];
                FileName = (string)spec["FileName"];
                var lineNumber = spec["LineNumber"];
                if (lineNumber is JArray)
                {
                    LineNumberFrom = (int?)lineNumber.FirstOrDefault();
                    LineNumberTo = (int?)lineNumber.Skip(1).FirstOrDefault() ?? LineNumberFrom;
                }
                else if (lineNumber is JValue)
                {
                    LineNumberFrom = (int)lineNumber;
                    LineNumberTo = (int)lineNumber;
                }
                if (LineNumberFrom > LineNumberTo)
                {
                    var tempLineNumber = LineNumberFrom;
                    LineNumberFrom = LineNumberTo;
                    LineNumberTo = tempLineNumber;
                }
                Success = (bool?)spec["Success"] ?? false;
                Skipped = (bool?)spec["Skipped"] ?? false;
                Output = (string)spec["Output"];
                IsValid = true;
            }
            catch (Exception ex)
            {
                IsValid = false;
                InvalidReason = ex.Message;
            }
        }

        public string Description { get; private set; }
        public string UniqueName { get; private set; }
        public string FileName { get; private set; }
        public int? LineNumberFrom { get; private set; }
        public int? LineNumberTo { get; private set; }
        public bool Success { get; private set; }
        public bool Skipped { get; private set; }
        public string Output { get; private set; }
        public bool IsValid { get; private set; }
        public string InvalidReason { get; private set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
