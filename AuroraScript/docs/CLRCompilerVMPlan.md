# CLR 编译器与虚拟机升级计划

本方案描述如何在保持脚本语法简洁的前提下，让编译器与 VM 支持宿主注册的 CLR 类型别名。
> 当前实现优先采用“运行时别名解析 + 类型/实例包装”方案，复用现有 `GET_GLOBAL_PROPERTY` / `GET_PROPERTY` / `CALL` 指令。
> 以下内容保留作为后续演进（若需要引入专用 opcode 时的方向）。

## 1. 语法与 AST 识别
- 现有语法中 `new`、属性访问、函数调用均已存在，不需要引入新 token。
- 需要在语义层识别“以大写开头的无命名空间标识符 + 可选成员”这一模式，并判断其是否可能映射为 CLR 别名。
- 策略：
  - 在 `ByteCodeGenerator` 生成指令时引入 `ClrAliasResolver` 接口，运行于编译阶段的“占位判定”：
    - 若引擎提供 `IClrAliasProvider`（可选），则可在编译期获取别名列表（例如构建期由宿主传入）。
    - 若无法在编译期知晓 alias，则在字节码中保留普通指令，同时在运行时尝试别名解析（参考方案：新增 `CLR_RESOLVE_ALIAS` 指令，执行时检查 `ClrRegistry`）。该方式安全但开销更高。
  - 推荐“延迟到运行时”策略，以兼容脚本编译与宿主注册的时序。

## 2. 新增 opcode
| 指令 | 参数 | 栈行为 | 说明 |
| --- | --- | --- | --- |
| `CLR_NEW` | stringIndex (别名) , byte argc | | 从常量池读取别名，查找 `ClrRegistry`，根据构造函数签名生成/调用动态桩并推入实例。 |
| `CLR_STATIC_GET` | stringIndex typeAlias, stringIndex memberName | | 读取静态字段/属性 |
| `CLR_STATIC_SET` | stringIndex typeAlias, stringIndex memberName | value | 写入静态字段/属性 |
| `CLR_STATIC_CALL` | stringIndex typeAlias, stringIndex methodName , byte argc | args..., -> result | 静态方法调用 |
| `CLR_INSTANCE_GET` | stringIndex memberName | obj -> value | 在对象实例上读取属性/字段（实例对象从栈顶弹出并恢复） |
| `CLR_INSTANCE_SET` | stringIndex memberName | obj, value -> | 设置属性/字段 |
| `CLR_INSTANCE_CALL` | stringIndex memberName , byte argc | obj, args -> result | 实例方法调用 |

- 实现细节：对于实例操作，可考虑使用统一的 `CLR_GET_MEMBER` 包含布尔标志指示实例/静态。为简化版本一，可将静态/实例区分开。

## 3. 字节码生成规则
- `new Alias(args)` → `CLR_NEW aliasIndex, argc`，参数按现有调用顺序压栈后调用。
- `Alias.Member`（未调用）→ `CLR_STATIC_GET`
- `Alias.Member = expr` → `CLR_STATIC_SET`
- `Alias.Method(args)` → `CLR_STATIC_CALL`
- `obj.Member` → `CLR_INSTANCE_GET`
- `obj.Member = expr` → `CLR_INSTANCE_SET`
- `obj.Method(args)` → `CLR_INSTANCE_CALL`
- 若别名在运行时不存在，VM 抛出 `AuroraRuntimeException`。
- 仍保留原有 `GET_GLOBAL_PROPERTY` 路径，方便脚本访问普通全局对象。

## 4. VM 扩展
- 在 `RuntimeVM` 的主循环中添加新 opcode 分支。
- 解析别名：`engine.ClrRegistry.TryGetDescriptor(alias)`，如失败抛错。
- 动态桩缓存：
  - 使用 `ClrTypeDescriptor` 的方法/字段缓存。
  - 为调用桩建立 `(MemberInfo, ArgumentSignature)` → delegate 缓存。
  - 可复用计划中的 `ClrDispatchFactory`。
- 返回值：确保转换为 `ScriptObject`，使用计划中的 `Marshaler`。

## 5. 兼容性
- 未更新的脚本仍按旧指令执行。
- 为避免字节码版本冲突，可在字节码头（若有）或 `RuntimeVM` 初始化时传入版本号；若新 opcode 未识别，抛出友好异常。

## 6. 迭代路线
1. 添加新 opcode 定义及 `InstructionBuilder` 帮助方法。
2. 在 `ByteCodeGenerator` 中识别调用/字段访问模式，发出 opcode。
3. 扩展 `RuntimeVM` 处理新指令，打通最小调用链（静态方法 + 构造函数）。
4. 引入 `ClrDispatchFactory` 完整实现（含桩缓存）。
5. 编写示例脚本与宿主代码，验证常见场景（构造对象、属性读写、方法调用）。
6. 扩展测试覆盖更多类型（数值、字符串、对象、集合）。

