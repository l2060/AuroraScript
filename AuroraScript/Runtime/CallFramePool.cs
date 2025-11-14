using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System.Collections.Concurrent;
using System.Threading;

namespace AuroraScript.Runtime
{
    internal static class CallFramePool
    {
        private const int MaxPoolSize = 512;
        private static readonly ConcurrentBag<CallFrame> _pool = new();
        private static int _count;

        internal static CallFrame Rent(ScriptDomain domain, ScriptModule module, int entryPointer, ScriptDatum[] argumentDatums, ClosureUpvalue[] captured)
        {
            if (!_pool.TryTake(out var frame))
            {
                frame = new CallFrame();
            }
            else
            {
                Interlocked.Decrement(ref _count);
            }

            frame.Initialize(domain, module, entryPointer, argumentDatums, captured);
            return frame;
        }

        internal static CallFrame Rent(ScriptDomain domain, ScriptModule module, int entryPointer, ScriptObject[] arguments, ClosureUpvalue[] captured)
        {
            if (!_pool.TryTake(out var frame))
            {
                frame = new CallFrame();
            }
            else
            {
                Interlocked.Decrement(ref _count);
            }

            frame.Initialize(domain, module, entryPointer, arguments, captured);
            return frame;
        }

        internal static void Return(CallFrame frame)
        {
            if (frame == null)
            {
                return;
            }

            if (Interlocked.Increment(ref _count) <= MaxPoolSize)
            {
                frame.ResetForPool();
                _pool.Add(frame);
            }
            else
            {
                Interlocked.Decrement(ref _count);
                frame.ResetFull();
            }
        }
    }
}
