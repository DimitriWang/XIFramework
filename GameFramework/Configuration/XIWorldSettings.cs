using System.Collections.Generic;
using UnityEngine;

namespace XIFramework.GameFramework
{
    [System.Serializable]
    public class XIWorldSettings
    {
        public string SceneName = "MainScene";
        
        [Header("Game Mode")]
        [SerializeField]
        [TypeConstraint(typeof(XIGameMode), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference gameModeType;
        public bool isPersistent;
        
        [Header("Game Feature")]
        public string featureConfigName = "DefaultFeatureConfig";
        public bool loadAsync = true;
    }
}