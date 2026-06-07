using System.Text.Json.Serialization;

namespace PWHistory.Models;

public class TestResult
{
    public int Id { get; set; }
    public int TestRunId { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
    public long Duration { get; set; }
    public string? Error { get; set; }
    
    [JsonIgnore]
    public TestRun? TestRun { get; set; }
}