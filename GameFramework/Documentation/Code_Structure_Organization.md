# GameFramework 代码结构整理说明

## 整理后的目录结构

```
Assets/XIFramework/GameFramework/
├── Configuration/          # 配置相关文件
│   ├── FeatureConfig.cs
│   ├── DefaultFeatureConfig.cs
│   ├── PlatformFeatureConfig.cs
│   ├── WorldFeatureConfig.cs
│   └── ... (其他配置文件)
├── Core/                  # 核心框架文件（基本为空，仅保留asmdef）
├── Documentation/         # 文档文件
│   └── GameFramework_Documentation.md
├── Extensions/           # 扩展功能
├── Implementations/      # 接口实现类
│   ├── GameInstance.cs
│   ├── GameMode.cs
│   ├── GameState.cs
│   ├── GameWorld.cs
│   ├── PlayerController.cs
│   ├── PlayerState.cs
│   └── WorldContext.cs
├── Interfaces/           # 接口定义
│   ├── IGameInstance.cs
│   ├── IGameInstanceConfiguration.cs
│   ├── IGameMode.cs
│   ├── IGameState.cs
│   ├── IGameWorld.cs
│   ├── IPlayerController.cs
│   ├── IPlayerState.cs
│   ├── ISubsystem.cs
│   ├── IWorld.cs
│   ├── IWorldContext.cs
│   └── IWorldSettings.cs
├── Managers/            # 管理器类
│   ├── FeatureConfigManager.cs
│   └── ... (其他管理器)
├── ProviderEx/          # 资源提供者扩展
├── Resources/           # 资源文件
├── SubSystems/          # 子系统实现
│   ├── GameInstanceSubsystem.cs
│   ├── WorldSubsystem.cs
│   ├── DynamicSubsystem.cs
│   ├── DebugSubsystem.cs
│   └── ... (其他子系统)
├── XIFramework.GameFramework.asmdef
└── XIFramework.GameFramework.asmdef.meta
```

## 删除的旧文件

已删除以下旧的XI前缀文件：
- XIGameFeature.cs
- XIGameInstance.cs
- XIGameMode.cs
- XIGameState.cs
- XIGameSubSystem.cs
- XIPlayerController.cs
- XIPlayerState.cs
- XIWorld.cs
- XIWorldContext.cs

以及对应的.meta文件。

## 新的组织原则

1. **Interfaces/** - 存放所有接口定义文件
2. **Implementations/** - 存放接口的具体实现类
3. **Configuration/** - 存放配置相关类
4. **Managers/** - 存放管理器类
5. **SubSystems/** - 存放各种子系统实现
6. **Core/** - 保留为核心框架定义，目前主要用于asmdef文件

这种结构使得代码组织更加清晰，便于维护和扩展。