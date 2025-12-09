using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Core
{
    public partial struct ScriptDatum
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetAnyObject(out ScriptObject value)
        {
            value = this.Object;
            return this.Object != null;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean IsObject()
        {
            return this.Object != null;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetObject(out ScriptObject value)
        {
            if (this.Kind == ValueKind.Object)
            {
                value = this.Object;
                return true;
            }
            value = null;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetArray(out ScriptArray value)
        {
            if (this.Kind == ValueKind.Array)
            {
                value = (ScriptArray)this.Object;
                return true;
            }
            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetRegex(out ScriptRegex value)
        {
            if (this.Kind == ValueKind.Regex)
            {
                value = (ScriptRegex)this.Object;
                return true;
            }
            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetClrType(out ClrType value)
        {
            if (this.Kind == ValueKind.ClrType)
            {
                value = (ClrType)this.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetClrBonding(out BondingFunction value)
        {
            if (this.Kind == ValueKind.ClrBonding)
            {
                value = (BondingFunction)this.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetClrFunction(out ClrMethodBinding value)
        {
            if (this.Kind == ValueKind.ClrFunction)
            {
                value = (ClrMethodBinding)this.Object;
                return true;
            }
            value = null;
            return false;
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Boolean TryGetClrInvokable(out IClrInvokable value)
        {
            if (this.Kind == ValueKind.ClrFunction || Kind == ValueKind.ClrType)
            {
                value = (IClrInvokable)Object;
                return true;
            }
            value = null;
            return false;
        }


 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetDate(out ScriptDate value)
        {
            if (this.Kind == ValueKind.Date)
            {
                value = (ScriptDate)this.Object;
                return true;
            }
            value = null;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Boolean TryGetFunction(out ClosureFunction value)
        {
            if (this.Kind == ValueKind.Function)
            {
                value = (ClosureFunction)this.Object;
                return true;
            }
            value = null;
            return false;
        }

    }
}
