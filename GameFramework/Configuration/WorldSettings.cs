using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XIFramework.GameFramework
{
    [System.Serializable]
    public class WorldSettings : IWorldSettings
    {
        [Header("Level Management")] 
        public string persistentLevel = "MainLevels";
        public string[] subLevels = new string[0];

        [Header("Game Mode")]
        [SerializeField]
        [TypeConstraint(typeof(GameMode), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference GameModeTypeRef;
        public string PersistentLevel => persistentLevel;
        public string[] SubLevels => subLevels;
        public System.Type GameModeType => GameModeTypeRef.Type;

        [Header("Player Controller")]
        [SerializeField]
        [TypeConstraint(typeof(PlayerController), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference PlayerControllerTypeRef;
        
        public System.Type PlayerControllerType => PlayerControllerTypeRef.Type;

        [Header("Player State")]
        [SerializeField]
        [TypeConstraint(typeof(PlayerState), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference PlayerStateTypeRef;
        
        public System.Type PlayerStateType => PlayerStateTypeRef.Type;

        [Header("Game Feature")] 
        public string FeatureConfigName = "DefaultFeatureConfig";

        public bool isAsyncLoad = true;
    }
}