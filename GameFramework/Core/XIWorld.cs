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
        public string sceneName = "MainScene";
        public System.Type gameModeType;
        public bool isPersistent;
    }
}