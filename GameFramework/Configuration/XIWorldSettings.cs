using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace XIFramework.GameFramework
{
    [System.Serializable]
    public class XIWorldSettings
    {
        [Header("Level Management")] 
        public string PersistentLevel = "MainLevels";
        public List<string> SubLevels = new List<string>();

        [Header("Game Mode")]
        [SerializeField]
        [TypeConstraint(typeof(XIGameMode), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference GameModeType;

        [Header("Game Feature")] 
        public string FeatureConfigName = "DefaultFeatureConfig";

        public bool isAsyncLoad = true;
    }
}