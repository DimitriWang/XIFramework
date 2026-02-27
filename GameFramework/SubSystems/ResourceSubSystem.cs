using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 资源子系统 - 管理资源加载与释放
    /// 自动创建在GameInstance级别
    /// </summary>
    [AutoCreateSubsystem(Priority = -50)]
    public class ResourceSubSystem : GameInstanceSubsystem, IAsyncInitialization, IAsyncShutdown
    {
        private IResourceProvider _resourceProvider;
        
        public override void Initialize()
        {
            base.Initialize();
        }
        
        public async UniTask InitializeAsync()
        {
            // 根据平台选择资源提供者
#if UNITY_EDITOR
            _resourceProvider = new EditorResourceProvider();
#else
            _resourceProvider = new YooAssetResourceProvider();
#endif
            await _resourceProvider.InitializeAsync();
            Debug.Log("[ResourceSubSystem] Resource provider initialized");
        }
        
        public async UniTask<AssetHandle> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object
        {
            return await _resourceProvider.LoadAssetAsync<T>(assetPath);
        }
        
        public AssetHandle LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return _resourceProvider.LoadAsset<T>(assetPath);
        }
        
        public async UniTask ShutdownAsync()
        {
            if (_resourceProvider != null)
            {
                await _resourceProvider.ShutdownAsync();
            }
        }
        
        public override void Update(float deltaTime)
        {
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
        }
    }

    /// <summary>
    /// 资源提供者接口
    /// </summary>
    public interface IResourceProvider
    {
        UniTask InitializeAsync();
        AssetHandle LoadAsset<T>(string assetPath) where T : UnityEngine.Object;
        UniTask<AssetHandle> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object;
        UniTask ShutdownAsync();
    }

    /// <summary>
    /// 资源句柄 - 带引用计数的资源包装
    /// </summary>
    public class AssetHandle : IDisposable
    {
        private UnityEngine.Object _asset;
        private Action<UnityEngine.Object> _releaseAction;
        private int _refCount = 1;
        
        public UnityEngine.Object Asset => _asset;
        
        public AssetHandle(UnityEngine.Object asset, Action<UnityEngine.Object> releaseAction)
        {
            _asset = asset;
            _releaseAction = releaseAction;
        }
        
        public void Retain()
        {
            Interlocked.Increment(ref _refCount);
        }
        
        public void Release()
        {
            if (Interlocked.Decrement(ref _refCount) <= 0)
            {
                _releaseAction?.Invoke(_asset);
                _asset = null;
                _releaseAction = null;
            }
        }
        
        public void Dispose() => Release();
    }
}
