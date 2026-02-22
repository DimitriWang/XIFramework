using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

namespace XIFramework.SkillEditor
{
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
            
            string uxmlPath = SkillEditorAssistant.FindUXMLPath(this);
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            
            VisualElement labelFromUXML = visualTree.Instantiate();
            
            root.Add(labelFromUXML);

            InitTopMenu();

            InitTimerShaft();

            InitConsole();

            InitContent();

            if (skillConfig != null)
            {
                SkillConfigObjectField.value = skillConfig;
                
            }
            else
            {
                CurrentFrameCount = 100;
            }

            CurrentSelectFrameIndex = 0;
        }
        
        private void OnGUI()
        {
        }


        #region TopMenu

        private const string SkillEditorScenePath = "Assets/XIFramework/SkillEditor/Scene/SkillEditorScene.unity";
        private const string previewCharacterParentPath = "PreviewCharacterRoot";

        private string preScenePath;

        private Button LoadEditorSceneButton;
        private Button LoadOldSceneButton;
        private Button SkillBasicButton;

        private ObjectField PreviewCharacterPrefabObjectField;
        private ObjectField SkillConfigObjectField;

        private GameObject currentPreviewCharacterObj;

        private void InitTopMenu()
        {
            LoadEditorSceneButton = root.Q<Button>(nameof(LoadEditorSceneButton));
            LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;

            LoadOldSceneButton = root.Q<Button>(nameof(LoadOldSceneButton));
            LoadOldSceneButton.clicked += LoadOldSceneButtonClick;

            SkillBasicButton = root.Q<Button>(nameof(SkillBasicButton));
            SkillBasicButton.clicked += SkillBasicButtonClick;

            PreviewCharacterPrefabObjectField = root.Q<ObjectField>(nameof(PreviewCharacterPrefabObjectField));
            PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldChanged);


            SkillConfigObjectField = root.Q<ObjectField>(nameof(SkillConfigObjectField));
            SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldChanged);
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
            if (!string.IsNullOrEmpty(preScenePath))
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
            if (skillConfig != null)
            {
                Selection.activeObject = skillConfig;
            }
        }

        /// <summary>
        /// 角色预制体修改
        /// </summary>
        /// <param name="evt"></param>
        private void PreviewCharacterPrefabObjectFieldChanged(ChangeEvent<Object> evt)
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            if (currentScenePath != SkillEditorScenePath)
            {
                return;
            }

            if (currentPreviewCharacterObj != null) DestroyImmediate(currentPreviewCharacterObj);
            Transform parent = GameObject.Find(previewCharacterParentPath).transform;
            if (parent != null && parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }

            if (evt.newValue != null)
            {
                currentPreviewCharacterObj = Instantiate((evt.newValue as GameObject), Vector3.zero, Quaternion.identity, parent);
                currentPreviewCharacterObj.transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 技能配置修改
        /// </summary>
        /// <param name="evt"></param>
        private void SkillConfigObjectFieldChanged(ChangeEvent<Object> evt)
        {
            skillConfig = evt.newValue as SkillConfig;

            CurrentFrameCount = skillConfig.FrameCount;
            //TODO: 重新绘制轨道
        }

        #endregion

        #region TimeShaft

        private IMGUIContainer timerShaft;
        
        private IMGUIContainer selectLine;

        private VisualElement contentContainer;

        private VisualElement contentViewPort;

        private int currentSelectFrameIndex = -1;
        private int CurrentSelectFrameIndex
        {
            get => currentSelectFrameIndex;
            set
            {
                if (currentSelectFrameIndex == value) return;
                //如果超出范围 更新，最大帧率
                if (value > CurrentFrameCount) CurrentFrameCount = value;
                currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentFrameCount);
                CurrentFrameTextField.value = currentSelectFrameIndex.ToString();
                MarkTimeShaftView();
            }
        }

        //当前内容区域的偏移坐标
        private float contentOffsetPos
        {
            get => Mathf.Abs(contentContainer.transform.position.x);
        }
        
        private float currentSelectFramePos
        {
            get => currentSelectFrameIndex * skillEditorConfig.frameUnitWidth;
        }

        private int currentFrameCount;
        
        public int CurrentFrameCount
        {
            get => currentFrameCount;
            set
            {
                if (currentFrameCount == value) return;
                currentFrameCount = value;
                FrameCountTextField.value = currentFrameCount.ToString();
                //同步给SkillConfig
                if (skillConfig != null)
                {
                    skillConfig.FrameCount = currentFrameCount;
                    SaveConfig();
                }

                UpdatContentSize();
            }
        }
        
        private bool timerShaftOnMouseEnter = false;
        private ScrollView TrackContentView;
        private void InitTimerShaft()
        {
            TrackContentView = root.Q<ScrollView>(nameof(TrackContentView));
            contentContainer = TrackContentView.Q<VisualElement>("unity-content-container");
            contentViewPort = TrackContentView.Q<VisualElement>("unity-content-viewport");
            
            timerShaft = root.Q<IMGUIContainer>("TimerShaft");
            selectLine = root.Q<IMGUIContainer>("SelectLine");
            
            
            timerShaft.onGUIHandler = DrawTimerShaft;
            
            timerShaft.RegisterCallback<WheelEvent>(OnTimerShaftWheelEvent);
            timerShaft.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            timerShaft.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
            timerShaft.RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
            timerShaft.RegisterCallback<MouseOutEvent>(OnMouseOutEvent);
            
            selectLine.onGUIHandler = DrawSelectLine;
            
        }
        

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            timerShaftOnMouseEnter = true;
            CurrentSelectFrameIndex = GetFrameIndexByMousePos(evt.localMousePosition.x);
        }
        private void OnMouseMoveEvent(MouseMoveEvent evt)
        {
            if (timerShaftOnMouseEnter)
            {
                CurrentSelectFrameIndex = GetFrameIndexByMousePos(evt.localMousePosition.x);
            }
        }
        
        private void OnMouseUpEvent(MouseUpEvent evt)
        {
            timerShaftOnMouseEnter = false;
        }

        private void OnMouseOutEvent(MouseOutEvent evt)
        {
            timerShaftOnMouseEnter = false;
        }
        
        private int GetFrameIndexByMousePos(float x)
        {
            float pos = x + contentOffsetPos;

            return Mathf.RoundToInt(pos / skillEditorConfig.frameUnitWidth);
        }

        private void DrawTimerShaft()
        {
            Handles.BeginGUI();
            Handles.color = new Color(0.7058f, 0.7058f, 0.7058f, 1);
            Rect rect = timerShaft.contentRect;
            int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUnitWidth);
            //计算绘制起点偏移
            float startOffset = 0;
            if (index > 0) startOffset = skillEditorConfig.frameUnitWidth - (contentOffsetPos % skillEditorConfig.frameUnitWidth);
            
            //tickstep = 10 + 1  - (100 / 10)
            int tickStep =  SkillEditorConfig.maxFrameWidthLv + 1 - (skillEditorConfig.frameUnitWidth / SkillEditorConfig.standFrameUnitWidth);
            tickStep = tickStep / 2;
            tickStep = tickStep == 0 ? 1 : tickStep;
            for (float i = 0; i < rect.width; i += skillEditorConfig.frameUnitWidth)
            {
                //满足长线条 步长
                if (index % tickStep == 0)
                {
                    Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                    string indexStr = index.ToString();
                    GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
                }
                else
                {
                    Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
                }

                index += 1;
            }


            Handles.EndGUI();
        }

        private void OnTimerShaftWheelEvent(WheelEvent evt)
        {
            int deltaY = (int)evt.delta.y;
            skillEditorConfig.frameUnitWidth =
                Mathf.Clamp(skillEditorConfig.frameUnitWidth - deltaY,
                    SkillEditorConfig.standFrameUnitWidth,
                    SkillEditorConfig.maxFrameWidthLv * SkillEditorConfig.standFrameUnitWidth);


            MarkTimeShaftView();

            UpdatContentSize();
        }
        
        private void DrawSelectLine()
        {
            //判断当前选中帧是否在视图范围内
            if (currentSelectFramePos >= contentOffsetPos)
            {
                Handles.BeginGUI();
                Handles.color = new Color(0.7058f, 0.7058f, 0.7058f, 1);
                float x = currentSelectFramePos - contentOffsetPos;
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, contentViewPort.contentRect.height + timerShaft.contentRect.height));
                Handles.EndGUI();
            }
        }

        public void MarkTimeShaftView()
        {
            timerShaft.MarkDirtyRepaint();
            selectLine.MarkDirtyRepaint();
        }

        #endregion

        #region Console

        private Button PreviouFrameButton;
        private Button PlayButton;
        private Button NextFrameButton;
        private TextField CurrentFrameTextField;
        private TextField FrameCountTextField;
        private void InitConsole()
        {
            PreviouFrameButton = root.Q<Button>("PreviouFrameButton");
            PlayButton = root.Q<Button>("PlayButton");
            NextFrameButton = root.Q<Button>("NextFrameButton");
            
            CurrentFrameTextField = root.Q<TextField>("CurrentFrameTextField");
            FrameCountTextField = root.Q<TextField>("FrameCountTextField");
            
            PreviouFrameButton.clicked += PreviouFrameButtonClick;
            PlayButton.clicked += PlayButtonClick;
            NextFrameButton.clicked += NextFrameButtonClick;

            CurrentFrameTextField.RegisterValueChangedCallback(CurrentFrameTextFieldChanged);
            FrameCountTextField.RegisterValueChangedCallback(FrameCountTextFieldChanged);
        }


        private void PreviouFrameButtonClick()
        {
            if (CurrentSelectFrameIndex > 0)
            {
                CurrentSelectFrameIndex -= 1;
            }
        }
        
        private void PlayButtonClick()
        {
            //TODO: 
        }
        
        private void NextFrameButtonClick()
        {
            CurrentSelectFrameIndex += 1;
        }
        
        private void CurrentFrameTextFieldChanged(ChangeEvent<string> evt)
        {
            CurrentSelectFrameIndex = int.Parse(evt.newValue);
        }

        private void FrameCountTextFieldChanged(ChangeEvent<string> evt)
        {
            CurrentFrameCount = int.Parse(evt.newValue);
        }

        #endregion

        #region Config

        private SkillConfig skillConfig;
        private SkillEditorConfig skillEditorConfig = new SkillEditorConfig();


        private void SaveConfig()
        {
            if (skillConfig != null)
            {
                EditorUtility.SetDirty(skillConfig);
                AssetDatabase.SaveAssetIfDirty(skillConfig);
            }
        }
        #endregion

        #region Track

        private VisualElement ContentListView;

        private VisualElement TrackMenuParent;
        private void InitContent()
        {
            ContentListView = TrackContentView.Q<VisualElement>(nameof(ContentListView));
            TrackMenuParent = root.Q<VisualElement>("TrackMenu");
            UpdatContentSize();
            InitAnimationTrack();
        }
        
        private void UpdatContentSize()
        {
            ContentListView.style.width = skillEditorConfig.frameUnitWidth * CurrentFrameCount;
        }

        private void InitAnimationTrack()
        {
            AnimationTrackView animationTrackView = new AnimationTrackView();
            animationTrackView.Init(TrackMenuParent, ContentListView);
        }

        #endregion
    }

    public class SkillEditorConfig
    {
        public const int standFrameUnitWidth = 10; //帧单位宽度
        public const int maxFrameWidthLv = 10;
        public int frameUnitWidth = 10;
    }
}