using AuroraScript.Runtime.Base;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Types
{
    public sealed class ScriptDate : ScriptObject
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

        public String Format(String format = null)
        {
            if (format == null) format = AuroraEngine.DateTimeFormat;
            return DateTime.ToString(format);
        }

    }
}
