# dotnet-upgrade-replay-tests

Replay tests that check an API still returns what it returned before a .NET upgrade.

A small sample for protecting a partner-facing API while you upgrade .NET.

`OrdersApi` is a stand-in for a system that other people depend on: a JSON endpoint and a nightly CSV export. `ReplayTests` replays recorded requests against it and compares the status code, content type and body with what the old system returned.

It is a sample built for the article, not a real production system.

## Run it

You need the .NET 8 and .NET 10 SDKs.

```
dotnet test ReplayTests -f net8.0
dotnet test ReplayTests -f net10.0
```

## How the recordings were made

`record.py` calls a running instance of the old system and writes one JSON file per request into `ReplayTests/recordings`.

```
dotnet run --project OrdersApi -f net8.0 --urls http://127.0.0.1:5077
python3 record.py http://127.0.0.1:5077
```

## Try breaking it

Change the CSV date format in `OrdersApi/Program.cs` from `yyyy-MM-dd` to `dd/MM/yyyy`, or set a snake_case JSON naming policy, and run the tests again. The replay tests fail and show the expected and actual body.
