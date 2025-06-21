using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public class EditorResourceProvider : IResourceProvider
    {
        public Dictionary<string, AssetHandle> _loadedAssets = new Dictionary<string, AssetHandle>();
        public async UniTask InitializeAsync() { await UniTask.CompletedTask; }
        public AssetHandle LoadAsset<T>(string assetPath) where T : Object
        {
            if (!_loadedAssets.TryGetValue(assetPath, out var asset))
            {
                asset = new AssetHandle(Resources.Load<T>(assetPath), (obj) => { });
                _loadedAssets.Add(assetPath, asset);
            }
            return asset;
        }
        public async UniTask<AssetHandle> LoadAssetAsync<T>(string assetPath) where T : Object
        {
            await UniTask.CompletedTask;
            if (!_loadedAssets.TryGetValue(assetPath, out var asset))
            {
                var asyncOp = Resources.LoadAsync<T>(assetPath);
                await asyncOp;
                asset = new AssetHandle(asyncOp.asset, (obj) => { });
                _loadedAssets.Add(assetPath, asset);
            }
            return asset;
        }
        public async UniTask ShutdownAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}