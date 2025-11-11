# 值类型化重构方案

## 背景
- 当前运行时所有脚本值均继承 `ScriptObject` 并在堆上分配；高频算术与临时值会造成装箱与 GC 压力。
- 目标是在不破坏现有 API 易用性的前提下，引入值类型存储结构（`ScriptDatum`），降低分配与访问开销。

## 总体思路
1. **引入 `ScriptDatum` 结构体**
   - 包含 `ValueKind`（枚举）与联合字段：`double` / `long` / `bool` / `StringValue` / `ScriptObject`。
   - 提供静态构造方法 `FromNumber(double)`、`FromBoolean(bool)`、`FromObject(ScriptObject)` 等，支持快速判断与转换。
   - 对于“对象”分支，仅在需要完整动态行为时才持有 `ScriptObject` 引用。

2. **执行栈与局部变量改造**
   - 将 `ExecuteContext._operandStack` 与 `CallFrame.Locals` 转换为 `ScriptDatum` 数组或自定义栈结构。
   - 栈操作 (`pushStack`, `popStack`) 返回 `ScriptDatum`；旧的 `ScriptObject` 接口通过适配（如 `datum.ToObject()`）保留。
   - 引入对象池管理数组扩容，减少重复分配。

3. **数值/布尔内建类型重构**
   - `NumberValue`, `BooleanValue`, `NullValue` 保留为 `ScriptObject` 派生类型，用于原型链与对象语义。
   - 提供共享飞行重量实例或缓存，缩减 `new NumberValue()` 等重复分配。
   - 运算时优先使用 `ScriptDatum` 直接计算，必要时再回退到对象层。

4. **与 `ScriptObject` 兼容**
   - 对外 API（如 `SetPropertyValue`, `Call`）仍接受/返回 `ScriptObject`；内部实现通过 `ScriptDatum` 与 `ScriptObject` 相互转换。
   - 注意保持 Closure、迭代器、属性访问等语义不变。

5. **阶段划分（当前状态）**
   1. ✅ 实现 `ScriptDatum`、`ValueKind`、`ScriptDatumStack` 基础设施。
   2. ✅ 将局部变量、捕获变量、操作数栈迁移到 `ScriptDatum`，并改写 `RuntimeVM` 的算术/逻辑指令。
   3. ⏳ 后续：`Callable`、`BoundFunction` 仍需值类型化；CLR 桥已改用 `ScriptDatum[]` 参数，其余内建可调用对象待整理。
   4. ⏳ 回归测试：为算术、闭包、迭代器、CLR 调用建立基准与单测。
   5. ⏳ 增量优化：短字符串/对象池策略、可能的 NaN-boxing 试验。

## 注意事项
- 需要确保与闭包捕获 (`CapturedVariablee`) 兼容，可将捕获值定义为 `ScriptDatum` 并在写入时转换。
- 浮点与整型运算时保持与现有 `NumberValue` 行为一致（含 bitwise 操作）。
- `ScriptDatum` 与 `ScriptObject` 的互转要避免过多装箱，可采用 `Lazy` 创建方式，仅在脚本显式访问原型或调用方法时构造对象包装。

## 下一步
- 设计 `ScriptDatum` 与 `ValueKind` 结构草稿。
- 评估 `ExecuteContext`、`OperandStack` 改造所需的接口变更，拟定重构顺序。
- 制定基准测试脚本（算术循环、MD5、迭代器）作为性能对照。

