using Cysharp.Threading.Tasks;

namespace Features.Firebase.Declaration
{
    public interface IFirebaseService
    {
        UniTask InitializeAsync();
    }
}
