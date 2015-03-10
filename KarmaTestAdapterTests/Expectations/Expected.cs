using KarmaTestAdapter;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
using KarmaTestAdapter.TestAdapter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public class Expected
    {
        public Expected(string name, string file)
        {
            KarmaOutput = new List<string>();
            Logger = new TestKarmaLogger((level, phase, message) =>
            {
                var levelText = level.LevelText();
                Console.WriteLine(string.Join(" ", new[]{
                    string.IsNullOrWhiteSpace(levelText) ? null : string.Format("[{0}]", levelText),
                    string.IsNullOrWhiteSpace(phase) ? null : string.Format("[{0}]", phase),
                    message
                }.Where(s => !string.IsNullOrWhiteSpace(s))));
            });
            Name = name ?? "";
            try
            {
                Directory = Path.GetDirectoryName(file);
                var expected = Json.ParseFile(file);
                KarmaConfig = (string)expected["KarmaConfig"];
                if (string.IsNullOrWhiteSpace(KarmaConfig))
                {
                    throw new ArgumentNullException("KarmaConfig", "KarmaConfig is null or empty");
                }
                KarmaConfig = Path.Combine(Directory, KarmaConfig);
                if (!File.Exists(KarmaConfig))
                {
                    throw new FileNotFoundException("KarmaConfig file not found", KarmaConfig);
                }

                if (expected["Specs"] != null)
                {
                    if (!(expected["Specs"] is JArray))
                    {
                        throw new ArgumentException("Specs is not an array", "Specs");
                    }
                    Specs = expected["Specs"].OfType<JObject>().Select(s => new ExpectedSpec(s, Directory)).ToList();
                }
                else
                {
                    Specs = Enumerable.Empty<ExpectedSpec>();
                }
                Globals.IsTest = true;
                //var buildDoneFile = Path.Combine(Globals.RootDirectory, "build-done");
                //var tries = 0;
                //do
                //{
                //    Task.Delay(100).Wait();
                //    tries += 1;
                //}
                //while (tries < 300 && !File.Exists(buildDoneFile));
                //if (!File.Exists(buildDoneFile))
                //{
                //    throw new Exception(string.Format("Could not find {0}", buildDoneFile));
                //}
                PopulateKarmaSpecs().Wait();
                IsValid = true;
            }
            catch (Exception ex)
            {
                IsValid = false;
                Logger.Error(ex);
            }
        }

        public IKarmaLogger Logger { get; private set; }
        public string KarmaConfig { get; set; }
        public IEnumerable<ExpectedSpec> Specs { get; set; }
        public string Directory { get; set; }
        public string Name { get; set; }
        public IEnumerable<KarmaSpec> KarmaSpecs { get; private set; }
        public bool IsValid { get; private set; }
        public List<string> KarmaOutput { get; private set; }

        public async Task PopulateKarmaSpecs()
        {
            var karmaSpecs = new List<KarmaSpec>();
            var settings = new KarmaSettings(KarmaConfig, Logger);
            var server = new KarmaServer(settings, Logger);
            server.OutputReceived += line => Logger.Info("[OUT] {0}", line);
            server.ErrorReceived += line => Logger.Info("[ERR] {0}", line);
            var port = await server.StartServer(60000);
            var stopCommand = new KarmaStopCommand(port);
            var discoverCommand = new KarmaDiscoverCommand(port);
            await discoverCommand.Run(spec => karmaSpecs.Add(spec));
            await stopCommand.Run();
            await server.Finished;
            KarmaSpecs = karmaSpecs;
            Logger.Info("{0} specs discovered", KarmaSpecs.Count());
        }

        public ProjectTestCase GetProjectTestCase()
        {
            return new ProjectTestCase(this);
        }

        public IEnumerable<SpecTestCase> GetSpecTestCases()
        {
            if (!IsValid)
            {
                return Enumerable.Empty<SpecTestCase>();
            }

            var uniqueNames = Enumerable.Union(
                Specs.Select(spec => spec.UniqueName),
                KarmaSpecs.Select(spec => spec.UniqueName)
            );

            var q = from uniqueName in uniqueNames
                    join spec in Specs on uniqueName equals spec.UniqueName into jSpec
                    from spec in jSpec.DefaultIfEmpty()
                    join karmaSpec in KarmaSpecs on uniqueName equals karmaSpec.UniqueName into jKarmaSpec
                    from karmaSpec in jKarmaSpec.DefaultIfEmpty()
                    select new SpecTestCase(this, uniqueName, spec, karmaSpec);

            return q.ToList();
        }

        public IEnumerable<SpecResultTestCase> GetSpecResultTestCases()
        {
            return GetSpecTestCases()
                .Where(t => t.IsValid && t.Spec != null && t.KarmaSpec != null)
                .SelectMany(t => t.KarmaSpec.Results.Select(r => new SpecResultTestCase(t.Expected, t.UniqueName, t.Spec, t.KarmaSpec, r)))
                .ToList();
        }
    }
}
