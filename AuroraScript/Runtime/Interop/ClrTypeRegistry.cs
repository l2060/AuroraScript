using System;
using System.Collections.Generic;
using System.Threading;

namespace AuroraScript.Runtime.Interop
{
    /// <summary>
    /// 宿主侧 CLR 类型注册表，管理脚本可见的 alias 与真实类型映射。
    /// </summary>
    public sealed class ClrTypeRegistry : IDisposable
    {
        private readonly Dictionary<string, ClrType> _aliasMap = new(StringComparer.Ordinal);

        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);
        private bool _disposed;

        public void RegisterType(Type type, string alias = null, ClrTypeOptions options = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (string.IsNullOrWhiteSpace(alias))
            {
                alias = type.Name;
            }
            alias = alias.Trim();
            options ??= ClrTypeOptions.Default;
            ClrTypeResolver.ResolveType(type, out var typeDescriptor);
            var clrType = new ClrType(type, typeDescriptor);
            _aliasMap.Add(alias, clrType);
        }




        public bool UnregisterType(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) return false;
            _lock.EnterWriteLock();
            try
            {
                EnsureNotDisposed();
                return _aliasMap.Remove(alias, out var descriptor);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool TryGetClrType(string alias, out ClrType descriptor)
        {
            descriptor = null;
            if (string.IsNullOrWhiteSpace(alias)) return false;
            _lock.EnterReadLock();
            try
            {
                EnsureNotDisposed();
                return _aliasMap.TryGetValue(alias, out descriptor);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _lock.EnterWriteLock();
            try
            {
                _aliasMap.Clear();
                _disposed = true;
            }
            finally
            {
                _lock.ExitWriteLock();
                _lock.Dispose();
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ClrTypeRegistry));
            }
        }
    }
}

