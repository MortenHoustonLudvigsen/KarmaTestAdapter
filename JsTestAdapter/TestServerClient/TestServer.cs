using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using NpmProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TwoPS.Processes;

namespace JsTestAdapter.TestServerClient
{
    public abstract class TestServer
    {
        public TestServer(ITestLogger logger)
        {
            Logger = new TestLogger(logger, "Server");
            State = ServerState.None;
            Attempts = 0;
        }

        public abstract string Name { get; }
        public abstract string StartScript { get; }
        public abstract string WorkingDirectory { get; }

        public ITestLogger Logger { get; private set; }
        public string NodePath { get { return string.Join(";", GetNodePath(WorkingDirectory).Where(d => !string.IsNullOrWhiteSpace(d))); } }
        public ServerState State { get; private set; }
        public int Port { get; private set; }
        public int Attempts { get; set; }

        private TaskCompletionSource<int> _finishedSource;
        public Task<int> Finished { get { return _finishedSource.Task; } }

        private IEnumerable<string> GetNodePath(string directory)
        {
            foreach (var dir in GetNodeDirs(directory))
            {
                yield return dir;
            }
            yield return _npm.Root(global: true);
        }

        private IEnumerable<string> GetNodeDirs(string directory)
        {
            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
            {
                if (Directory.Exists(Path.Combine(directory, "node_modules")))
                {
                    yield return Path.Combine(directory, "node_modules");
                }
                foreach (var path in GetNodeDirs(Path.GetDirectoryName(directory)))
                {
                    yield return path;
                }
            }
        }

        private static Npm _npm = new Npm();

        public ProcessOptions GetProcessOptions()
        {
            if (!File.Exists(StartScript))
            {
                throw new Exception("Could not find start script for " + Name + " (" + StartScript + ")");
            }

            var options = new ProcessOptions("node")
            {
                WorkingDirectory = WorkingDirectory,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            options.EnvironmentVariables["NODE_PATH"] = NodePath;
            options.Add(StartScript);
            AddOptions(options);

            return options;
        }

        protected abstract void AddOptions(ProcessOptions options);

        public event Action<int> Started;
        private void OnStarted(int port)
        {
            if (Started != null)
            {
                Started(port);
            }
        }

        public event Action<int?, Exception> Stopped;
        private void OnStopped(int? exitCode, Exception ex)
        {
            if (Stopped != null)
            {
                Stopped(exitCode, ex);
            }
        }

        public event Action<string> OutputReceived;
        private void OnOutputReceived(string line)
        {
            if (OutputReceived != null)
            {
                OutputReceived(line);
            }
        }

        public event Action<string> ErrorReceived;
        private void OnErrorReceived(string line)
        {
            if (ErrorReceived != null)
            {
                ErrorReceived(line);
            }
        }

        private Process _process;
        public Task<int> StartServer(int timeout = Timeout.Infinite)
        {
            switch (State)
            {
                case ServerState.Starting:
                    throw new Exception("The server is already starting");
                case ServerState.Running:
                    throw new Exception("The server is already running");
            }

            Port = 0;
            State = ServerState.Starting;
            Attempts += 1;

            _finishedSource = new TaskCompletionSource<int>();

            var portSource = new TaskCompletionSource<int>().SetTimeout(timeout);

            _process = new Process(GetProcessOptions());
            _process.StandardOutputRead += (s, e) =>
            {
                if (Port == 0)
                {
                    var match = Regex.Match(e.Line, @"Started - port: (\d+)");
                    if (match.Success)
                    {
                        Port = int.Parse(match.Groups[1].Value);
                        State = ServerState.Running;
                        OnStarted(Port);
                        portSource.TrySetResult(Port);
                    }
                }
            };
            _process.StandardOutputRead += (s, e) => this.OnOutputReceived(e.Line);
            _process.StandardErrorRead += (s, e) => this.OnErrorReceived(e.Line);

            Logger.Debug("Starting {0}: {1}", Name, _process.Options.CommandLine);
            var runTask = Task.Run(() =>
            {
                try
                {
                    return _process.Run();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Server error");
                    throw;
                }
            });

            runTask.ContinueWith(t =>
            {
                State = ServerState.None;
                portSource.TrySetCanceled();
                _finishedSource.TrySetResult(t.Result.ExitCode);
                OnStopped(t.Result.ExitCode, t.Exception);
            });

            return portSource.Task;
        }

        public void Kill(string reason, bool warn)
        {
            if (_process != null)
            {
                if (warn)
                {
                    Logger.Warn("Killing server: {0}", reason);
                }
                else
                {
                    Logger.Debug("Killing server: {0}", reason);
                }
                _process.Cancel();
            }
        }
    }
}