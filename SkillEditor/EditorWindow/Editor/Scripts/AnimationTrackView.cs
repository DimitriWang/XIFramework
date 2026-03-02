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

            bool canPlace = true;

            int durationFrame = -1; //-1 代表可以用AnimationClip的持续时间

            int nextTrackItem = -1;

            int clipFrameCount = (int) (clip.length * clip.frameRate);
            
            int currentOffset = int.MaxValue;

            foreach (var iteritem in SkillEditorWindow.Instance.SkillConfig.SkillAnimationData.FrameData)
            {
                //不允许 选中帧TrackItem 中间(动画事件的起点到您的终点之间)

                if (selectFrameIndex > iteritem.Key && selectFrameIndex < iteritem.Value.DurationFrame + iteritem.Key)
                {
                    canPlace = false;
                    break;
                }
                
                //找到最右侧 最近的TrackItem

                if (iteritem.Key > selectFrameIndex)
                {
                    int tempOffset = iteritem.Key - selectFrameIndex;
                    if (tempOffset < currentOffset)
                    {
                        currentOffset = tempOffset;
                        nextTrackItem = iteritem.Key;
                    }
                }
            }
            //实际的放置

            if (canPlace)
            {
                //右边有其他的TrackItem,要考虑Track不能重叠的问题
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    if (offset < 0) durationFrame = clipFrameCount;
                    else durationFrame = currentOffset;
                }
                // 右边啥都没有
                else
                {
                    durationFrame = clipFrameCount;
                }
            }
            
            Debug.Log(canPlace);
            
        }
    }
}
