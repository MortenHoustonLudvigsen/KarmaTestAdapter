using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Helpers
{
    public static class IServiceProviderExtensions
    {
        private static IVsSolution GetSolution(this IServiceProvider serviceProvider)
        {
            return (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
        }

        public static IEnumerable<IVsProject> GetLoadedProjects(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetSolution().GetLoadedProjects();
        }

        public static string GetSolutionDirectory(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetSolution().GetSolutionDirectory();
        }
    }
}
