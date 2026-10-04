# Virtualized selection benchmarks

This companion benchmark harness provides reproducible performance evidence for upstream PR #648.

PR #648 is the production fix. This benchmark work is optional and should not be accepted independently of that PR.

## Pinned comparison

The authoritative comparison is:

- upstream `dev`: `7b5faf7ebb7666558d13c447313e9b09c92a110d`
- PR #648 candidate: `e7020d49a824a371ab3f3450255f10d9cfad8783`

The runner deliberately pins both commits. PR2 itself contains benchmark infrastructure, so using PR2 `HEAD` as the fixed side would measure the wrong Git point.

A normal merge of PR #648 is the simplest path because it preserves the candidate SHA. If PR #648 is squash-merged or rebase-merged, update `FixedRef` to the resulting upstream commit and rebase this companion branch before merging it.

## What is measured

The benchmark project uses bUnit to execute the real Blazor component lifecycle.

The structural probe records:

- hidden shadow-list DOM item count;
- materialized `MudSelectItemExtended` component count;
- bUnit render count;
- generated markup length;
- one-shot render time;
- total allocated bytes;
- an explicitly labelled approximate retained-memory delta after a forced full GC while the rendered component remains alive.

The probe matrix includes:

- 10, 100, 1,000 and 4,000 items;
- virtualized and non-virtualized controls;
- 1, 5 and 20 simultaneous selects;
- selection densities from 1 through 100 selected values.

Direct `MudListExtended` BenchmarkDotNet cases remain available as a control.

## CI

The GitHub Actions workflow runs the structural probe against the two immutable commits and uploads the raw results plus generated comparison summary.

Hosted-runner elapsed timings are illustrative because runner hardware and contention vary. Do not use elapsed-time thresholds as a merge gate. Structural component counts, allocation behavior and scaling shape are the stronger CI signals.

The automated workflow intentionally does **not** run the current initial-render BenchmarkDotNet scenario. During development that scenario did not distinguish the known shadow-list pathology reliably enough to be useful headline evidence.

## Running locally

Requirements:

- Git
- PowerShell 7+
- .NET 10 SDK

Run the pinned structural comparison:

```powershell
./benchmarks/run-virtualization-benchmarks.ps1 -ProbeOnly
```

Run the complete benchmark suite manually:

```powershell
./benchmarks/run-virtualization-benchmarks.ps1
```

Run a quick BenchmarkDotNet smoke test:

```powershell
./benchmarks/run-virtualization-benchmarks.ps1 `
    -SkipProbe `
    -Quick `
    -BenchmarkFilter "*SelectInitialRenderBenchmarks*"
```

For any published evidence, pass explicit refs and retain `run-info.txt` plus each variant's `source.txt`.

## Interpretation

The important scaling property is that `Virtualize=true` must not cause `MudSelectExtended` to instantiate one hidden item component for every member of `ItemCollection` merely to retain selected-value presentation state.

Large reductions in materialized component count and managed allocation are therefore directly meaningful. Absolute timings from shared CI are secondary evidence.

This benchmark companion exists to make the performance claim behind PR #648 inspectable and repeatable; it is not required for the production fix itself.
