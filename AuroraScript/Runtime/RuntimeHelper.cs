using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;

namespace AuroraScript.Runtime
{
    internal class RuntimeHelper
    {
        internal static ScriptDatum Clone(ScriptDatum origin, bool deepth = false)
        {
            switch (origin.Kind)
            {
                case ValueKind.Null:
                case ValueKind.Number:
                case ValueKind.Boolean:
                case ValueKind.String:
                    return origin;
                default:
                    if (deepth)
                    {
                        return ScriptDatum.FromObject(DeepClone(origin.Object));
                    }
                    else
                    {
                        return ShallowClone(origin);
                    }
            }

        }





        private static ScriptDatum ShallowClone(ScriptDatum origin)
        {
            switch (origin.Object)
            {
                case ScriptDate date:
                    return origin;
                case ClrInstanceObject clrInstance:
                    return origin;
                case ScriptRegex regex:
                    return origin;
                case ClosureFunction closure:
                    return origin;
                case ClrType clrType:
                    return origin;
                case ClrMethodBinding clrFunc:
                    return origin;
                case BondingFunction bonding:
                    return origin;
                case ScriptArray array:
                    return ScriptDatum.FromArray(new ScriptArray(array));
                default:
                    var newObject = new ScriptObject();
                    CopyProperties(origin.Object, newObject);
                    return ScriptDatum.FromObject(newObject);
            }
        }

        private static ScriptObject DeepClone(ScriptObject origin)
        {
            switch (origin)
            {
                case ScriptDate date:
                    return origin;
                case ClrInstanceObject clrInstance:
                    return origin;
                case ScriptRegex regex:
                    return origin;
                case ClosureFunction closure:
                    return origin;
                case ClrType clrType:
                    return origin;
                case ClrMethodBinding clrFunc:
                    return origin;
                case BondingFunction bonding:
                    return origin;
                case ScriptArray array:
                    return new ScriptArray(array);
                default:
                    return DeepCloneObject(origin);
            }
        }

        private static ScriptObject DeepCloneObject(ScriptObject source)
        {
            var target = new ScriptObject();

            if (source._prototype == target._prototype)
            {
                foreach (var item in source._properties)
                {
                    ObjectProperty property = item.Value.Clone();
                    property.Value = DeepCloneObject(property.Value);
                    target._properties.Add(item.Key, property);
                }
                return target;
            }

            var fromKeys = source.EnumerationKeys();
            if (fromKeys.Count > 0)
            {
                foreach (var key in fromKeys)
                {
                    var propety = source._resolveProperty(key);
                    if (propety != null)
                    {
                        target.Define(key, DeepCloneObject(propety.Value), propety.Writable, propety.Enumerable);
                    }
                }
            }
            return target;
        }







        internal static void CopyProperties(ScriptObject source, ScriptObject target, bool force = false)
        {
            if (source._prototype == target._prototype)
            {
                foreach (var item in source._properties)
                {
                    if (target._properties.TryGetValue(item.Key, out var targetPropety))
                    {
                        if (targetPropety.Writable || force)
                        {
                            targetPropety.Value = item.Value.Value;
                            targetPropety.Enumerable = item.Value.Enumerable;
                            targetPropety.Writable = item.Value.Writable;
                        }
                    }
                    else
                    {
                        target._properties.Add(item.Key, item.Value.Clone());
                    }
                }
                return;
            }

            var fromKeys = source.EnumerationKeys();
            if (fromKeys.Count > 0)
            {
                foreach (var key in fromKeys)
                {
                    var propety = source._resolveProperty(key);
                    if (propety != null)
                    {
                        var targetPropety = target._resolveProperty(key);
                        if (targetPropety != null)
                        {
                            if (targetPropety.Writable || force)
                            {
                                targetPropety.Value = propety.Value;
                            }
                        }
                        else
                        {
                            target.Define(key, propety.Value, propety.Writable, propety.Enumerable);
                        }
                    }
                }
            }
        }


    }

}
