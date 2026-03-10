using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XIFramework.SkillEditor
{
    public class AnimationTrackItem : TrackItemBase
    {
        public AnimationTrackItem() { }
        
        private const string trackItemAssetPath = "Assets/XIFramework/SkillEditor/EditorWindow/Editor/Asset/VisualTree/AnimationTrackItem.uxml";
        
        private int frameIndex;
        private float frameUnitWidth;
        private AnimationTrackView animationTrack;
        private SkillAnimationEvent animationEvent;

        public VisualElement Root => root;

        private VisualElement root;
        private VisualElement leftLine;
        private VisualElement rightLine;
        private Label itemLabel;

        private VisualElement mainDragArea;
        private VisualElement animationOverLine;

        public void Init(AnimationTrackView animationTrack, VisualElement parent, int startFrameIndex, float frameWidth, SkillAnimationEvent animationEvent)
        {
            this.frameUnitWidth = frameWidth;
            this.frameIndex = startFrameIndex;
            this.animationTrack = animationTrack;
            this.animationEvent = animationEvent;

            root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query("AnimationClipTrackView");
            leftLine = root.Q("left_line");
            rightLine = root.Q("right_line");
            itemLabel = root.Q<Label>("clip_name");
            animationOverLine = leftLine;
            mainDragArea = root.Q<VisualElement>("main");
            
            parent.Add(root);
            ResetView(frameWidth);
        }

        public void ResetView(float frameUnitWidth)
        {
            this.frameUnitWidth = frameUnitWidth;
            
            itemLabel.text = animationEvent.AnimationClip.name;
            //位置计算
            Vector3 mainPos = root.transform.position;
            
            mainPos.x = frameIndex * frameUnitWidth;
            
            root.transform.position = mainPos;
            root.style.width = animationEvent.DurationFrame * frameUnitWidth;
            itemLabel.style.width = animationEvent.DurationFrame * frameUnitWidth;
            
            int animationClipFrameCount = (int)(animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate);
            
            // 计算动画结束线的位置
            if (animationClipFrameCount > animationEvent.DurationFrame)
            {
                animationOverLine.style.display = DisplayStyle.None;
            }
            else
            {
                animationOverLine.style.display = DisplayStyle.Flex;
                Vector3 overLinePos = animationOverLine.transform.position;
                overLinePos.x = animationClipFrameCount * frameUnitWidth - 2;//线条宽度为2
                animationOverLine.transform.position = overLinePos;
            }
        }
    }
}