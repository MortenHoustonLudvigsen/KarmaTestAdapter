using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public class ExpectedSpec
    {
        public ExpectedSpec(JObject spec, string baseDirectory)
        {
            try
            {
                BaseDirectory = baseDirectory;
                Description = (string)spec["Description"];
                UniqueName = (string)spec["UniqueName"];
                FileName = FullPath((string)spec["FileName"]);
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
                StackTrace = FormatStackTrace(spec["StackTrace"] as JArray);
                IsValid = true;
            }
            catch (Exception ex)
            {
                IsValid = false;
                InvalidReason = ex.Message;
            }
        }

        public string BaseDirectory { get; private set; }
        public string Description { get; private set; }
        public string UniqueName { get; private set; }
        public string FileName { get; private set; }
        public int? LineNumberFrom { get; private set; }
        public int? LineNumberTo { get; private set; }
        public bool Success { get; private set; }
        public bool Skipped { get; private set; }
        public string Output { get; private set; }
        public string StackTrace { get; private set; }
        public bool IsValid { get; private set; }
        public string InvalidReason { get; private set; }

        private string FormatStackTrace(JArray stackTrace)
        {
            if (stackTrace != null)
            {
                return string.Join(Environment.NewLine, stackTrace.Select(f => FormatStackFrame(f as JObject)));
            }
            return null;
        }

        private string FormatStackFrame(JObject frame)
        {
            if (frame != null)
            {
                var line = string.Format("    at {0} in {1}", frame["FunctionName"], FullPath((string)frame["FileName"]));
                var lineNumber = frame["LineNumber"];
                if (lineNumber is JValue)
                {
                    line += string.Format(":line {0}", lineNumber);
                }
                return line;
            }
            return "";
        }

        private string FullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            return Path.Combine(BaseDirectory, path);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
