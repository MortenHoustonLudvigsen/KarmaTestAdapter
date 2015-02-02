using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public static class ExceptionExtensions
    {
        public static Exception Wrap(this Exception ex, string message, params object[] args)
        {
            return new Exception(string.Format(message, args), ex);
        }
    }
}
