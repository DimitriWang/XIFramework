# Create XIFramework GameMode

## Description
快速创建符合 XIFramework 规范的 GameMode 类文件。

## Usage
`/create-gamemode`

## Instructions

当用户执行此 skill 时，按以下步骤执行：

### Step 1: 收集信息
询问用户以下信息：
1. **GameMode 名称**（如 `BattleGameMode`）
2. **对应的场景/关卡名称**
3. **需要哪些 per-world 服务**（GameState、PlayerController、PlayerState 等）
4. **放置的命名空间/目录**

### Step 2: 生成代码

```csharp
using XIFramework.GameFramework;

namespace [用户命名空间]
{
    public class [GameModeName] : GameMode
    {
        [Inject] private WorldContext _worldContext;

        public override void Initialize()
        {
            base.Initialize();
            // 注册 per-world 服务到 WorldContainer
            // _worldContext.WorldContainer.Register<IGameState, [GameStateName]>(isSingleton: false);
        }

        public override void OnWorldBegin()
        {
            base.OnWorldBegin();
            // 世界开始时的逻辑
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
- [ ] per-world 服务是否注册到 WorldContainer（而非 GlobalContainer）
- [ ] per-world 服务注册时是否使用 `isSingleton: false`
- [ ] 依赖注入是否使用 `[Inject]`
- [ ] 命名是否符合规范
