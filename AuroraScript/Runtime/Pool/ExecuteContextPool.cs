using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AuroraScript.Runtime.Pool
{
    internal static class ExecuteContextPool
    {
        private const int MaxPoolSize = 128;
        private static readonly ConcurrentBag<ExecuteContext> _pool = new();
        private static int _count;

        internal static ExecuteContext Rent(ScriptDomain domain, RuntimeVM virtualMachine, ExecuteOptions options)
        {
            if (!_pool.TryTake(out var context))
            {
                context = new ExecuteContext();
            }
            else
            {
                Interlocked.Decrement(ref _count);
            }
            context.Lease(domain, virtualMachine, options ?? ExecuteOptions.Default);
            return context;
        }

        internal static void Return(ExecuteContext context)
        {
            if (context == null)
            {
                return;
            }

            context.ResetForPool();
            if (Interlocked.Increment(ref _count) <= MaxPoolSize)
            {
                GC.SuppressFinalize(context);
                _pool.Add(context);
            }
            else
            {
                Interlocked.Decrement(ref _count);
            }
        }
    }
}
