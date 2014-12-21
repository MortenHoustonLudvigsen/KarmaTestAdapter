using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoPS.Processes;

namespace KarmaTestAdapter.Commands
{
    public static class ProcessOptionsExtensions
    {
        public static bool AddFileOption(this ProcessOptions processOptions, string option, string path)
        {
            return processOptions.Add(option, PathUtils.GetRelativePath(processOptions.WorkingDirectory, path, true));
        }
    }
}
