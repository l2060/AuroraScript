namespace AuroraScript.Core
{
    public enum ValueKind : byte
    {
        Null,
        Boolean,
        Number,
        String,
        Object,
        Array,

        Module,


        Regex,
        /// <summary>
        /// 脚本原生方法
        /// </summary>
        Function,

        ClrType,


        /// <summary>
        /// Clr原生方法
        /// </summary>
        ClrFunction,

        /// <summary>
        /// 原型对象的Clr粘合函数
        /// </summary>
        ClrBonding



    }
}

