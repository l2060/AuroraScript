using AuroraScript.Exceptions;
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
        private readonly Dictionary<string, ClrTypeDescriptor> _aliasMap = new(StringComparer.Ordinal);
        private readonly Dictionary<Type, ClrTypeDescriptor> _typeMap = new();
        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);
        private bool _disposed;

        public ClrTypeDescriptor RegisterType(string alias, Type type, ClrTypeOptions options = null, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentException("alias cannot be null or whitespace", nameof(alias));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            alias = alias.Trim();
            options ??= ClrTypeOptions.Default;

            _lock.EnterWriteLock();
            try
            {
                EnsureNotDisposed();

                if (_aliasMap.TryGetValue(alias, out var existing))
                {
                    if (!overwrite && !options.AllowOverride)
                    {
                        throw new AuroraException($"CLR type alias '{alias}' has already been registered for '{existing.Type.FullName}'.");
                    }
                }

                if (_typeMap.TryGetValue(type, out var existingDescriptor) && !ReferenceEquals(existingDescriptor, existing))
                {
                    if (!overwrite && !options.AllowOverride)
                    {
                        throw new AuroraException($"CLR type '{type.FullName}' has already been registered with alias '{existingDescriptor.Alias}'.");
                    }
                }

                var descriptor = new ClrTypeDescriptor(alias, type, options, this);
                _aliasMap[alias] = descriptor;
                _typeMap[type] = descriptor;
                return descriptor;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool UnregisterType(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) return false;
            alias = alias.Trim();
            _lock.EnterWriteLock();
            try
            {
                EnsureNotDisposed();
                if (_aliasMap.Remove(alias, out var descriptor))
                {
                    if (descriptor != null)
                    {
                        _typeMap.Remove(descriptor.Type);
                    }
                    return true;
                }
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool TryGetDescriptor(string alias, out ClrTypeDescriptor descriptor)
        {
            descriptor = null;
            if (string.IsNullOrWhiteSpace(alias)) return false;
            //alias = alias.Trim();

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

        public bool TryGetDescriptor(Type type, out ClrTypeDescriptor descriptor)
        {
            descriptor = null;
            if (type == null) return false;

            _lock.EnterReadLock();
            try
            {
                EnsureNotDisposed();
                return _typeMap.TryGetValue(type, out descriptor);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IReadOnlyCollection<ClrTypeDescriptor> Snapshot()
        {
            _lock.EnterReadLock();
            try
            {
                EnsureNotDisposed();
                return _aliasMap.Values is IReadOnlyCollection<ClrTypeDescriptor> collection
                    ? collection
                    : new List<ClrTypeDescriptor>(_aliasMap.Values);
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

