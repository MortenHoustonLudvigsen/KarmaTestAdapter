using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoPS.Processes;

namespace NpmProxy
{
    public partial class Npm : ProcessRunner
    {
        private const string _cmdName = "npm.cmd";

        private class Options : ProcessRunnerOptions
        {
            public Options(Npm npm)
            {
                Global = npm.Global;
            }

            public bool Global { get; set; }
            public string Command { get; set; }

            public override void SetProcessOptions(ProcessOptions processOptions)
            {
                processOptions.Add(Command);
                processOptions.Add("-g", Global);
                AddOtherArguments(processOptions);
                AddStandardInput(processOptions);
            }
        }

        /// <summary>
        /// Create an instance of Npm
        /// </summary>
        /// <param name="npmCmd">File path to the npm cmd file</param>
        public Npm(string npmCmd)
            : base(npmCmd)
        {
        }

        /// <summary>
        /// Create an instance of Npm
        /// The first file named "npm.cmd" in the system path will be used as the npm cmd file
        /// </summary>
        public Npm()
            : base(ProcessUtils.FindExecutable(_cmdName))
        {
        }

        /// <summary>
        /// Specifies whether to run npm in global mode
        /// </summary>
        public bool Global { get; set; }
    }
}
