using XIFramework.Editor;

namespace XIFramework
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    public static class TypeCacheRefresher
    {
        [MenuItem("Tools/Refresh Type Cache")]
        public static void RefreshTypeCache()
        {
            TypeSelectorPropertyDrawer.ResetCache();
            Debug.Log("Type cache refreshed successfully!");
        }
    }
#endif
}