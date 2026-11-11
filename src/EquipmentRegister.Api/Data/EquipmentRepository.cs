using System.Collections.Concurrent;
using EquipmentRegister.Api.Domain;

namespace EquipmentRegister.Api.Data;

public interface IEquipmentRepository
{
    Task<IReadOnlyList<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<EquipmentItem?> FindByTagAsync(string tag, CancellationToken cancellationToken = default);

    Task UpsertAsync(EquipmentItem item, CancellationToken cancellationToken = default);
}

public sealed class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly ConcurrentDictionary<string, EquipmentItem> _items = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryEquipmentRepository(IEnumerable<EquipmentItem> seed)
    {
        foreach (var item in seed)
        {
            _items[item.Tag] = item;
        }
    }

    public Task<IReadOnlyList<EquipmentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<EquipmentItem> items = _items.Values
            .OrderBy(item => item.Tag, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return Task.FromResult(items);
    }

    public Task<EquipmentItem?> FindByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        _items.TryGetValue(tag, out var item);
        return Task.FromResult(item);
    }

    public Task UpsertAsync(EquipmentItem item, CancellationToken cancellationToken = default)
    {
        _items[item.Tag] = item;
        return Task.CompletedTask;
    }
}
