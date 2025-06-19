using System;
using UnityEngine;

namespace XIFramework
{
    public class ScriptableObjectSingleton<T> : ScriptableObject, ISingleton where T : ScriptableObjectSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = SingletonFactory.CreateScriptableObjectSingleton<T>();
                    instance.OnSingletonInit();
                }
                return instance;
            }
        }
        public virtual void OnDestroy()
        {
            instance = null;
        }
        public void OnDisable()
        {
            instance = null;
        }
        public void OnSingletonInit()
        {
        }
    }
}