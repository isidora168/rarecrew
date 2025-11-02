using Employees.Models;
using Microsoft.Extensions.Options;

namespace Employees.Services;

public class ApiUrlFactory : IApiUrlFactory
{
    private readonly ApiOptions _api;
    public ApiUrlFactory(IOptions<ApiOptions> api) => _api = api.Value;

    public string BuildUrl(string rawKey)
    {
        if (string.IsNullOrWhiteSpace(_api.BaseUrl))
            throw new InvalidOperationException("Missing Api:BaseUrl.");
        var code = rawKey.Contains('%') ? rawKey : Uri.EscapeDataString(rawKey);
        return $"{_api.BaseUrl}?code={code}";
    }
}
