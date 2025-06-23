using UnityEngine;
using System.Collections.Generic;

namespace XIFramework.GameFramework
{


[AutoCreateSubsystem]
public class UISubsystem : XIGameSubSystem
{
    private readonly Stack<UIPanel> _panelStack = new();
    private Transform _uiRoot;
    
    public override void Initialize()
    {
        Debug.Log("1");
        // 创建UI根节点
        _uiRoot = new GameObject("UIRoot").transform;
        //_uiRoot.SetParent(XIGameInstance.Instance.transform);
        
        // 添加Canvas
        var canvas = _uiRoot.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
       // _uiRoot.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
       // _uiRoot.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }
    
    public T OpenPanel<T>(object data = null) where T : UIPanel
    {
        // 关闭当前面板
        if (_panelStack.Count > 0)
        {
            _panelStack.Peek().OnPause();
        }
        
        // 创建新面板
        var panel = CreatePanel<T>();
        panel.Open(data);
        _panelStack.Push(panel);
        return panel;
    }
    
    public void CloseTopPanel()
    {
        if (_panelStack.Count == 0) return;
        
        var panel = _panelStack.Pop();
        panel.Close();
        
        // 恢复上一个面板
        if (_panelStack.Count > 0)
        {
            _panelStack.Peek().OnResume();
        }
    }
    
    private T CreatePanel<T>() where T : UIPanel
    {
        // 获取资源系统
        var resourceSystem = GetSubsystem<ResourceSubSystem>();
        if (resourceSystem == null)
        {
            Debug.LogError("ResourceSubsystem not available");
            return null;
        }
        
        // 加载UI预制体
        var panelName = typeof(T).Name;
        var handle = resourceSystem.LoadAsset<GameObject>($"UI/{panelName}");
        var instance = Object.Instantiate(handle.Asset as GameObject, _uiRoot);
        
        var panel = instance.GetComponent<T>();
        if (panel == null)
        {
            Debug.LogError($"Panel {panelName} missing component {typeof(T).Name}");
            return null;
        }
        
        // 注入依赖
        Framework.Inject(panel);
        return panel;
    }
}

// UI面板基类
public class UIPanel : MonoBehaviour
{
    public virtual void Open(object data) { }
    public virtual void Close() { }
    public virtual void OnPause() { }
    public virtual void OnResume() { }
}
}