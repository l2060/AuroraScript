# CLR 互操作桥设计草案

## 目标
- 由宿主预先注册允许脚本访问的 CLR 类型，脚本端不提供类型导入或命名空间语法。
- 脚本仅使用宿主注册的简短别名（无命名空间）进行对象构造、字段/属性读写、方法调用。
- 提供性能优先的调用路径：元数据预热、IL 动态桩缓存、值类型快速编组。
- 控制内存占用：缓存具备 LRU 淘汰策略，避免无界增长；对象转换使用池化。

## 类型注册模型
- 宿主通过 API 显式注册类型：
  ```csharp
  var registry = engine.ClrRegistry;
  registry.RegisterType("StringBuilder", typeof(System.Text.StringBuilder));
  registry.RegisterType("Math", typeof(System.Math));
  ```
- 注册内容：
  - `alias`：脚本中使用的名称（必需，且全局唯一，大小写敏感）。
  - `Type`：真实 CLR 类型。
  - 可选：成员可见性过滤、自定义转换器。
- 注册 API 支持批量、移除、查询，便于宿主控制暴露范围。

## 脚本侧使用约定
- 无命名空间、无 import；仅允许使用宿主提供的别名。
- 对象构造：`let sb = new StringBuilder("seed");`
- 静态成员：`Math.Abs(-1)`、`Math.PI`
- 实例成员：`sb.Length`, `sb.Append("text")`
- 若需要避免名称冲突，可在宿主层约定不同别名（例如 `StringBuilder2`）。
- 字典式访问/泛型暂不开放，后续通过包装器支持。

## 运行时组件
### 1. TypeRegistry
- 入口：`ResolveType(string alias)`
- 缓存结构：
  - key: 脚本可见名称（无命名空间，宿主注册的 alias）
  - value: `ClrTypeEntry`
    - `Type Type`
    - 已解析的 `FieldInfo[]`、`PropertyInfo[]`、`MethodGroup`
    - 可选别名、限定程序集
- 生命周期：弱引用 + LRU；可配置最大条目数。

### 2. MemberMetadata
- 统一封装字段/属性/方法/构造函数 `MemberDescriptor`
  - `MemberKind Kind`（Field/Property/Method/Constructor）
  - `string Name`
  - 参数/返回值类型签名
  - 可见性限定（Public 仅、或支持自定义 Attribute 控制）
- 方法组解析策略：
  - 调用时根据参数个数与类型优先选择精确匹配
  - 不匹配时尝试可转换类型（数值扩展、字符串解析等）
  - 决策缓存：(`MemberDescriptor`, `ArgumentSignature`)→`DispatchStub`

### 3. DispatchStub 生成
- 工具类：`ClrDispatchFactory`
  - 输入：`DispatchPlan`（成员、参数转换、是否实例、返回值处理）
  - 输出：`DynamicMethod` → `Delegate`（缓存）
  - 生成策略：
    1. 准备 IL 参数数组（`LocalBuilder` 引用）
    2. 按 `ConversionPlan` 从 `ScriptDatum[]` 读取参数，执行必要转换
    3. 调用目标成员（调用指令区分 `Call`, `Callvirt`, `Newobj`）
    4. 将结果包装回 `ScriptDatum`（或写入 out/ref 参数）
  - 缓存键：`DispatchPlan.GetHashCode()`，保留弱引用并附带命中计数。

### 4. 编组策略（Marshaler）
| 类型类别 | 策略 |
| --- | --- |
| `double`, `long`, `bool` 等基元 | 直接映射到 `ScriptDatum`；
| `string` | 直接引用，必要时做 intern；
| 枚举 | 使用底层整型存储；
| `ScriptObject` → `IDictionary<string, object>` | 仅在标记需要时转换，目标 `DictionaryPool` 提供对象池；
| `Array`/`List` | 使用 `ArrayPool<object>` 缓冲；
| 非支持类型 | 抛出 `AuroraClrInteropException`，提示注册自定义转换器；

### 5. GC 与资源管理
- 通过 `ConditionalWeakTable<object, ScriptObject>` 维持 CLR 对象与脚本包装的关联，防止重复包装。
- 动态桩缓存包含访问计数；低频桩在空闲回收时释放。

## VM 集成点
- 新增 `CLR_GET_MEMBER`, `CLR_SET_MEMBER`, `CLR_INVOKE`, `CLR_NEW` 等 opcode；解释器中添加执行分支。
- `ByteCodeGenerator` 在识别到以宿主注册 alias 为目标的调用/访问时发出对应指令；脚本词法无需支持命名空间。
- 兼容旧字节码：若无新 opcode 则路径保持不变。

## 安全控制
- 默认仅允许访问白名单程序集（配置或编译期约束）。
- 可通过 Attribute 暴露/隐藏成员，例如 `[AuroraInteropExpose]`。
- 反射异常包装为脚本级 `AuroraRuntimeException`，提供详细信息。

## 开放问题
- 宿主 API 是否需要支持层级别名管理（如不同 Domain 使用不同映射）？
- 事件处理、委托支持优先级？
- 未来引入泛型时的宿主注册形态（可能通过模板式别名 `List_String`）。

## 下一步
- 完成编译器扩展：在语法分析阶段识别 `new Alias(...)`、`Alias.Member`、`Alias.Method(...)`，生成对应 CLR 指令。
- 在虚拟机中实现 `CLR_NEW`、`CLR_GET_MEMBER`、`CLR_SET_MEMBER`、`CLR_INVOKE`，运行时通过 `ClrRegistry` 查找别名。
- 设计 `DispatchPlan` 数据结构与哈希策略，实现动态桩生成与缓存。

