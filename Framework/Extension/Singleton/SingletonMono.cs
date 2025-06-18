using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XIFramework
{
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}