using System;
using UnityEngine;

namespace XIFramework
{
    public class ScriptableSingletonProperty<T> where T : ScriptableObject
    {
        public static Func<string, T> ScriptableLoader = Resources.Load<T>;
        
        private static T mInstance;

        public static T InstanceWithLoader(Func<string, T> loader)
        {
            ScriptableLoader = loader;
            return Instance;
        }
        
        public static T Instance
        {
            get
            {
                if (mInstance == null) mInstance = ScriptableLoader?.Invoke(typeof(T).Name);
                return mInstance;
            }
        }

        public static void Dispose()
        {
            if (SingletonFactory.IsUnitTestMode)
            {
                Resources.UnloadAsset(mInstance);
            }
            else
            {
                Resources.UnloadAsset(mInstance);
            }

            mInstance = null;
        }
    }
}