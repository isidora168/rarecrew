namespace Employees.Services;

public interface IApiKeySource
{
    string GetKeyOrThrow();
}
