# 字节码优化设计草案

## 分析来源
- `code.txt`：当前编译器生成的 opcode dump，覆盖 `md5.as`, `timer.as`, `unit.as` 等案例。
- 重点观察：频繁出现的算术、闭包、迭代器片段，作为窥孔优化与指令复合的候选。

## 发现的模式
1. **常量加载链**
   - 模式：`PUSH_I32 k`、`PUSH_I64 k`、`PUSH_STRING s` 紧随算术或比较指令。
   - 优化思路：在 AST 层执行常量折叠，生成单一 `PUSH_*`；对重复常量应用常量池去重。

2. **闭包声明模板**
   ```
   PUSH_THIS
   CREATE_CLOSURE [addr]
   SET_THIS_PROPERTY <name>
   ```
   - 为函数、lambda 普遍存在。
   - 可新增 `CREATE_METHOD <name>` 复合指令或在编译阶段复用 `PUSH_THIS`，减少栈操作。

3. **变量声明 + Null 初始化**
   ```
   PUSH_NULL
   STORE_LOCAL <slot>
   ```
   或 `SET_THIS_PROPERTY`
   - 建议增加 `STORE_LOCAL_NULL`, `SET_THIS_PROPERTY_NULL` 指令，或者通过静态分析跳过无用初始化（当变量立即被赋值时）。

4. **迭代器循环模板**
   ```
   LOAD_LOCAL iterator
   ITERATOR_HAS_VALUE
   JUMP_IF_FALSE exit
   LOAD_LOCAL iterator
   ITERATOR_VALUE
   STORE_LOCAL var
   ...
   LOAD_LOCAL iterator
   ITERATOR_NEXT
   JUMP loopStart
   ```
   - 可设计 `FORIN_INIT`, `FORIN_NEXT`, `FORIN_END` 指令，每次循环减少 2~3 次栈操作。

5. **算术组合**
   - `LOAD_LOCAL a`, `LOAD_LOCAL b`, `ADD`/`SUBTRACT` 等组合频繁，结合常见的 `STORE_LOCAL`。
   - 可以引入 `ADD_LOCAL_LOCAL dst, a, b` 形式的新指令，在解释器层面直接访问寄存器。

6. **冗余 POP**
   - `CALL n` 后紧跟 `POP` (忽略返回值) 出现于语句调用。
   - 可将 `CALL` 默认视为返回留栈，新增 `CALL_VOID` 或窥孔转换 `CALL`→`CALL_VOID`。

## 优化策略
### 1. AST 常量折叠
- 在 `BinaryExpression`, `UnaryExpression`, `CompoundExpression` 上执行：
  - 纯常量子树求值。
  - 字符串拼接（`"a" + "b"` → `"ab"`）。
  - 布尔逻辑（短路可靠时）。
- 保留行列信息用于调试；必要时添加 `OptimizationInfo` 记录原节点。

### 2. 窥孔优化 Pass
- 执行时机：`InstructionBuilder.Build()` 前，对 `_instructions` 列表进行扫描。
- 匹配策略：滑动窗口（长度 2~5），比对 opcode 模式；命中后替换为优化指令。
- 需要新增 opcode 时，在 `OpCode` enum 中扩展，并保证 VM 解释器已有兼容 fallback。
- 提供可配置开关，允许禁用优化以加载旧字节码。

### 3. 统计与验证
- 新增 `tools/OpcodeAnalyzer`：读取 `InstructionBuilder` 输出，生成
  - 指令频率表
  - 模式匹配命中率
  - 优化前后指令数量/长度对比
- 自动化测试：使用现有脚本集，验证执行结果一致，同时记录性能指标。

## 开放事项
- 复合指令是否需要支持调试断点映射？可能需要记录原始指令范围。
- 与值类型化进程的交互：未来若局部变量改用 `ScriptDatum`，需评估 `ADD_LOCAL_LOCAL` 等新指令的实现复杂度。
- 字节码版本控制：考虑在输出文件头加入 `bytecode_version` 字段，保证运行时能够按需启/停优化解释路径。

## 下一步
1. 实现 AST 常量折叠原型（优先覆盖算术、字符串）。
2. 设计窥孔模式匹配描述结构及应用流程。
3. 编写 opcode 分析工具脚手架，并将 `code.txt` 数据导出为结构化格式。

