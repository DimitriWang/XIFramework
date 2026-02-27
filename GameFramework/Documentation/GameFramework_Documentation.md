# XIFramework GameFramework 重构文档

## 概述

本文档介绍了基于Unreal Engine GamePlay框架理念重构的XIFramework GameFramework模块。该重构旨在解决原有设计中过度依赖Inject属性导致的架构不清晰问题，并提供更加清晰、模块化的游戏框架结构。

## 设计理念

### 核心原则
1. **遵循Unreal World概念** - 严格按照Unreal Engine的World、GameMode、GameState等核心概念设计
2. **清晰的职责分离** - 每个组件都有明确的职责和边界
3. **最小化依赖注入** - 减少不必要的[Inject]标记，提高代码可读性
4. **一个文件一个类** - 严格遵守每个脚本文件只包含一个类/接口的规范

### 架构层次
```
IWorld (基础世界接口)
└── IGameWorld (游戏世界接口)
    └── GameWorld (游戏世界实现)

IWorldContext (世界上下文接口)
└── WorldContext (世界上下文实现)

IGameInstance (游戏实例接口)
└── GameInstance (游戏实例实现)

ISubsystem (子系统接口)
├── GameInstanceSubsystem (游戏实例子系统)
├── WorldSubsystem (世界子系统)
└── DynamicSubsystem (动态子系统)
```

## 核心组件详解

### 1. IWorld & GameWorld
**职责**：管理游戏世界的基础功能
- 生命周期管理（Initialize → Activate → Update → Deactivate → Shutdown）
- 时间管理
- 基础状态跟踪

### 2. IGameWorld & GameWorld
**职责**：游戏专用世界功能
- 关卡管理（持久关卡、子关卡）
- 游戏模式管理
- 游戏状态管理
- 玩家管理

### 3. IWorldContext & WorldContext
**职责**：世界上下文管理
- 连接GameInstance和GameWorld
- 管理世界生命周期
- 协调子系统初始化

### 4. IGameInstance & GameInstance
**职责**：游戏实例管理
- 管理多个世界上下文
- 全局配置管理
- 全局子系统管理
- Unity生命周期集成

### 5. 游戏模式系统
- **IGameMode/GameMode**：游戏规则控制
- **IGameState/GameState**：游戏全局状态管理
- **IPlayerController/PlayerController**：玩家行为控制
- **IPlayerState/PlayerState**：玩家数据存储

### 6. 子系统架构
三种生命周期类型：
- **GameInstance级别**：与游戏实例同生命周期
- **World级别**：与世界同生命周期  
- **Dynamic级别**：可动态控制生命周期

## 使用示例

### 基础设置
```csharp
// 在场景中添加GameInstance组件
public class Bootstrap : MonoBehaviour
{
    void Start()
    {
        // 获取GameInstance引用
        var gameInstance = FindObjectOfType<GameInstance>();
        
        // 初始化世界上下文
        gameInstance.InitializeWorldContext("MainWorld");
        
        // 激活世界
        gameInstance.SetActiveWorldContext("MainWorld");
    }
}
```

### 自定义GameMode
```csharp
public class MyGameMode : GameMode
{
    public override void StartGame()
    {
        base.StartGame();
        // 自定义游戏启动逻辑
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        // 自定义游戏更新逻辑
    }
}
```

### 创建子系统
```csharp
[AutoCreateSubsystem]
public class MyWorldSubsystem : WorldSubsystem
{
    public override void Initialize()
    {
        base.Initialize();
        // 初始化逻辑
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        // 更新逻辑
    }
}
```

## 配置系统

### 特性配置
提供了三种配置类型：
1. **DefaultFeatureConfig**：默认全局配置
2. **PlatformFeatureConfig**：平台特定配置
3. **WorldFeatureConfig**：世界特定配置

### 使用配置
```csharp
// 创建配置资源
var config = ScriptableObject.CreateInstance<DefaultFeatureConfig>();
config.TargetFrameRate = 60;
config.EnablePostProcessing = true;

// 应用配置
var configManager = new FeatureConfigManager();
configManager.LoadConfig(config);
configManager.ApplyAllConfigs(gameWorld);
```

## 重构改进点

### 相比原设计的优势
1. **清晰的架构层次**：严格按照Unreal概念设计，职责分明
2. **减少依赖注入**：避免过度使用[Inject]，提高代码可读性
3. **标准化接口**：所有核心组件都有对应的接口定义
4. **完善的生命周期**：提供完整的初始化、激活、更新、停用、关闭流程
5. **灵活的子系统**：支持不同生命周期的子系统管理

### 性能优化
1. **减少反射调用**：优化IOC容器和服务定位器
2. **合理的对象创建时机**：按需创建对象，避免内存浪费
3. **高效的配置管理**：使用缓存机制提高配置查找效率

## 迁移指南

### 从旧版本迁移
1. **替换核心类**：使用新的接口和实现类
2. **更新依赖注入**：减少[Inject]标记的使用
3. **重构配置系统**：迁移到新的特性配置系统
4. **调整子系统**：按照新的生命周期类型重构子系统

### 注意事项
- 确保所有自定义GameMode继承新的GameMode基类
- 更新所有子系统以适应新的生命周期管理
- 重新设计配置资源以匹配新的配置系统

## 最佳实践

### 代码组织
- 每个文件只包含一个类或接口
- 使用命名空间合理组织代码
- 遵循统一的命名规范

### 性能考虑
- 合理使用子系统生命周期
- 避免在Update中进行heavy计算
- 及时释放不需要的资源

### 调试建议
- 充分利用DebugSubsystem
- 合理设置日志级别
- 使用Unity Profiler监控性能

## 未来扩展

### 计划功能
- 网络同步支持
- 更丰富的配置选项
- 可视化编辑器工具
- 性能分析工具集成

### 扩展建议
- 可以轻松添加新的子系统类型
- 支持插件化架构
- 提供更多的Hook点供自定义扩展

---
*文档版本：1.0*
*最后更新：2024年*