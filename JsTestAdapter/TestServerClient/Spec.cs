using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsTestAdapter.TestServerClient
{
    public class Spec
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string UniqueName { get; set; }
        public IEnumerable<string> Suite { get; set; }
        public Source Source { get; set; }
        public IEnumerable<SpecResult> Results { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class Source
    {
        public string FileName { get; set; }
        public int? LineNumber { get; set; }
        public int? ColumnNumber { get; set; }
    }

    public class SpecResult
    {
        public string Name { get; set; }
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public string Output { get; set; }
        public double? Time { get; set; }
        public IEnumerable<string> Log { get; set; }
        public IEnumerable<Expectation> Failures { get; set; }
    }

    public class Expectation
    {
        public string message { get; set; }
        public IEnumerable<string> stack { get; set; }
        public bool? passed { get; set; }
    }
}