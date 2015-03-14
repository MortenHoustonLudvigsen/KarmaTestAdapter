using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpmProxy
{
    partial class Npm
    {
        /// <summary>
        /// npm root - Print the effective node_modules folder to standard out.
        /// </summary>
        /// <param name="global">Specifies whether to run npm in global mode</param>
        /// <returns>The effective node_modules folder to standard out</returns>
        public string Root(bool global = false)
        {
            return Run(new Options(this)
            {
                Global = global,
                Command = "root"
            }).StandardOutputList.FirstOrDefault();
        }
    }
}
