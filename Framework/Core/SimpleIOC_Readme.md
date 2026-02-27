# 简化版IOC容器使用说明

## 概述
这是一个为XIFramework设计的简化版IOC容器，旨在替代复杂的依赖注入系统，提供更简单、更直观的服务定位功能。

## 核心特性
- ✅ 简单易用的API设计
- ✅ 支持实例注册和类型映射
- ✅ 自动实例创建
- ✅ 静态辅助方法
- ✅ 线程安全的单例模式

## 使用方法

### 1. 基本注册和解析
```csharp
// 注册实例
var gameState = new GameState();
SimpleIOCContainer.RegisterService<IGameState>(gameState);

// 注册类型映射
SimpleIOCContainer.RegisterService<IGameState, GameState>();

// 解析服务
var resolvedGameState = SimpleIOCContainer.GetService<IGameState>();

// 检查服务是否存在
bool hasService = SimpleIOCContainer.HasService<IGameState>();
```

### 2. 在GameInstance中的使用
```csharp
public class MyGameInstance : MonoBehaviour
{
    private void Start()
    {
        // 初始化容器
        InitializeContainer();
        
        // 使用容器解析服务
        var gameState = SimpleIOCContainer.GetService<IGameState>();
    }
    
    private void InitializeContainer()
    {
        // 注册核心服务
        SimpleIOCContainer.RegisterService<IGameState, GameState>();
        SimpleIOCContainer.RegisterService<IPlayerState, PlayerState>();
        SimpleIOCContainer.RegisterService<IPlayerController, PlayerController>();
    }
}
```

### 3. 子系统管理
```csharp
// 注册子系统
var debugSubsystem = new DebugSubsystem();
SimpleIOCContainer.RegisterService<ISubsystem>(debugSubsystem);

// 获取所有子系统
var allSubsystems = SimpleIOCContainer.GetAllServices<ISubsystem>();

// 获取特定类型的子系统
var debugSystem = SimpleIOCContainer.GetService<DebugSubsystem>();
```

## 与原IOC系统的对比

| 特性 | 原IOC容器 | 简化IOC容器 |
|------|-----------|-------------|
| 复杂度 | 高 | 低 |
| 循环依赖检测 | 有 | 无 |
| 构造函数注入 | 支持 | 不支持 |
| 字段/属性注入 | 支持 | 不支持 |
| 子容器支持 | 支持 | 不支持 |
| 性能 | 中等 | 高 |
| 易用性 | 复杂 | 简单 |

## 适用场景
- 🎯 快速原型开发
- 🎯 学习和教学用途
- 🎯 小型项目
- 🎯 不需要复杂依赖注入的场景

## 注意事项
1. 不支持构造函数注入
2. 不支持字段/属性自动注入
3. 不支持循环依赖检测
4. 不支持子容器
5. 适用于相对简单的依赖关系

## 迁移指南
如果您要从原IOC容器迁移到简化版：

1. **替换容器初始化**：
   ```csharp
   // 原来
   var container = new XIFrameworkContainer();
   
   // 现在
   var container = SimpleIOCContainer.Instance;
   ```

2. **替换注册方法**：
   ```csharp
   // 原来
   container.Register<IGameState, GameState>();
   
   // 现在
   SimpleIOCContainer.RegisterService<IGameState, GameState>();
   ```

3. **替换解析方法**：
   ```csharp
   // 原来
   var gameState = container.Resolve<IGameState>();
   
   // 现在
   var gameState = SimpleIOCContainer.GetService<IGameState>();
   ```

## 性能优化建议
1. 尽早注册常用服务
2. 避免频繁的动态解析
3. 合理使用单例模式
4. 缓存经常使用的服务引用