using UnityEditor;


namespace XIFramework.Editor
{
    /// <summary>
    /// 框架编辑器 助手类
    /// </summary>
    public class XIEditorUtility
    {
        [InitializeOnLoadMethod]
        static void OnLoaded()
        {
            DeltaTime = LastTimeSinceStartup = 0;
            EditorApplication.update += SetEditorDeltaTime;
        }

        public static double DeltaTime { get; private set; } = 0;
        static double LastTimeSinceStartup = 0f;
        static void SetEditorDeltaTime()
        {
            if (LastTimeSinceStartup == 0f)
            {
                LastTimeSinceStartup = EditorApplication.timeSinceStartup;
            }
            DeltaTime = EditorApplication.timeSinceStartup - LastTimeSinceStartup;
            LastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }
    }
    //已实现基础功能，Editor下统一DeltaTime使用
    //TODO： 后续框架实作功能 
    //1. Editor 下编辑器框架结构 
    //2. 自定生命周期，接管Editor下所有内容
    //3. 自定Window UIElement 界面窗口类 结构框架内容，类似 UI框架的结构进行内容拓展，
    //适用于全Editor扩展内容
}