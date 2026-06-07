# PWHistory

Een lichtgewicht ASP.NET Core Web API om testrun-resultaten op te slaan en te analyseren. Bedoeld als backend voor geautomatiseerde testen (bijv. vanuit OutSystems of andere tools via ngrok).

## Functionaliteit

- Testruns opslaan met alle individuele testresultaten
- Runs ophalen met optionele datumfiltering
- Statistieken berekenen: pass rate, gemiddelde duur en flakey tests

## Tech stack

| Onderdeel | Technologie |
|---|---|
| Framework | ASP.NET Core 8 (.NET 8) |
| Database | SQLite (via Entity Framework Core) |
| API-stijl | REST (JSON) |

## Endpoints

### `POST /runs`
Sla een nieuwe testrun op.

**Body:**
```json
{
  "suiteName": "Smoke Tests",
  "passed": 10,
  "failed": 2,
  "skipped": 1,
  "duration": 4500,
  "results": [
    {
      "title": "Login test",
      "status": "passed",
      "duration": 350,
      "error": null
    }
  ]
}
```

---

### `GET /runs`
Haal alle testruns op, gesorteerd op datum (nieuwste eerst).

**Query parameters (optioneel):**

| Parameter | Beschrijving |
|---|---|
| `from` | Startdatum (bijv. `2026-05-01`) |
| `to` | Einddatum (bijv. `2026-05-31`) |

---

### `GET /runs/stats`
Geeft een samenvatting van alle runs.

**Response:**
```json
{
  "totalRuns": 42,
  "passRate": 94.3,
  "avgDuration": 3800,
  "flakyTests": ["Login test", "Checkout flow"]
}
```

## Lokaal draaien

```bash
dotnet run
```

De SQLite-database (`pwhistory.db`) wordt automatisch aangemaakt bij de eerste opstart.
