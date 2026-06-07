namespace PWHistory.Models;

public class TestRun
{
    public int Id { get; set; }
    public string SuiteName { get; set; } = "";
    public DateTime RunAt { get; set; } = DateTime.UtcNow;
    public long Passed { get; set; }
    public long Failed { get; set; }
    public long Skipped { get; set; }
    public long Duration { get; set; }
    public List<TestResult> Results { get; set; } = new();
}