using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using YooAsset;
using Object = UnityEngine.Object;

namespace XIFramework.GameFramework
{
    public enum YooAssetModeType
    {
        EditorSimulate, //编辑器模拟
        Offline, //离线模式
        Host, //在线联网模式
    }
    
    public class YooAssetResourceProvider : IResourceProvider
    {
        
        public static YooAssetModeType Mode = YooAssetModeType.EditorSimulate;
        
        public Dictionary<string, AssetHandle> _loadedAssets = new Dictionary<string, AssetHandle>();
        private ResourcePackage package;
        public async UniTask InitializeAsync()
        {
            YooAssets.Initialize();
            
            package = YooAssets.CreatePackage("DefaultPackage");
            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);
            //YooAsset中需要初始化 且分编辑器下和运行时
            await InitializeYooAsset();
        }
        public AssetHandle LoadAsset<T>(string assetPath) where T : Object
        {
            if (!_loadedAssets.TryGetValue(assetPath, out var asset))
            {
                var yooasset = package.LoadAssetSync<T>(assetPath);
                asset = new AssetHandle(yooasset.AssetObject, (asset) =>
                {
                    package.TryUnloadUnusedAsset(yooasset.GetAssetInfo());
                    //package.ReleaseAsset(asset);
                    _loadedAssets.Remove(assetPath);
                });
            }
            
            return asset;
        }
        public async UniTask<AssetHandle> LoadAssetAsync<T>(string assetPath) where T : Object
        {
            if (!_loadedAssets.TryGetValue(assetPath, out var asset))
            {
                var yooasset = package.LoadAssetAsync<T>(assetPath);

                await yooasset.ToUniTask();
                
                asset = new AssetHandle(yooasset.AssetObject, (asset) =>
                {
                    package.TryUnloadUnusedAsset(yooasset.GetAssetInfo());
                    //package.ReleaseAsset(asset);
                    _loadedAssets.Remove(assetPath);
                });
            }
            return asset;
        }
        
        
#if !UNITY_EDITOR
        private IEnumerator InitializeYooAsset()
        {
            var initParameters = new OfflinePlayModeParameters();
            yield return package.InitializeAsync(initParameters);
            StartOpenPanel().Forget();package = YooAssets.CreatePackage("DefaultPackage");
            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);
            //YooAsset中需要初始化 且分编辑器下和运行时
            StartCoroutine(InitializeYooAsset());
        }

#else
        
        private async UniTask InitializeYooAsset()
        {
            var initParameters = new EditorSimulateModeParameters();
            string packageRoot = string.Empty;
#if UNITY_EDITOR
            packageRoot = UnityEditor.EditorPrefs.GetString("DefaultPackage");
#endif
            if (Directory.Exists(packageRoot) == false)
                throw new Exception($"Not found package root : {packageRoot}");

            var package = YooAssets.CreatePackage("DefaultPackage");

            // 初始化资源包
            var initParams = new EditorSimulateModeParameters();
            initParams.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initializeOp = package.InitializeAsync(initParams);
            await initializeOp;
            if (initializeOp.Status != EOperationStatus.Succeed)
                Debug.LogError(initializeOp.Error);
            Assert.AreEqual(EOperationStatus.Succeed, initializeOp.Status);

            // 请求资源版本
            var requetVersionOp = package.RequestPackageVersionAsync();
            await requetVersionOp;
            if (requetVersionOp.Status != EOperationStatus.Succeed)
                Debug.LogError(requetVersionOp.Error);
            Assert.AreEqual(EOperationStatus.Succeed, requetVersionOp.Status);

            // 更新资源清单
            var updateManifestOp = package.UpdatePackageManifestAsync(requetVersionOp.PackageVersion);
            await updateManifestOp;
            if (updateManifestOp.Status != EOperationStatus.Succeed)
                Debug.LogError(updateManifestOp.Error);
            Assert.AreEqual(EOperationStatus.Succeed, updateManifestOp.Status);
        }
        
#endif
        public async UniTask ShutdownAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}