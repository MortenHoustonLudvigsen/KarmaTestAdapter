using KarmaTestAdapter;
using KarmaTestAdapter.Logging;
using KarmaTestAdapter.Helpers;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwoPS.Processes;

namespace KarmaTestAdapterTests.Karma.KarmaServerTests
{
    public abstract class KarmaServerFixture : BaseFixture
    {
        public KarmaTestAdapter.Karma.KarmaServer Server { get; private set; }

        public override void Init()
        {
            base.Init();
            var karmaConfFile = Path.Combine(SolutionDir, @"TestProjects\JasmineTypescriptTests\karma.conf.js");
            var settings = new KarmaTestAdapter.TestAdapter.KarmaSettings(karmaConfFile, f => File.Exists(f), TestProjectsDir, Logger);
            Server = new KarmaTestAdapter.Karma.KarmaServer(settings, Logger);
            _startTime = DateTime.Now;
            Server.OutputReceived += line => Logger.Info("[Karma] {0}", line);
            Server.ErrorReceived += line => Logger.Info("[Karma] {0}", line);
        }
    }

    public class KarmaServer : KarmaServerFixture
    {
        [Test]
        public async void CanStartServer()
        {
            var port = await Server.StartServer();
            Logger.Info("[Test] Port: {0}", port);
            Server.Finished.Wait(500);
            Server.Kill("Stop", false);
            var exitCode = await Server.Finished;
            Logger.Info("[Test] Exit code: {0}", exitCode);
        }

        [Test]
        public async void CanGetPort()
        {
            var port = await Server.StartServer(60000);
            Logger.Info("[Test] Port: {0}", port);
            Logger.Info("[Test] Server.Port: {0}", Server.Port);
            Server.Finished.Wait(500);
            Server.Kill("Stop", false);
            var exitCode = await Server.Finished;
            Logger.Info("[Test] Exit code: {0}", exitCode);
            Assert.That(port, Is.GreaterThan(0));
            Assert.That(Server.Port, Is.EqualTo(port));
        }

        [Test]
        public async void CanGetEvents()
        {
            var port = await Server.StartServer(60000);
            var eventsCommand = new KarmaTestAdapter.Karma.KarmaEventCommand(port);
            var events = eventsCommand.Run(evt => Logger.Info("Event: {0}", evt));
            Server.Finished.Wait(500);
            Server.Kill("Stop", false);
            await events;
            var exitCode = await Server.Finished;
            Logger.Info("[Test] Exit code: {0}", exitCode);
        }

        [Test]
        public async void CanStopServer()
        {
            var port = await Server.StartServer(60000);
            var stopCommand = new KarmaTestAdapter.Karma.KarmaStopCommand(port);
            await stopCommand.Run();
            var exitCode = await Server.Finished;
            Logger.Info("[Test] Exit code: {0}", exitCode);
        }

        [Test]
        public async void KarmaGetsDifferentPort()
        {
            var karmaPortTCS = new TaskCompletionSource<int>().SetTimeout(3000);
            Server.OutputReceived += line =>
            {
                var match = Regex.Match(line, @"\[karma\]:\s+Karma\s+.*\s+server started at http://[^:]+:(\d+)");
                if (match.Success)
                {
                    karmaPortTCS.TrySetResult(int.Parse(match.Groups[1].Value));
                }
            };
            var port = await Server.StartServer(60000);
            var karmaPort = await karmaPortTCS.Task;

            var stopCommand = new KarmaTestAdapter.Karma.KarmaStopCommand(port);
            await stopCommand.Run();
            var exitCode = await Server.Finished;
            Logger.Info("[Test] Exit code: {0}", exitCode);

            Assert.That(karmaPort, Is.GreaterThan(0));
            Assert.That(karmaPort, Is.Not.EqualTo(port));
        }

        [Test]
        public async void CanDiscover()
        {
            var port = await Server.StartServer(60000);
            var discoverCommand = new KarmaTestAdapter.Karma.KarmaDiscoverCommand(port);
            //discoverCommand.MessageReceived += message => Logger.Info("[Discover] [Message] {0}", message);
            discoverCommand.Connected += () => Logger.Info("[Discover] connected");
            discoverCommand.Disconnected += () => Logger.Info("[Discover] disconnected");
            var discover = discoverCommand.Run(spec => Logger.Info("[Spec] {0}", spec));
            await discover;
            //discover.Wait(10000);
            Server.Kill("Stop", false);
            var exitCode = await Server.Finished;
            Logger.Info("[Test] Exit code: {0}", exitCode);
        }
    }
}
