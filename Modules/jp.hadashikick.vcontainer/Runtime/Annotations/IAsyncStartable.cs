using System.Threading;
using Cysharp.Threading.Tasks;
public interface IAsyncStartable
{
    UniTask StartAsync(CancellationToken cancellation);
}
