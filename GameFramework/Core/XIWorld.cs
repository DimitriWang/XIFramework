using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XIFramework.GameFramework
{
    public class XIWorldContext
    {
        public string Name { get; }
        public XIGameInstance GameInstance { get; }
        public XIWorldSettings Settings { get; }
        public XIGameWorld GameWorld { get; private set; }
        public IXIFrameworkContainer WorldContainer { get; private set; }
    }


    public class XIGameWorld
    {
        
    }
    
    
    [System.Serializable]
    public class XIWorldSettings
    {
        public string SceneName = "MainScene";
        
        [Header("Game Mode")]
        [SerializeField]
        [TypeConstraint(typeof(GameMode), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference gameModeType;
        
        [Header("Game Feature")]
        public List<XIGameFeature> worldFeatures = new List<XIGameFeature>();
        
        public bool isPersistent;
    }
}