using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XIFramework
{
    public class PrefabSingletonProperty<T> where T : MonoBehaviour
    {
        public static Func<string, GameObject> PrefabLoader = Resources.Load<GameObject>;
        
        private static T mInstance;

        public static T InstanceWithLoader(Func<string, GameObject> loader)
        {
            PrefabLoader = loader;
            return Instance;
        }
        
        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = Object.FindObjectOfType<T>();
                    if (!mInstance)
                    {
                        var prefab = PrefabLoader?.Invoke(typeof(T).Name);
                        if (prefab)
                        {
                            mInstance = prefab.Instantiate().GetComponent<T>();
                            mInstance.DontDestroyOnLoad();
                        }
                    }
                }
                return mInstance;
            }
        }
        
        public static void Dispose()
        {
            if (SingletonFactory.IsUnitTestMode)
            {
                UnityEngine.Object.DestroyImmediate(mInstance.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(mInstance.gameObject);
            }

            mInstance = null;
        }
    }
}