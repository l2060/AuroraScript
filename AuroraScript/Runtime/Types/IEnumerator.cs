using AuroraScript.Runtime.Types.Internal;

namespace AuroraScript.Runtime.Types
{
    internal interface IEnumerator
    {

        /// <summary>
        /// 获取当前对象的迭代器
        /// </summary>
        /// <returns></returns>
        ItemIterator GetIterator();

    }
}
