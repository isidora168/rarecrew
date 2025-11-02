namespace Employees.Services;

public interface IApiUrlFactory
{
    string BuildUrl(string rawKey);
}
