using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using XIFramework;

namespace XIFramework.SkillEditor
{
    [CreateAssetMenu(fileName = "SkillConfig", menuName = "Config/SkillConfig")]
    public class SkillConfig : ConfigBase
    {
        [LabelText("SkillConfig")] public string SKillName;
        [LabelText("帧数上限")] public int FrameCount = 100;

        [NonSerialized, OdinSerialize]
        public SkillAnimationData SkillAnimationData;
    }

    /// <summary>
    /// 技能动画数据
    /// </summary>
    [SerializeField]
    public class SkillAnimationData
    {
        /// <summary>
        /// 动画帧事件
        /// Key: 帧数
        /// Value: 事件数据
        /// </summary>
        [NonSerialized, OdinSerialize]
        [DictionaryDrawerSettings(KeyLabel = "帧数", ValueLabel = "动画事件数据")]
        public Dictionary<int, SkillFrameEventBase> FrameData = new Dictionary<int, SkillFrameEventBase>();
        
        
    }

    /// <summary>
    /// 帧事件基类
    /// </summary>
    public abstract class SkillFrameEventBase
    {
        
    }
    
    /// <summary>
    /// 技能动画事件
    /// </summary>
    public class SkillAnimationEvent : SkillFrameEventBase
    {
        public AnimationClip AnimationClip;
        public int TransitionTime;
#if UNITY_EDITOR
        public int DurationFrame;
#endif
    }
}

