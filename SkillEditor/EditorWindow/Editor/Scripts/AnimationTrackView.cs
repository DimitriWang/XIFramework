using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using XIFramework.SkillEditor;

public class AnimationTrackView : TrackViewBase
{
    public override string MenuAssetPath => "Assets/XIFramework/SkillEditor/EditorWindow/Editor/Asset/VisualTree/AnimationTrackMenu.uxml";
    public override string TrackAssetPath => "Assets/XIFramework/SkillEditor/EditorWindow/Editor/Asset/VisualTree/AnimationTrackContent.uxml";


    public override void Init(VisualElement menuParent, VisualElement trackParent)
    {
        base.Init(menuParent, trackParent);
        track.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        track.RegisterCallback<DragExitedEvent>(OnDragExited);
    }

    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        
        AnimationClip clip = objs[0] as AnimationClip;

        if (clip != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }
    
    private void OnDragExited(DragExitedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        
        AnimationClip clip = objs[0] as AnimationClip;

        if (clip != null)
        {
            //放置动画资源,当前位置是否可以放置
            //当前选中的位置检测能否放置动画
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            Debug.Log(selectFrameIndex);
            //TODO : 检查当前选中帧 不再任何已有的TrackItem之间
        }
    }
}
