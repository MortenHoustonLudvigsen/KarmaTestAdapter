using KarmaTestAdapter.TestResults;
using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TwoPS.Processes;
using IO = System.IO;

namespace KarmaTestAdapter.Commands
{
    public class KarmaServeCommand : KarmaCommand, IDisposable
    {
        public KarmaServeCommand(string source)
            : base("serve", source)
        {
        }

        public override string Name { get { return "Server"; } }

        public void Start(IKarmaLogger logger, Action done)
        {
            var commandLogger = new KarmaCommandLogger(logger, this);
            using (var settings = new KarmaSettings(Source, commandLogger))
            {
                if (settings.AreValid)
                {
                    var outputDirectory = settings.GetOutputDirectory(Command);
                    commandLogger.Info("Start ({0})", Source);
                    try
                    {
                        var processOptions = GetProcessOptions(settings);
                        processOptions.Add("-p", settings.ServerPort);
                        Task.Run(() => RunCommand(processOptions, commandLogger)).ContinueWith(t =>
                        {
                            commandLogger.Info("Finished ({0})", Source);
                            done();
                        });
                    }
                    catch (Exception ex)
                    {
                        commandLogger.Error(ex);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        // Flag: Has Dispose already been called? 
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Cancel(null);

            if (disposing)
            {
            }

            _disposed = true;
        }

        ~KarmaServeCommand()
        {
            Dispose(false);
        }
    }
}
