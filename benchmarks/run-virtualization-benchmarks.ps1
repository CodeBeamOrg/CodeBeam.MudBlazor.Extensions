[CmdletBinding()]
param(
    [string]$BaselineRef = "7b5faf7ebb7666558d13c447313e9b09c92a110d",
    [string]$FixedRef = "e7020d49a824a371ab3f3450255f10d9cfad8783",
    [string]$ResultsDirectory,
    [string]$BenchmarkFilter = "*",
    [switch]$Quick,
    [switch]$ProbeOnly,
    [switch]$SkipProbe
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if ($ProbeOnly -and $SkipProbe) {
    throw "ProbeOnly and SkipProbe cannot be used together."
}

$repoRoot = (& git -C $PSScriptRoot rev-parse --show-toplevel).Trim()
if ($LASTEXITCODE -ne 0) {
    throw "Unable to locate the repository root."
}

if ([string]::IsNullOrWhiteSpace($ResultsDirectory)) {
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $ResultsDirectory = Join-Path $repoRoot "BenchmarkDotNet.Artifacts/virtualized-list-selection-state/$timestamp"
}

$ResultsDirectory = [IO.Path]::GetFullPath($ResultsDirectory)
New-Item -ItemType Directory -Path $ResultsDirectory -Force | Out-Null

function Resolve-Commit([string]$ref) {
    $sha = (& git -C $repoRoot rev-parse "$ref^{commit}").Trim()
    if ($LASTEXITCODE -ne 0) {
        throw "Unable to resolve '$ref'."
    }

    return $sha
}

function Invoke-Checked([string]$command, [string[]]$arguments) {
    & $command @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed ($LASTEXITCODE): $command $($arguments -join ' ')"
    }
}

$baselineSha = Resolve-Commit $BaselineRef
$fixedSha = Resolve-Commit $FixedRef
$benchmarksSource = Join-Path $repoRoot "benchmarks"
$workRoot = Join-Path ([IO.Path]::GetTempPath()) "CodeBeamMudExtensionsBenchmarks-$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $workRoot -Force | Out-Null

$previousCi = $env:CI
$previousVariant = $env:BENCHMARK_VARIANT
$worktrees = [System.Collections.Generic.List[string]]::new()

function Invoke-Variant([string]$name, [string]$sha) {
    $worktree = Join-Path $workRoot $name
    $worktrees.Add($worktree)

    Write-Host "`n=== $name ($sha) ===" -ForegroundColor Cyan
    Invoke-Checked git @("-C", $repoRoot, "worktree", "add", "--detach", $worktree, $sha)

    $targetBenchmarks = Join-Path $worktree "benchmarks"
    New-Item -ItemType Directory -Path $targetBenchmarks -Force | Out-Null
    Copy-Item -Path (Join-Path $benchmarksSource "*") -Destination $targetBenchmarks -Recurse -Force

    $variantResults = Join-Path $ResultsDirectory $name
    New-Item -ItemType Directory -Path $variantResults -Force | Out-Null

    $project = Join-Path $targetBenchmarks "CodeBeam.MudBlazor.Extensions.Benchmarks/CodeBeam.MudBlazor.Extensions.Benchmarks.csproj"
    $probeOutput = Join-Path $variantResults "scale-probe.csv"
    $bdnArtifacts = Join-Path $variantResults "BenchmarkDotNet.Artifacts"

    $env:CI = "true"
    $env:BENCHMARK_VARIANT = $name

    (& dotnet --info) | Out-File -FilePath (Join-Path $variantResults "dotnet-info.txt") -Encoding utf8
    (& git -C $worktree show -s --format="%H%n%ad%n%s" --date=iso-strict HEAD) |
        Out-File -FilePath (Join-Path $variantResults "source.txt") -Encoding utf8

    if (-not $SkipProbe) {
        Invoke-Checked dotnet @(
            "run", "--project", $project, "--configuration", "Release", "--",
            "probe", "--output", $probeOutput
        )
    }

    if ($ProbeOnly) {
        return
    }

    $benchmarkArguments = [System.Collections.Generic.List[string]]::new()
    @(
        "run", "--project", $project, "--configuration", "Release", "--",
        "--filter", $BenchmarkFilter,
        "--artifacts", $bdnArtifacts,
        "--exporters", "GitHub", "CSV", "JSON",
        "--allStats",
        "--join"
    ) | ForEach-Object { $benchmarkArguments.Add($_) }

    if ($Quick) {
        $benchmarkArguments.Add("--job")
        $benchmarkArguments.Add("short")
    }

    Invoke-Checked dotnet $benchmarkArguments.ToArray()
}

function Format-SummaryBytes([double]$bytes) {
    $absolute = [Math]::Abs($bytes)
    if ($absolute -ge 1GB) { return "{0:F2} GB" -f ($bytes / 1GB) }
    if ($absolute -ge 1MB) { return "{0:F2} MB" -f ($bytes / 1MB) }
    if ($absolute -ge 1KB) { return "{0:F1} KB" -f ($bytes / 1KB) }
    return "{0:F0} B" -f $bytes
}

function Get-ProbeRow([string]$variant, [int]$itemCount, [int]$selectCount, [int]$selectedCount, [bool]$virtualize) {
    $path = Join-Path $ResultsDirectory "$variant/scale-probe.csv"
    if (-not (Test-Path $path)) {
        return $null
    }

    return Import-Csv $path |
        Where-Object {
            [int]$_.ItemCount -eq $itemCount -and
            [int]$_.SelectCount -eq $selectCount -and
            [int]$_.SelectedCount -eq $selectedCount -and
            [bool]::Parse($_.Virtualize) -eq $virtualize
        } |
        Select-Object -First 1
}

function Get-ProbeValue($row, [string]$property) {
    if ($null -eq $row) {
        return $null
    }

    return [double]$row.PSObject.Properties[$property].Value
}

function Format-ProbeValue([string]$property, $value) {
    if ($null -eq $value) {
        return "—"
    }

    switch ($property) {
        "ElapsedMilliseconds" { return "{0:F2} ms" -f $value }
        "AllocatedBytes" { return Format-SummaryBytes $value }
        "ApproxRetainedBytes" { return Format-SummaryBytes $value }
        default { return "{0:N0}" -f $value }
    }
}

function Format-Reduction($baselineValue, $fixedValue) {
    if ($null -eq $baselineValue -or $null -eq $fixedValue -or $fixedValue -le 0) {
        return "—"
    }

    $factor = $baselineValue / $fixedValue
    if ($factor -ge 1) {
        return "{0:F1}x lower" -f $factor
    }

    return "{0:F1}x higher" -f (1 / $factor)
}

function Write-ProbeComparisonSummary {
    if ($SkipProbe) {
        return
    }

    $scenarios = @(
        @{ Name = "4,000 items x 1 select"; Items = 4000; Selects = 1; Selected = 2; Virtualize = $true },
        @{ Name = "4,000 items x 5 selects"; Items = 4000; Selects = 5; Selected = 2; Virtualize = $true },
        @{ Name = "4,000 items x 20 selects"; Items = 4000; Selects = 20; Selected = 2; Virtualize = $true },
        @{ Name = "4,000 items, 100 selected"; Items = 4000; Selects = 1; Selected = 100; Virtualize = $true }
    )

    $metrics = @(
        @{ Name = "Materialized components"; Property = "SelectItemComponentCount" },
        @{ Name = "Allocated"; Property = "AllocatedBytes" },
        @{ Name = "Approx. retained"; Property = "ApproxRetainedBytes" },
        @{ Name = "One-shot elapsed"; Property = "ElapsedMilliseconds" }
    )

    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add("# Virtualized selection comparison")
    $lines.Add("")
    $lines.Add("Generated from the same benchmark harness against immutable Git commits.")
    $lines.Add("")
    $lines.Add("- Upstream baseline: `$baselineSha` (`$BaselineRef`)")
    $lines.Add("- PR1 candidate: `$fixedSha` (`$FixedRef`)")
    $lines.Add("")
    $lines.Add("Probe elapsed/retained values are diagnostic. Shared-runner timing should be treated as illustrative; structural component counts and allocation/scaling shape are the stronger CI evidence.")
    $lines.Add("")
    $lines.Add("| Scenario | Metric | Upstream baseline | PR1 candidate | Baseline -> PR1 |")
    $lines.Add("| --- | --- | ---: | ---: | ---: |")

    foreach ($scenario in $scenarios) {
        $baselineRow = Get-ProbeRow "baseline" $scenario.Items $scenario.Selects $scenario.Selected $scenario.Virtualize
        $fixedRow = Get-ProbeRow "fixed" $scenario.Items $scenario.Selects $scenario.Selected $scenario.Virtualize

        foreach ($metric in $metrics) {
            $baselineValue = Get-ProbeValue $baselineRow $metric.Property
            $fixedValue = Get-ProbeValue $fixedRow $metric.Property
            $lines.Add("| $($scenario.Name) | $($metric.Name) | $(Format-ProbeValue $metric.Property $baselineValue) | $(Format-ProbeValue $metric.Property $fixedValue) | $(Format-Reduction $baselineValue $fixedValue) |")
        }
    }

    $summaryPath = Join-Path $ResultsDirectory "comparison-summary.md"
    $lines | Out-File -FilePath $summaryPath -Encoding utf8
    Write-Host "Wrote $summaryPath"
}

try {
    @(
        "Baseline ref: $BaselineRef",
        "Baseline SHA: $baselineSha",
        "PR1 ref: $FixedRef",
        "PR1 SHA: $fixedSha",
        "Started: $([DateTimeOffset]::Now.ToString('O'))",
        "Benchmark filter: $BenchmarkFilter",
        "Quick: $Quick",
        "Probe only: $ProbeOnly",
        "Skip probe: $SkipProbe"
    ) | Out-File -FilePath (Join-Path $ResultsDirectory "run-info.txt") -Encoding utf8

    Invoke-Variant "baseline" $baselineSha
    Invoke-Variant "fixed" $fixedSha
    Write-ProbeComparisonSummary

    Write-Host "`nBenchmark results: $ResultsDirectory" -ForegroundColor Green
}
finally {
    $env:CI = $previousCi
    $env:BENCHMARK_VARIANT = $previousVariant

    foreach ($worktree in $worktrees) {
        if (Test-Path $worktree) {
            & git -C $repoRoot worktree remove --force $worktree | Out-Null
        }
    }

    & git -C $repoRoot worktree prune | Out-Null
    if (Test-Path $workRoot) {
        Remove-Item -Path $workRoot -Recurse -Force
    }
}
