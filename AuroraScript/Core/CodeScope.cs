using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler.Emits;
using AuroraScript.Compiler.Exceptions;
using System;
using System.Collections.Generic;

namespace AuroraScript.Core
{
    internal enum DeclareType
    {
        /// <summary>
        /// 模块的属性或全局变量属性
        /// </summary>
        Property,

        /// <summary>
        /// 代码段变量
        /// </summary>
        Variable,

        /// <summary>
        /// 闭包捕获的变量
        /// </summary>
        Captured,
    }


    internal class ResolveValue
    {
        public DeclareType Type;
        public int Index;
        public int CaptureAlias = -1;
    }


    internal class DeclareObject
    {
        public DeclareObject(CodeScope scope, String name, String alias, DeclareType type, Int32 index, MemberAccess access, Int32 captureAlias = -1)
        {
            Name = name;
            Alias = alias;
            Type = type;
            Index = index;
            Scope = scope;
            Access = access;
            CaptureAlias = captureAlias;
        }

        public readonly String Name;
        public readonly String Alias;
        public readonly CodeScope Scope;
        public readonly DeclareType Type;
        public readonly int Index;
        public readonly MemberAccess Access;
        public readonly int CaptureAlias;
    }


    internal enum DomainType
    {
        Global,
        Module,
        Function,
        Code
    }


    internal class CodeScope
    {

        public CodeScope _parent { get; private set; }

        public int ScopeDepth { get; private set; } = 0;



        private int _variableBaseCount = 0;
        private int _maxVariableCount = 0;


        public readonly List<DeclareObject> _variables = new List<DeclareObject>();

        public readonly StringList _stringSet;
        public DomainType Domain { get; private set; }


        public CodeScope(CodeScope current, DomainType domain, StringList stringSet)
        {
            _parent = current;
            _stringSet = stringSet;
            if (_parent != null)
            {
                ScopeDepth = _parent.ScopeDepth + 1;

                if (domain == DomainType.Code)
                {
                    _variableBaseCount = _parent._variableBaseCount;
                }
            }

            Domain = domain;
            _maxVariableCount = _variableBaseCount;
        }


        /// <summary>
        /// 进入新的范围
        /// </summary>
        /// <returns></returns>

        public CodeScope Enter(StringList stringSet, DomainType domain)
        {
            var scope = new CodeScope(this, domain, stringSet);
            return scope;
        }

        /// <summary>
        /// 离开范围
        /// </summary>
        /// <returns></returns>
        public CodeScope Leave()
        {
            return _parent;
        }



        private DeclareObject findByName(String name)
        {
            for (int i = 0; i < _variables.Count; i++)
            {
                if (_variables[i].Name == name) return _variables[i];
            }
            return null;
        }



        public int Declare(DeclareType type, FunctionDeclaration func)
        {
            var name = func.Name.Value;
            var alias = name;
            var dobjeect = findByName(name);

            if (dobjeect != null)
            {
                throw new AuroraCompilerException("", "域内变量名重复");
            }
            if (Resolve(name, out var _))
            {
                // 父scope下有重名变量或属性，增加别名
            }
            int slot = 0;
            if (type == DeclareType.Property)
            {
                if (Resolve(name, out var _))
                {
                    // 父scope下有重名变量或属性，增加别名
                    alias = name + "_" + func.Name.LineNumber + "_" + func.Name.ColumnNumber;
                }
                slot = _stringSet.GetSlot(alias);
            }
            else
            {
                slot = _variableBaseCount++;
                TrackMax();
            }



            //var slot = _stringSet.GetSlot(alias);
            var declare = new DeclareObject(this, name, alias, type, slot, func.Access, -1);
            _variables.Add(declare);
            return slot;
        }



        public int Declare(DeclareType type, ImportDeclaration func)
        {
            var name = func.Name.Value;
            var alias = name;
            var dobjeect = findByName(name);
            if (dobjeect != null)
            {
                throw new AuroraCompilerException("", "域内变量名重复");
            }
            if (Resolve(name, out var _))
            {
                // 父scope下有重名变量或属性，增加别名
                alias = name + "_" + func.Name.LineNumber + "_" + func.Name.ColumnNumber;
            }
            var slot = _stringSet.GetSlot(alias);
            var declare = new DeclareObject(this, name, alias, type, slot, MemberAccess.Internal, -1);
            _variables.Add(declare);
            return slot;
        }



        public int Declare(DeclareType type, VariableDeclaration variable)
        {
            var name = variable.Name.Value;
            var existing = findByName(name);
            if (existing != null)
            {
                return existing.Index;
            }

            var alias = name;
            int slot;
            if (type == DeclareType.Property)
            {
                if (Resolve(name, out var _))
                {
                    // 父scope下有重名变量或属性，增加别名
                    alias = name + "_" + variable.Name.LineNumber + "_" + variable.Name.ColumnNumber;
                }
                slot = _stringSet.GetSlot(alias);
            }
            else
            {
                slot = _variableBaseCount++;
                TrackMax();
            }

            var declare = new DeclareObject(this, name, alias, type, slot, variable.Access, -1);
            _variables.Add(declare);
            return slot;
        }




        public int Declare(DeclareType type, string name, int captureAlias = -1)
        {
            var val = findByName(name);
            if (val != null)
            {
                throw new AuroraCompilerException("", "域内变量名重复");
            }
            int slot = 0;
            var alias = name;
            if (type == DeclareType.Property)
            {

                if (Resolve(name, out var _))
                {
                    // 父scope下有重名变量或属性，增加别名
                    //alias = name + "_" + func.Name.LineNumber + "_" + func.Name.ColumnNumber;
                }
            }
            else
            {
                slot = _variableBaseCount++;
                TrackMax();
            }
            if (type == DeclareType.Captured && captureAlias < 0)
            {
                throw new AuroraCompilerException("", "Captured variables require an alias slot.");
            }
            var declare = new DeclareObject(this, name, name, type, slot, MemberAccess.Internal, captureAlias);
            _variables.Add(declare);
            return slot;
        }
        private void TrackMax()
        {
            if (_variableBaseCount > _maxVariableCount)
            {
                _maxVariableCount = _variableBaseCount;
                _parent?.PropagateMax(_maxVariableCount);
            }
        }

        private void PropagateMax(int count)
        {
            if (count > _maxVariableCount)
            {
                _maxVariableCount = count;
                _parent?.PropagateMax(count);
            }
        }

        public int MaxVariableCount => _maxVariableCount;


        // public Boolean Resolve(string name, out DeclareObject value)
        // {
        //     value = findByName(name);
        //     if (value != null) return true;
        //     if (_parent != null) return _parent.Resolve(name, out value);
        //     return false;
        // }


        public bool Resolve(string name, out ResolveValue value)
        {
            value = null;
            var val = findByName(name);
            if (val != null)
            {
                value = new ResolveValue() { Type = val.Type, Index = val.Index, CaptureAlias = val.CaptureAlias };
                return true;
            }
            if (_parent != null)
            {
                return _parent.Resolve(name, out value);
            }
            return false;
        }


        private bool FindInParent(string name, int depth, out ResolveValue value)
        {
            value = null;
            var val = this.findByName(name);
            if (val != null)
            {
                var offset = val.Index - _variableBaseCount;
                value = new ResolveValue() { Type = val.Type, Index = offset - depth, CaptureAlias = val.CaptureAlias };
                return true;
            }

            if (this._parent != null)
            {
                return this._parent.FindInParent(name, depth + _variableBaseCount, out value);
            }

            return false;
        }



    }
}

