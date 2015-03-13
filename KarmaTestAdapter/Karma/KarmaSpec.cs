using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.Karma
{
    public class KarmaSpec
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string UniqueName { get; set; }
        public IEnumerable<string> Suite { get; set; }
        public KarmaSource Source { get; set; }
        public IEnumerable<KarmaSpecResult> Results { get; set; }

        //public string ToString(string source)
        //{
        //    return JsonConvert.SerializeObject(this.CreateTestCase(source), Formatting.Indented);
        //}

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class KarmaSource
    {
        public string FileName { get; set; }
        public int? LineNumber { get; set; }
        public int? ColumnNumber { get; set; }
    }

    public class KarmaSpecResult
    {
        public string Browser { get; set; }
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public string Output { get; set; }
        public double? Time { get; set; }
        public IEnumerable<string> Log { get; set; }
        public IEnumerable<KarmaExpectation> Failures { get; set; }
    }

    public class KarmaExpectation
    {
        public string message { get; set; }
        public IEnumerable<string> stack { get; set; }
        public bool? passed { get; set; }
    }
}