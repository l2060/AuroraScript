namespace AuroraScript.Runtime
{

    public class ScriptObject
    {

        public string Name = "";
        private ScriptObject _prototype = null;
        public ScriptObject Prototype => _prototype;








        public ScriptObject()
        {

        }


        //public object GetPropertyValue(object key)
        //{
        //    return GetPropertyValue(key, this);
        //}

        //public virtual object GetPropertyValue(object key, object thisValue)
        //{
        //    // Check if the property is an indexed property.
        //    uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
        //    if (arrayIndex != uint.MaxValue)
        //        return GetPropertyValue(arrayIndex, thisValue);

        //    // Otherwise, the property is a name.
        //    return GetNamedPropertyValue(key, thisValue);
        //}

        //public bool SetPropertyValue(object key, object value, bool throwOnError)
        //{
        //    return SetPropertyValue(key, value, this, throwOnError);
        //}




    }




}
