using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JsTestAdapter.Helpers
{
    public static class TaskCompletionSourceExtensions
    {
        public static TaskCompletionSource<T> SetTimeout<T>(this TaskCompletionSource<T> source, int timeout = Timeout.Infinite)
        {
            var token = new CancellationTokenSource(timeout).Token;
            token.Register(() => source.TrySetCanceled(), useSynchronizationContext: false);
            return source;
        }
    }
}