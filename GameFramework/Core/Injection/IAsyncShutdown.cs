using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
// IAsyncShutdown.cs
    public interface IAsyncShutdown
    {
        UniTask ShutdownAsync();
    }
}