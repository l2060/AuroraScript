using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public class ScriptDate : ScriptObject
    {
        public DateTimeOffset DateTime { get; private set; }

        private ScriptDate()
        {
            _prototype = Prototypes.DatePrototype;
        }

        public ScriptDate(DateTime date) : this()
        {
            this.DateTime = date;
        }

        public ScriptDate(DateTimeOffset dateTimeOffset) : this()
        {
            this.DateTime = dateTimeOffset;
        }

        public ScriptDate(Int64 ticks) : this()
        {
            this.DateTime = new DateTime(ticks);
        }
    }
}
