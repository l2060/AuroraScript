using AuroraScript.Runtime.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Pool
{

    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    internal sealed class StringPool
    {
        internal static readonly StringPool Instance = new StringPool();


        private readonly ConcurrentDictionary<string, WeakReference<StringValue>> _dict = new(StringComparer.Ordinal);

        // 清理计数（无锁）
        private int _allocCounter;
        private const int CleanupInterval = 1024;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringValue Allocation(string key)
        {
            var interned = string.Intern(key);
            ArgumentNullException.ThrowIfNull(key);
            if (_dict.TryGetValue(key, out var weakRef) && weakRef.TryGetTarget(out var cached))
            {
                return cached;
            }
            // 创建或修复弱引用
            var newValue = new StringValue(key);
            _dict.AddOrUpdate(
                key,
                static (_, v) => new WeakReference<StringValue>(v),
                static (_, oldRef, v) =>
                {
                    if (oldRef.TryGetTarget(out var existing)) return oldRef;
                    return new WeakReference<StringValue>(v);
                },
                newValue
            );
            // 轻量级触发清理（不阻塞）
            if ((Interlocked.Increment(ref _allocCounter) & (CleanupInterval - 1)) == 0)
            {
                _ = Task.Run(CleanupDeadEntries);
            }
            return newValue;
        }

        /// <summary>
        /// 后台清理失效弱引用（非阻塞）
        /// </summary>
        private void CleanupDeadEntries()
        {
            foreach (var (key, weakRef) in _dict)
            {
                if (!weakRef.TryGetTarget(out _))
                {
                    _dict.TryRemove(key, out _);
                }
            }
        }

        public int Count => _dict.Count;

        public void Clear()
        {
            _dict.Clear();
            _allocCounter = 0;
        }
    }

}
