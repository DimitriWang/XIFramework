# Create XIFramework Subsystem

## Description
快速创建符合 XIFramework 规范的子系统（Subsystem）类文件。

## Usage
`/create-subsystem`

## Instructions

当用户执行此 skill 时，按以下步骤执行：

### Step 1: 收集信息
询问用户以下信息：
1. **子系统名称**（如 `AudioSubsystem`）
2. **生命周期类型**：
   - `GameInstance` → 继承 `GameInstanceSubsystem`，随整个游戏会话存活
   - `World` → 继承 `WorldSubsystem`，随 WorldContext 存活
   - `Dynamic` → 继承 `DynamicSubsystem`，手动控制生命周期
3. **是否需要自动注册**（AutoCreateSubsystem），如果需要，询问优先级数值（数值越小越先初始化）
4. **需要注入哪些依赖**（如 `GameInstance`、`WorldContext` 等）
5. **放置的命名空间/目录**

### Step 2: 生成代码

根据收集的信息生成以下结构的 C# 类：

```csharp
using XIFramework.GameFramework;
// 根据需要添加其他 using

namespace [用户命名空间]
{
    // 如果需要自动注册，添加此特性
    [AutoCreateSubsystem(Priority = [优先级])]
    public class [SubsystemName] : [GameInstanceSubsystem/WorldSubsystem/DynamicSubsystem]
    {
        // 使用 [Inject] 注入所需依赖
        [Inject] private GameInstance _gameInstance;
        // ... 其他注入字段

        public override void Initialize()
        {
            base.Initialize();
            // 初始化逻辑
        }

        public override void Shutdown()
        {
            // 清理逻辑
            base.Shutdown();
        }
    }
}
```

### Step 3: 规范检查
生成后自动检查：
- [ ] 依赖方向是否正确（不能反向依赖）
- [ ] 是否使用 `[Inject]` 而非手动赋值
- [ ] 全局服务是否注册到正确的容器
- [ ] 命名是否符合规范
