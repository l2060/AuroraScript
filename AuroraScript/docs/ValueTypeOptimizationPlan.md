# 值类型化后续优化计划

## 背景
现有值类型化已覆盖执行栈、局部变量、CLR 调用，但基准结果仍显示：

| Benchmark | 时间 | 分配 |
|-----------|------|------|
| `benchmarkNumbers` | 1444 ms | ~4.7 KB |
| `benchmarkArrays` | 620 ms | ~141 MB |
| `benchmarkClosure` | 4901 ms | ~919 MB |

说明瓶颈集中在数组存储与闭包返回值。

## 优化目标
1. **数组**：将分配降至百 KB 级，把 `ScriptArray` 操作改造为值类型存储。
2. **闭包**：降低闭包调用的对象创建（`NumberValue`、`ScriptObject[]`）。
3. **CLR 调用**：跟随数组/闭包优化带来连带收益。

## 方案与步骤

### 1. ScriptArray 值类型化
- **设计**：将内部 `_items` 从 `List<ScriptObject>` 改为 `ScriptDatum[]` + `Count`。保留 `ScriptObject` 接口，通过 `ScriptDatum.ToObject()` 惰性转换。
- **功能点覆盖**：
  - `[index]` 读取：返回 `ScriptDatum` → `ScriptObject`。
  - `push`/`pop`：直接操作 `ScriptDatum`。
  - 迭代器 `ItemIterator`：转为按 `ScriptDatum` 构建，或在迭代时转换。
- **关键改动**：
  - `ScriptArray.cs` + `.g.cs` + `RuntimeVM` 中 `NEW_ARRAY`、`GET/SET_ELEMENT`。
  - `ArrayConstructor`、数组原型方法 (`push`, `pop`, `slice` 等)。
  - 逃逸场景（例如将数组传给 CLR 类型）仍需调用 `ToObject()`。
- **测试**：更新 `benchmarkArrays`，验证分配与时间；确保数组脚本行为一致。

### 2. Callable 参数/返回值微调
- **目标**：减少 `ScriptObject[]` 临时数组、`NumberValue` 创建。
- **策略**：
  - 针对入参数量少（0~2），使用静态缓存数组或 `stackalloc`。
  - `Callable` 原生接受 `ScriptDatum[]`，仅通过遗留适配器在需要时转换为 `ScriptObject[]`。
  - 对常用数值 `NumberValue` 采用飞量缓存（例如 `[-128,256)` 小整数）。
  - 闭包返回 `NumberValue` 时，优先返回缓存对象。
- **影响范围**：`Callable`, `BoundFunction`, `ClosureFunction`, `NumberValue`.
- **测试**：`benchmarkClosure` 应显著降低分配量；同时回归 `testClouse` 行为。

### 3. 附加项（可并行/视时间）
- **CLR 调用缓存**：重用 `ScriptObject[]` 结果，进一步削减 `testClrFunc` 分配。✅ 通过 `ArrayPool` 复用参数数组；后续可考虑结果缓存与托管对象生命周期分析。
- **字符串/对象池**：针对短字符串、Map 临时对象进行池化（可选）。🔍 当前缺少数据支撑，待引入分配分析工具后决策。

## 实施顺序
1. `ScriptArray` 内部重构 + 数组相关原型方法 -> 完成后跑 `benchmarkArrays`。
2. `Callable` + `NumberValue` 缓存优化 -> 验证 `benchmarkClosure`、`benchmarkNumbers`。
3. 若时间允许，补充 CLR 调用与字符串池的二次优化。

## 验收指标
- `benchmarkArrays`: 分配降至 <10MB，时间下降。
- `benchmarkClosure`: 分配降至 <50MB，时间显著下降。
- 功能回归全绿（数组操作、闭包逻辑、CLR 调用）。

## 注意事项
- 确保数组与 Map 行为依旧符合脚本语义（未定义索引返回 `null`/`undefined`）。
- 变化涉及大范围内存结构，需慎重回归性能与功能。
- 将基准运行纳入 CI（记录执行结果，允许合理波动）。

