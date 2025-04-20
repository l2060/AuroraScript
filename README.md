﻿# Aurora Script
这是一个轻量级的弱类型脚本执行引擎，他没有引用任何第三方组件，它通过将脚本编译为字节码然后通过虚拟机来解释运行

它目前还是个玩具，速度可能会很慢，但是它可以正常的跑起来还可以输出一些东西。

设计它时借鉴了javascript的一些机制和语法但它不是javascript，它不会遵守ECMA规范。

下图中的`MD5_SUM Used 126ms`包含了.net代码预热的执行时间，实际测试`1ms`左右。

![avatar](/Documents/snipaste.png)



---

## 脚本特性 

 - [x] Domain
 - [x] 模块
 - [x] 方法+闭包
 - [x] 方法调用/Clr方法调用
 - [x] 脚本执行 中断/继续
 - [x] where for
 - [x] export 导出模块方法和变量
 - [x] Import
 - [x] MD5函数与Javascript输出一致
 - [ ] 导出属性的访问权限  export  const
 - [x] 迭代器 Iterator 
 - [x] for in
 - [ ] 固定大小的本地变量表测量
 - [ ] CallFrame 复用
 - [ ] 脚本对象NumberValue、StringValue的优化
 - [ ] 调试符号表、闭包方法名、行号、列号、调用者行号
 


 

 ## Domain
 隔离的脚本环境，Domain之间的执行环境是隔离的，但是他们共用了AuroraEngine的Global对象

 Domain也有自己的Global对象，他继承了AuroraEngine的Global对象

 ## Module 
 每个脚本文件为一个Module，可以理解为一个对象。

 Module内的root方法和root变量作为 Module的Property，他们是可以被外部访问的。

 在脚本头部可以通过使用`@module("MODULENAME");`定义Module名字，如果未定义则使用文件的相对路径作为Module名字

 Module之间可以通过 Import xxx from 'modulefile'; 进行引用，这里会将modulefile作为当前module的xxx变量，这样你可以通过xxx.yy来访问modulefile的导出方法或属性。

 ## Function
 方法调用支持闭包方法、Lambda方法、方法指针，你可以发挥你的想象。


 ## Interruption & Continue

 脚本中可以通过yield指令进行中断，中断后Execute方法会立即返回，可以通过ExecuteContext的Continue方法从中断位置继续执行。

 也可以通过ExecuteOptions选项禁用yield指令，或通过AutoInterruption字段定义自动中断机制。

 在异常上下文上调用Continue可能会导致不可预料的结果，在计算类的脚本执行过程中出现异常应拒绝Continue继续执行，异常继续的机制适用于在面向方法的脚本中。

 


 ## 支持的类型
 - String
 - Number
 - Boolean
 - Null
 - Array
 - Map Object
 - Closure
 - ClrFunction


 ---

##  创建环境 & 编译脚本
``` csharp  

// 创建一个脚本环境
var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./tests/" });
// 编译脚本，编译器会扫描工作目录下所有脚本文件，并根据Import语句自动编译依赖的脚本
await engine.BuildAsync("./unit.as");

```


## 定义全局变量
``` csharp  

// 1. 普通方法，直接设置属性值， 可读性、可写性、可枚举性都为true
engine.Global.SetPropertyValue("PI", g.GetPropertyValue("PI"))

// 2. 高级方法，支持设置属性的可读性、可写性、可枚举性
engine.Global.Define("PI", g.GetPropertyValue("PI"));

// 3. 定义CLR方法
engine.Global.Define("debug", new ClrFunction(LOG), writeable: false, enumerable: false);

// CLR方法
public static ScriptObject LOG(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
{
    Console.WriteLine(String.Join(", ", args));
    return ScriptObject.Null;
}

// 获取全局变量
var pi = engine.Global.GetPropertyValue("PI");

// Domain 的全局变量
domain.Global.SetPropertyValue("PI", g.GetPropertyValue("PI"))
domain.Global.Define("PI", g.GetPropertyValue("PI"));
var pi = domain.Global.GetPropertyValue("PI");
```




##  创建Domain
``` csharp  

// 1. 如果你的模块脚本顶级语句使用了全局变量，那么你需要提前创建好Global环境。
var g = engine.NewEnvironment();
g.Define("PI", new NumberValue(Math.PI), readable: true, writeable: false, enumerable: false);
var domain = engine.CreateDomain(g);

// 2. 如果你的模块脚本顶级语句没有使用模块以外的变量，可以直接创建一个Domain
var domain = engine.CreateDomain();
domain.Global.Define("PI", new NumberValue(Math.PI), readable: true, writeable: false, enumerable: false);

```



##  执行脚本方法
``` csharp  

var forCount = 10000000;
// 执行UNIT_LIB中的  forTest 方法 并传入参数 10000000
var testFor = domain.Execute("UNIT_LIB", "forTest", new NumberValue(forCount));
Console.WriteLine($"for:{forCount} UsedTime {testFor.UsedTime}ms");

```



##  继续中断的上下文
``` csharp  

// 1. 手动控制中断继续
var testContinue = domain.Execute("UNIT_LIB", "testContinue");
if (testContinue.Status  == ExecuteStatus.Interrupted)
{
    testContinue.Continue();
}

// 2. 自动完成，如遇到中断自动继续直到完成，如果遇到异常则返回
var testContinue = domain.Execute("UNIT_LIB", "testContinue").Done();

// 3. 自动完成，不管异常还是中断都可以继续执行直到完成
var testContinue = domain.Execute("UNIT_LIB", "testContinue").Done(AbnormalStrategy.Continue);

```

## 特殊的文本模板

``` csharp

// 1. 双引号
log("Hello");

// 2. 单引号
log('Wrold');


// 3. 多行文本
log(`1. 这是一个特殊的字符串模板
2. 支持多行文本
3. 它会让代码看起来更舒服
4. <Buy/@Buy> <Close/@Close>`);


// 4. 多行文本
log(
    |> 1. 这是一个特殊的字符串模板
    |> 2. 支持多行文本
    |> 3. 它会让代码看起来更舒服
    |> 4. <Buy/@Buy> <Close/@Close> 
);



```
