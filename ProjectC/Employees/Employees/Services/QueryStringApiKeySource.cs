namespace Employees.Services;

public class QueryStringApiKeySource : IApiKeySource
{
    private readonly IHttpContextAccessor _ctx;
    public QueryStringApiKeySource(IHttpContextAccessor ctx) => _ctx = ctx;

    public string GetKeyOrThrow()
    {
        var req = _ctx.HttpContext?.Request;
        var key = (req?.Query["key"].ToString() ?? req?.Query["code"].ToString() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing API key. Add ?key=YOUR_KEY.");
        return key;
    }
}
