using Employees.Models;

namespace Employees.Services;

public interface IEntryDataService
{
    Task<IReadOnlyList<EntryData>> GetEntriesAsync();
    Task<IReadOnlyList<Time>> GetTotalsAsync();
}
