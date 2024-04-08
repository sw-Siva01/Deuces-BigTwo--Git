using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

#nullable enable

public interface ITableView
{
    UniTask DoShuffleVisualAsync(
        Dictionary<int, List<int>> shuffleResult,
        CancellationToken cancellationToken);
}