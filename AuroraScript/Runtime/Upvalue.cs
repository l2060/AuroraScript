using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示一个闭包捕获到的变量。Upvalue 会在引用的调用帧仍然存在时直接访问该帧的局部槽位；
    /// 当调用帧结束时，Upvalue 会被关闭并保存当前值的副本，从而保证闭包后续访问的正确性。
    /// </summary>
    internal sealed class Upvalue : ScriptObject
    {
        private CallFrame _ownerFrame;
        private Int32 _localIndex;
        private readonly Int32 _originalSlot;
        private Int32 _aliasSlot = -1;
        private ScriptDatum _closedValue;
        private readonly String _name;

        internal Upvalue(CallFrame ownerFrame, Int32 localIndex, String name = null)
            : base(null, false)
        {
            _ownerFrame = ownerFrame ?? throw new ArgumentNullException(nameof(ownerFrame));
            _localIndex = localIndex;
            _originalSlot = localIndex;
            _name = name;
        }

        internal Int32 Slot => _localIndex;

        public override string ToString()
        {
            if (_name != null)
            {
                return $"<upvalue {_name}>";
            }
            if (_ownerFrame != null)
            {
                return $"<upvalue @{_localIndex} (open)>";
            }
            return $"<upvalue @{_localIndex} (closed)>";
        }

        internal bool IsClosed => _ownerFrame == null;

        internal ScriptDatum Get()
        {
            if (_ownerFrame != null)
            {
                return _ownerFrame.GetLocalDatum(_localIndex);
            }
            return _closedValue;
        }

        internal void Set(ScriptDatum value)
        {
            if (_ownerFrame != null)
            {
                _ownerFrame.SetLocalDatum(_localIndex, value);
            }
            else
            {
                _closedValue = value;
            }
        }

        internal void Close()
        {
            if (_ownerFrame != null)
            {
                _closedValue = _ownerFrame.GetLocalDatum(_localIndex);
                _ownerFrame = null;
            }
        }

        internal void Rebind(CallFrame ownerFrame, Int32 index)
        {
            _ownerFrame = ownerFrame;
            _localIndex = index;
            _aliasSlot = -1;
        }

        internal void MarkAliasSlot(Int32 slot)
        {
            if (slot >= 0)
            {
                _aliasSlot = slot;
            }
        }

        internal Int32 ConsumeAliasSlot()
        {
            var slot = _aliasSlot;
            _aliasSlot = -1;
            if (slot >= 0)
            {
                return slot;
            }
            if (_ownerFrame != null)
            {
                return _localIndex;
            }
            return _originalSlot;
        }
    }

    /// <summary>
    /// 在闭包对象中记录捕获到的局部槽位以及对应的 Upvalue。
    /// </summary>
    internal readonly struct ClosureUpvalue
    {
        internal readonly Int32 Slot;
        internal readonly Upvalue Upvalue;

        internal ClosureUpvalue(Int32 slot, Upvalue upvalue)
        {
            Slot = slot;
            Upvalue = upvalue;
        }
    }
}

