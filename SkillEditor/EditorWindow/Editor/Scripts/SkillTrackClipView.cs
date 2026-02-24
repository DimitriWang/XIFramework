using UnityEditor;
using UnityEngine.UIElements;

namespace XIFramework.SkillEditor
{
    public class SkillTrackClipView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SkillTrackClipView, UxmlTraits> { }
        
        
        public SkillTrackClipView() { }

        public SkillTrackClipView(string path) : base()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            visualTree.CloneTree(this);
            
            //
            
        }
        
        
        VisualElement m_Content;
        VisualElement m_LeftMixer;
        VisualElement m_RightMixer;
        VisualElement m_Title;
        Label m_ClipName;
        VisualElement m_LeftClipIn;
        VisualElement m_RightClipIn;
        VisualElement m_BottomLine;
        VisualElement m_DrawBox;
    }
}

