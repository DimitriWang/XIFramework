using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class SkillEditorWindow : EditorWindow
{
    [MenuItem("SkillEditor/SkillEditorWindow")]
    public static void ShowExample()
    {
        SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
        wnd.titleContent = new GUIContent("SkillEditorWindow");
    }

    private VisualElement root;
    
    public void CreateGUI()
    {
        root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SkillEditor/EditorWindow/Editor/Asset/VisualTree/SkillEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        InitTopMenu();
    }

    #region TopMenu

    private const string SkillEditorScenePath = "Assets/SkillEditor/Scene/SkillEditorScene.unity";

    private string preScenePath;
    
    private Button LoadEditorSceneButton;
    private Button LoadOldSceneButton;
    private Button SkillBasicButton;

    private void InitTopMenu()
    {
        LoadEditorSceneButton = root.Q<Button>(nameof(LoadEditorSceneButton));
        LoadOldSceneButton = root.Q<Button>(nameof(LoadOldSceneButton));
        SkillBasicButton = root.Q<Button>(nameof(SkillBasicButton));
        
        LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;
        LoadOldSceneButton.clicked += LoadOldSceneButtonClick;
        SkillBasicButton.clicked += SkillBasicButtonClick;
    }
    
    /// <summary>
    /// 加载编辑器场景
    /// </summary>
    private void LoadEditorSceneButtonClick()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
       
        if (currentScenePath == SkillEditorScenePath)
        {
            return;
        }
        else
        {
            preScenePath = currentScenePath;
        }
        EditorSceneManager.OpenScene(SkillEditorScenePath);
    }
    
    /// <summary>
    /// 回归旧场景
    /// </summary>
    private void LoadOldSceneButtonClick()
    {
        if (string.IsNullOrEmpty(preScenePath))
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            
            if (currentScenePath == preScenePath)
            {
                return;
            }
            EditorSceneManager.OpenScene(preScenePath);
        }
        else
        {
            Debug.LogWarning("PreScenePath is Null Or Empty");
        }
    }
    
    /// <summary>
    /// 查看技能基本信息
    /// </summary>
    private void SkillBasicButtonClick()
    {
    }

    #endregion
}