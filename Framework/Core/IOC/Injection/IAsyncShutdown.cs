using Cysharp.Threading.Tasks;

namespace XIFramework
{
// IAsyncShutdown.cs
    public interface IAsyncShutdown
    {
        UniTask ShutdownAsync();
    }
}