using System.Text.Json;
using Employees.Models;

namespace Employees.Services;

public class EntryDataService : IEntryDataService
{
    private readonly HttpClient _http;
    private readonly IApiKeySource _keySource;
    private readonly IApiUrlFactory _urlFactory;

    public EntryDataService(HttpClient http, IApiKeySource keySource, IApiUrlFactory urlFactory)
    {
        _http = http;
        _keySource = keySource;
        _urlFactory = urlFactory;
    }

    public async Task<IReadOnlyList<EntryData>> GetEntriesAsync()
    {
        var key = _keySource.GetKeyOrThrow();

        var url = _urlFactory.BuildUrl(key);

        using var resp = await _http.GetAsync(url);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"API call failed ({(int)resp.StatusCode}). Check the ?key= value. Body: {body}");

        var items = JsonSerializer.Deserialize<List<EntryData>>(
            body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? new();

        return items;
    }

    public async Task<IReadOnlyList<Time>> GetTotalsAsync()
    {
        var entries = await GetEntriesAsync();

        var sums = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var e in entries)
        {
            if (e.DeletedOn != null) continue;
            if (e.EndTimeUtc < e.StartTimeUtc) continue;
            if (string.IsNullOrWhiteSpace(e.EmployeeName)) continue;

            var name = e.EmployeeName.Trim();
            var hours = (e.EndTimeUtc - e.StartTimeUtc).TotalHours;

            if (sums.TryGetValue(name, out var acc)) sums[name] = acc + hours;
            else sums[name] = hours;
        }

        return sums
            .Select(kv => new Time
            {
                Employee = kv.Key,
                TotalHours = kv.Value
            })
            .OrderByDescending(t => t.TotalHours)
            .ToList();
    }
}

