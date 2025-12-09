using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AuroraScript.Runtime
{
    internal static class CallFramePool
    {
        private const int MaxPoolSize = 512;
        private static readonly ConcurrentBag<CallFrame> _pool = new();
        private static int _count;

        internal static CallFrame Rent()
        {
            if (!_pool.TryTake(out var frame))
            {
                frame = new CallFrame();
            }
            else
            {
                Interlocked.Decrement(ref _count);
            }
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

        internal static Int32 Size
        {
            get { return _pool.Count; }
        }


    }
}
