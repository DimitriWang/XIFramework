using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
    [AutoCreateSubsystem]
    public class ResourceSubSystem : XIGameSubSystem, IAsyncInitialization, IAsyncShutdown
    {
        private IResourceProvider _resourceProvider;
        public async UniTask InitializeAsync()
        {
            // 根据平台选择资源提供者
#if UNITY_EDITOR
            _resourceProvider = new EditorResourceProvider();
#else
        _resourceProvider = new AddressablesResourceProvider();
#endif
            await _resourceProvider.InitializeAsync();
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
            await _resourceProvider.ShutdownAsync();
        }
    }

// 资源提供者接口
    public interface IResourceProvider
    {
        UniTask InitializeAsync();
        AssetHandle LoadAsset<T>(string assetPath) where T : UnityEngine.Object;
        UniTask<AssetHandle> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object;
        UniTask ShutdownAsync();
    }

// 资源句柄
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