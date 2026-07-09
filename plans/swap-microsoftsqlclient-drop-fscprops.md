# Swap FSharp.Data.SqlClient → FSharp.Data.MicrosoftSqlClient, try to drop fsc.props

Replace the legacy `FSharp.Data.SqlClient` (System.Data.SqlClient design-time) type
provider with the `FSharp.Data.MicrosoftSqlClient` fork (Microsoft.Data.SqlClient
design-time). Then test whether the build works under the .NET SDK `fsc` so the
`fsc.props` desktop-compiler hack can be deleted.

## Context

`src/Infrastructure.Data` uses `SqlCommandProvider` (26 sites, 6 repo files). The
provider's **design-time** code (runs inside `fsc` at compile) uses
`System.Data.SqlClient`, which throws "not supported on this platform" inside the
.NET SDK hosted `fsc`. Current workaround: `fsc.props` forces the **desktop**
`fsc.exe` (a VS install), imported only by `Infrastructure.Data.fsproj:30`, plus a
`DisableAutoSetFscCompilerPath` block (fsproj:5-9). This ties builds to a Windows
box with VS + F# desktop tooling.

The `daniellittledev/FSharp.Data.MicrosoftSqlClient` fork (NuGet 1.0.5, targets
net8) is API-identical — same `FSharp.Data` namespace, same `SqlCommandProvider`
and static params — but its design-time uses **Microsoft.Data.SqlClient**, which is
cross-platform and works under SDK `fsc`. If true here, `fsc.props` becomes
unnecessary. That claim is the thing this plan verifies empirically; it is not
guaranteed until the build passes.

Key facts (verified this session):
- All 6 repos `open FSharp.Data` only → **no source code change expected**.
- Design-time conn string literal: `DevConnectionString`, `src/Infrastructure.Data/DbContext.fs:5` (LocalDB `the_collection`).
- `Microsoft.Data.SqlClient` (7.0.2) already resolved in the solution (used by `devops/Migrations`); fork needs `>= 5.1.3` → paket unifies to 7.0.2.
- `src/Infrastructure.MsSql/` is an empty abandoned stub (obj/bin only, no fsproj) — ignore / out of scope.
- CI (`.github/workflows/CI.yml`) installs LocalDB and runs migrations before build — the compile-time DB dependency is already satisfied there.

## For Future Agents
Mark checkboxes `- [x]` as items complete; set each phase Status to `Complete` and
write its Phase Summary + Verification result before moving on. Phase 2 is
**contingent**: only delete `fsc.props` wiring if Phase 1's SDK-fsc build passes.
Prereq for any build: LocalDB `the_collection` must exist and be migrated first
(`dotnet run RunMigrations --project .\devops\Build\Build.fsproj`) — the provider
hits the DB at compile time.

## Phase 1: Swap the package
Status: Complete

Prove the fork compiles the existing provider code with the current desktop-fsc
setup untouched. Isolates the package swap from the compiler question.

- [ ] `paket.dependencies`: replace `nuget FSharp.Data.SqlClient` with `nuget FSharp.Data.MicrosoftSqlClient`. Leave `nuget Microsoft.Data.SqlClient` as-is.
- [ ] `src/Infrastructure.Data/paket.references`: replace `FSharp.Data.SqlClient` with `FSharp.Data.MicrosoftSqlClient`; remove `System.Data.SqlClient` (design-time dep of the old provider, no longer needed); add `Microsoft.Data.SqlClient` (fork's runtime SqlClient for the generated commands). Keep `NodaTime`.
- [ ] Run `dotnet paket install` to re-resolve `paket.lock` (confirm `FSharp.Data.MicrosoftSqlClient` present, `FSharp.Data.SqlClient` + `System.Data.SqlClient` gone unless pulled transitively elsewhere).
- [ ] Do NOT touch source — repos `open FSharp.Data`, unchanged by the fork. Only revisit if the build reports a missing namespace/type.

### Verification Plan
- `dotnet run RunMigrations --project .\devops\Build\Build.fsproj` → migrations apply (ensures `the_collection` exists for design-time).
- `dotnet build src\Infrastructure.Data\Infrastructure.Data.fsproj -c Debug --nologo` → `Build succeeded`, `0 Error(s)` (still using desktop fsc via fsc.props).
- Grep `paket.lock` for `FSharp.Data.MicrosoftSqlClient` → present; `FSharp.Data.SqlClient (` → absent.

### Phase Summary
Swapped to the fork successfully. Changes:
- `paket.dependencies`: `nuget FSharp.Data.SqlClient` → `nuget FSharp.Data.MicrosoftSqlClient`.
- `src/Infrastructure.Data/paket.references`: `FSharp.Data.SqlClient` + `System.Data.SqlClient` → `FSharp.Data.MicrosoftSqlClient` + `Microsoft.Data.SqlClient` (kept `NodaTime`).
- `dotnet paket install` → `paket.lock` now `FSharp.Data.MicrosoftSqlClient (1.0.5)`; old packages gone.

**Key discovery (the whole reason the swap first appeared to "fail"): the fork provides
its types under namespace `FSharp.Data.SqlClient`, NOT `FSharp.Data`.** With the old
`open FSharp.Data`, the provided `SqlCommandProvider` is simply out of scope → the
compiler reports `FS0039: The type 'SqlCommandProvider' is not defined` with **no**
type-provider diagnostic, which looks exactly like a silent TP-load failure but is not.
Proven by loading the fork's design-time DLL standalone (exposes all 4 providers) and by
a minimal repro that builds cleanly once the `open` is corrected.

- Namespace fix applied to all 6 repo files: `open FSharp.Data` → `open FSharp.Data.SqlClient`
  (`UserRepository.fs`, `BagtypeRepository.fs`, `BrandRepository.fs`, `CountryRepository.fs`,
  `TeabagRepository.fs`, `FileRepository.fs`). `SqlCommandProvider` usage/static params unchanged.

### Verification (ran)
- `dotnet build src\Infrastructure.Data\Infrastructure.Data.fsproj` → **Build succeeded** (under SDK fsc, no fsc.props — see Phase 2).
- `paket.lock`: `FSharp.Data.MicrosoftSqlClient (1.0.5)` present; `FSharp.Data.SqlClient` / `System.Data.SqlClient` absent.

## Phase 2: Drop fsc.props
Status: Complete

Because the fork's design-time uses the cross-platform Microsoft.Data.SqlClient, the
provider loads under the .NET SDK `fsc` — no desktop `fsc.exe` needed.

- [x] `src/Infrastructure.Data/Infrastructure.Data.fsproj`: removed `<Import Project="..\..\fsc.props" />` and the `DisableAutoSetFscCompilerPath` / `DotnetFscToolPath` / `DotnetFscCompilerPath` block.
- [x] Built under SDK fsc → passes.
- [x] Deleted `fsc.props` at repo root (`git rm`); repo-wide grep confirms no remaining `fsc.props` / `FscToolPath` references (except this plan).

### Verification (ran)
- `dotnet build TheCollection-SAFE.sln` → **Build succeeded, 0 Error(s)** (Domain, Shared, Infrastructure.Data, Server, Client, Migrations, all test projects).
- Isolated matrix proving the provider now loads under SDK fsc (FS3033 with a live SQL error when given a bad table = provider connected & analyzed the DB; clean build with a valid query).
- `dotnet run --project tests\Server` → **2 passed, 0 failed** (runtime path exercises Microsoft.Data.SqlClient).

## Final Recap
Replaced `FSharp.Data.SqlClient` (design-time `System.Data.SqlClient`, which is unsupported
under the SDK `fsc` and forced the `fsc.props` desktop-compiler hack) with
`FSharp.Data.MicrosoftSqlClient` 1.0.5 (design-time `Microsoft.Data.SqlClient`, cross-platform).
The only source change beyond packaging was the `open` namespace (`FSharp.Data` →
`FSharp.Data.SqlClient`) in the 6 repositories. `fsc.props` deleted; builds now use the
plain .NET SDK `fsc` on any host — no VS / desktop F# tooling required. Full solution and
Server tests pass. Net removed: `fsc.props` (92 lines) + the fsproj compiler-override block +
the `System.Data.SqlClient` dependency.

## Deployment Plan
1. Commit: paket.dependencies, paket.lock, src/Infrastructure.Data/paket.references,
   src/Infrastructure.Data/Infrastructure.Data.fsproj, the 6 repository `.fs` files, and the
   deletion of `fsc.props`.
2. CI (`.github/workflows/CI.yml`): still installs LocalDB and runs migrations before build —
   both **still required** (the provider hits the DB at compile time). No VS/desktop-fsc was
   invoked explicitly in CI, so no workflow edit needed; the build no longer depends on a VS
   install being present on the runner. `windows-latest` can stay; a Linux runner is now
   theoretically possible (LocalDB is Windows-only, so keep Windows unless the DB step is
   swapped for a containerized SQL Server).
3. Runtime note: runtime SqlClient is now Microsoft.Data.SqlClient. Microsoft.Data.SqlClient
   defaults `Encrypt=true`; LocalDB verified working here with the existing connection string,
   but confirm production/Azure connection strings include appropriate `Encrypt` /
   `TrustServerCertificate` settings if a TLS/cert error appears.
