using JsTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaServerTestLogger : TestServerLogger
    {
        public KarmaServerTestLogger(ITestLogger logger)
            : base(logger)
        {
        }

        private static Regex messageRe = new Regex(@"^(INFO|WARN|ERROR|DEBUG)\s*(.*)$");
        public override void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            var match = messageRe.Match(message);
            if (match.Success)
            {
                switch (match.Groups[1].Value)
                {
                    case "INFO":
                        this.Info(match.Groups[2].Value);
                        break;
                    case "WARN":
                        this.Warn(match.Groups[2].Value);
                        break;
                    case "ERROR":
                        this.Error(match.Groups[2].Value);
                        break;
                    case "DEBUG":
                        this.Debug(match.Groups[2].Value);
                        break;
                    default:
                        this.Debug(message);
                        break;
                }
            }
            else
            {
                this.Debug(message);
            }
        }
    }
}
