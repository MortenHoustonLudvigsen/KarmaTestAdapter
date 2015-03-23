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
                        this.Debug(message);
                        break;
                    case "WARN":
                        this.Warn(message);
                        break;
                    case "ERROR":
                        this.Error(message);
                        break;
                    case "DEBUG":
                        this.Debug(message);
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
