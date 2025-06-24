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
        
        [Header("Game Feature")]
        public List<XIGameFeature> worldFeatures = new List<XIGameFeature>();
        
        public bool isPersistent;
    }
}