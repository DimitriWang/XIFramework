using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
// IAsyncInitialization.cs
    public interface IAsyncInitialization
    {
        UniTask InitializeAsync();
    }
}