using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWHistory.Data;
using PWHistory.Models;

namespace PWHistory.Controllers;

[ApiController]
[Route("[controller]")]
public class RunsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RunsController(AppDbContext db)
    {
        _db = db;
    }

    // POST /runs — sla een testrun op
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TestRun run)
    {
        if (run == null)
            return BadRequest("run is null");

        Console.WriteLine(
            $"Received run: {run.SuiteName}, passed={run.Passed}, results={run.Results?.Count}"
        );

        run.RunAt = DateTime.UtcNow;
        foreach (var result in run.Results ?? [])
        {
            result.Id = 0;
            result.TestRunId = 0;
        }

        _db.TestRuns.Add(run);
        await _db.SaveChangesAsync();
        return Ok(run);
    }

    // GET /runs?from=2026-05-01&to=2026-05-11 — alle runs ophalen, optioneel gefilterd op datum
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? from, [FromQuery] string? to)
    {
        var runs = await _db.TestRuns
            .Include(r => r.Results)
            .OrderByDescending(r => r.RunAt)
            .ToListAsync();

        Console.WriteLine($"from={from}, to={to}");

        if (DateTime.TryParse(from, out var fromDate))
            runs = runs.Where(r => r.RunAt.Date >= fromDate.Date).ToList();

        if (DateTime.TryParse(to, out var toDate))
            runs = runs.Where(r => r.RunAt.Date <= toDate.Date).ToList();

        return Ok(runs);
    }

    // GET /runs/stats — pass rate, gemiddelde duration, flakey tests
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var runs = await _db.TestRuns.Include(r => r.Results).ToListAsync();

        if (!runs.Any())
            return Ok(new { message = "No runs yet" });

        var totalRuns = runs.Count;

        var totalPassed = runs.Sum(r => r.Passed);
        var totalFailed = runs.Sum(r => r.Failed);
        var passRate = totalPassed + totalFailed == 0 ? 0
            : Math.Round((double)totalPassed / (totalPassed + totalFailed) * 100, 1);

        var avgDuration = runs.Average(r => r.Duration);

        var allResults = runs.SelectMany(r => r.Results);
        var flaky = allResults
            .GroupBy(r => r.Title)
            .Where(g => g.Any(r => r.Status == "passed") && g.Any(r => r.Status == "failed"))
            .Select(g => g.Key)
            .ToList();

        return Ok(
            new
            {
                totalRuns,
                passRate = passRate,
                avgDuration = Math.Round(avgDuration, 0),
                flakyTests = flaky,
            }
        );
    }
}