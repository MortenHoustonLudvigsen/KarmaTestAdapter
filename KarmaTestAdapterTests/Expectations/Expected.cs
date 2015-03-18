using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using JsTestAdapter.TestServerClient;
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
        public Expected(string name, string file, string baseDirectory)
        {
            BaseDirectory = baseDirectory;
            Logger = new TestKarmaLogger(message => Console.WriteLine(message));
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
            }
            catch (AggregateException ex)
            {
                var innerException = ex.InnerExceptions.FirstOrDefault();
                if (innerException != null)
                {
                    _validator.Validate(false, string.Format("{0}\n{1}", innerException.Message, innerException.StackTrace));
                    Logger.Error(innerException);
                }
                else
                {
                    _validator.Validate(false, string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
                    Logger.Error(ex);
                }
            }
            catch (Exception ex)
            {
                _validator.Validate(false, string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
                Logger.Error(ex);
            }
        }

        public ITestLogger Logger { get; private set; }
        public string KarmaConfig { get; set; }
        public IEnumerable<ExpectedSpec> Specs { get; set; }
        public string BaseDirectory { get; private set; }
        public string Directory { get; set; }
        public string Name { get; set; }
        public bool IsValid { get { return _validator.IsValid; } }
        public string InvalidReason { get { return _validator.InvalidReason; } }

        private IEnumerable<Spec> _karmaSpecs;
        private List<string> _karmaOutput = new List<string>();

        public async Task<IEnumerable<string>> GetKarmaOutput()
        {
            await PopulateKarmaSpecs();
            return _karmaOutput;
        }

        public async Task<IEnumerable<Spec>> GetKarmaSpecs()
        {
            await PopulateKarmaSpecs();
            return _karmaSpecs;
        }

        public async Task<Spec> GetKarmaSpec(string uniqueName)
        {
            var karmaSpecs = await GetKarmaSpecs();
            return karmaSpecs.FirstOrDefault(s => s.UniqueName == uniqueName);
        }

        public async Task<IEnumerable<Spec>> GetUnexpectedKarmaSpecs()
        {
            var karmaSpecs = await GetKarmaSpecs();
            return karmaSpecs.Where(k => !Specs.Any(s => s.UniqueName == k.UniqueName)).ToList();
        }

        private Validator _validator = new Validator();

        public async Task PopulateKarmaSpecs()
        {
            if (_karmaSpecs == null)
            {
                Globals.IsTest = true;
                var karmaSpecs = new List<Spec>();
                var settings = new KarmaSettings(KarmaConfig, f => File.Exists(f), BaseDirectory, Logger);
                if (settings.AreValid)
                {
                    var server = new KarmaServer(settings, Logger);
                    server.OutputReceived += line => _karmaOutput.Add(line);
                    server.ErrorReceived += line => _karmaOutput.Add(line);
                    var port = await server.StartServer(60000);
                    var stopCommand = new StopCommand(port);
                    var discoverCommand = new DiscoverCommand(port);
                    await discoverCommand.Run(spec => karmaSpecs.Add(spec));
                    await stopCommand.Run();
                    await server.Finished;
                    _karmaSpecs = karmaSpecs;
                    Logger.Info("{0} specs discovered", _karmaSpecs.Count());
                }
                else
                {
                    _validator.Validate(false, settings.InvalidReason);
                }
            }
        }

        public ProjectTestCase GetProjectTestCase()
        {
            return new ProjectTestCase(this);
        }

        public IEnumerable<SpecTestCase> GetSpecTestCases()
        {
            return Specs.Select(spec => new SpecTestCase(this, spec)).ToList();
        }
    }
}
