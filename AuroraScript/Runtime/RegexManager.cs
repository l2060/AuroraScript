using AuroraScript.Exceptions;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace AuroraScript.Runtime
{
    internal class RegexManager
    {

        private const Char CacheSeparator = '\u001F';

        private static readonly ConcurrentDictionary<String, WeakReference<ScriptRegex>> _cache = new ConcurrentDictionary<string, WeakReference<ScriptRegex>>();

        /// <summary>
        /// 用于将 javascript 正则表达式 生成 脚本正则对象
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ScriptRegex Resolve(string pattern, string flags)
        {
            var key = CreateCacheKey(pattern, flags);
            if (_cache.TryGetValue(key, out var regex))
            {
                if (regex.TryGetTarget(out var target))
                {
                    return target;
                }
                else
                {
                    target = Build(pattern, flags);
                    regex.SetTarget(target);
                    return target;
                }
            }
            else
            {
                var target = Build(pattern, flags);
                regex = new WeakReference<ScriptRegex>(target);
                _cache.TryAdd(key, regex);
                return target;
            }
        }


        private static ScriptRegex Build(string pattern, string flags)
        {
            // 2. 转换 JS -> .NET 正则语法（兼容性修正）
            pattern = ConvertJsPatternToDotNet(pattern);
            // 3. 解析 JS flags 到 .NET RegexOptions
            var options = JsFlagsToDotNetOptions(flags);
            return new ScriptRegex(new Regex(pattern, options), flags);
        }

        private static string ConvertJsPatternToDotNet(string pattern)
        {
            // JS 与 .NET 大部分语法互通，只需要做安全转义

            // 处理 JS 的不兼容语法，如：
            // 1. JS 不支持后行 (?<=...)，但 .NET 支持 —— 不需要处理
            // 2. 处理 ECMAScript Unicode \u{1F600} 形式
            pattern = Regex.Replace(pattern, @"\\u\{([0-9A-Fa-f]+)\}", m =>
            {
                int value = Convert.ToInt32(m.Groups[1].Value, 16);
                return char.ConvertFromUtf32(value);
            });

            // 3. 将 JS 的 (?<!...)、(?<=...) 保留 —— .NET 可直接支持  
            //（不做修改）

            return pattern;
        }

        private static RegexOptions JsFlagsToDotNetOptions(string flags)
        {
            RegexOptions options = RegexOptions.None;

            if (String.IsNullOrEmpty(flags))
            {
                return options;
            }

            var seenFlags = new HashSet<Char>();

            foreach (char flag in flags)
            {
                if (!seenFlags.Add(flag))
                {
                    throw new AuroraRuntimeException($"Duplicate regex flag '{flag}'.");
                }

                switch (flag)
                {
                    case 'i': options |= RegexOptions.IgnoreCase; break;
                    case 'm': options |= RegexOptions.Multiline; break;
                    case 's': options |= RegexOptions.Singleline; break;
                    case 'g': /* 全局 g 在 .NET 中手动循环处理 */ break;
                    case 'u': /* .NET 默认 Unicode，无需 */ break;
                    case 'y': /* 粘性匹配 JS 特性，.NET 不支持 */ break;
                    default:
                        throw new AuroraRuntimeException($"Unsupported regex flag '{flag}'.");
                }
            }

            return options;
        }

        private static String CreateCacheKey(String pattern, String flags)
        {
            pattern ??= String.Empty;
            flags ??= String.Empty;
            return String.Concat(pattern, CacheSeparator, flags);
        }

    }
}
