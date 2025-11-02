using System.Text.Json.Serialization;

namespace Employees.Models;

public class EntryData
{
    public string? Id { get; set; }
    public string? EmployeeName { get; set; }

    [JsonPropertyName("StarTimeUtc")]
    public DateTime StartTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }
    public string? EntryNotes { get; set; }
    public DateTime? DeletedOn { get; set; }
}
