using System.Text;

namespace AuroraScript.Runtime.Base
{

    public partial class ScriptObject
    {
        public static readonly ScriptObject Null = NullValue.Instance;
        protected Dictionary<String, ObjectProperty> _properties = new Dictionary<String, ObjectProperty>();

        internal ScriptObject _prototype;

        internal Boolean IsFrozen { get; set; } = false;


        public ScriptObject()
        {
            _prototype = ScriptObject.Prototype;
        }


        public ScriptObject this[String key]
        {
            get
            {
                return GetPropertyValue(key);
            }
            set
            {
                SetPropertyValue(key, value);
            }
        }



        public virtual ScriptObject GetPropertyValue(String key, ScriptObject thisObject = null)
        {
            ScriptObject own = thisObject != null ? thisObject : this;
            _properties.TryGetValue(key, out var value);
            if (value != null)
            {
                if (!value.Readable) throw new Exception("Property disables write");

                if (value.Value is ClrGetter getter)
                {
                    return getter.Invoke(own);
                }
                return value.Value;
            }
            if (_prototype != null)
            {
                return _prototype.GetPropertyValue(key, own);
            }
            return Null;
        }

        public virtual void SetPropertyValue(String key, ScriptObject value)
        {
            if (IsFrozen)
            {
                throw new Exception("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Readable = true;
                existValue.Writeeable = true;
                _properties[key] = existValue;
            }
            if (!existValue.Writeeable) throw new Exception("Property disables write");
            existValue.Value = value;
        }


        public virtual void Define(String key, ScriptObject value, bool readable = true, bool writeable = true)
        {
            if (IsFrozen)
            {
                throw new Exception("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Readable = readable;
                existValue.Writeeable = writeable;
                _properties[key] = existValue;
            }
            else
            {
                if (!existValue.Writeeable) throw new Exception("Property disables write");
            }
            existValue.Value = value;
        }



        public override string ToString()
        {
            return "";
            if (_properties.Count == 0) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var pair in _properties)
            {
                sb.Append($"\"{pair.Key}\": ");
                sb.Append((pair.Value != null && pair.Value.Value != null) ? pair.Value.Value.ToString() : "");
                sb.Append(", ");
            }
            sb.Length -= 2;
            sb.Append("}");
            return sb.ToString();
        }




        public virtual string ToDisplayString()
        {
            if (_properties.Count == 0) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var pair in _properties)
            {
                sb.Append($"\"{pair.Key}\": ");
                sb.Append(pair.Value.Value.ToDisplayString());
                sb.Append(", ");
            }
            sb.Length -= 2;
            sb.Append("}");
            return sb.ToString();
        }


        public static StringValue operator +(ScriptObject a, ScriptObject b)
        {
            return new StringValue(a.ToString() + b.ToString());
        }


        public virtual Boolean IsTrue()
        {
            return true;
        }

    }




}
