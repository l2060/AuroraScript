namespace AuroraScript.Core
{
    public enum ValueKind : byte
    {
        Null = 0,
        Boolean = 1,
        Number = 2,
        String = 3,
        Object = 4,
        Array = 5,

        /// <summary>
        /// 脚本原生方法
        /// </summary>
        Function = 6,

        ClrType = 7,


        /// <summary>
        /// Clr原生方法
        /// </summary>
        ClrFunction = 8,

        /// <summary>
        /// 原型对象的Clr粘合函数
        /// </summary>
        ClrBonding = 9



    }
}

