using Cysharp.Threading.Tasks;

namespace XIFramework
{
// IAsyncInitialization.cs
    public interface IAsyncInitialization
    {
        UniTask InitializeAsync();
    }
}