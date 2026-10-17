# Orphaned snapshots

A snapshot stays on disk when its test, or a case of it, is renamed or deleted. To find such snapshots, call
`OrphanedSnapshots.CheckAsync()` once after all tests of the test project have run, in the teardown of your test
framework:

```csharp title="xUnit v3"
[assembly: AssemblyFixture(typeof(SnapshotCheck))]

public sealed class SnapshotCheck : IAsyncDisposable
{
    public async ValueTask DisposeAsync() => await OrphanedSnapshots.CheckAsync();
}
```

```csharp title="NUnit"
[SetUpFixture]
public sealed class SnapshotCheck
{
    [OneTimeTearDown]
    public async Task CheckSnapshots() => await OrphanedSnapshots.CheckAsync();
}
```

```csharp title="MSTest"
[TestClass]
public sealed class SnapshotCheck
{
    [AssemblyCleanup]
    public static async Task CheckSnapshots() => await OrphanedSnapshots.CheckAsync();
}
```

```csharp title="TUnit"
public sealed class SnapshotCheck
{
    [After(TestSession)]
    public static async Task CheckSnapshots() => await OrphanedSnapshots.CheckAsync();
}
```

It fails when a snapshot or a pending file of the test project is not used by any `MatchesSnapshot()` of the test run,
e.g. after the case `"Let It Be"` was removed from `AlbumTests.Describes`:

```text title="Failure message"
Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
  /src/Tests/AlbumTests.Describes.Let It Be.snapshot.txt
```

While accepting all changes (`SnapshotAcceptance.All`, see [Accepting a snapshot](02-accepting-snapshots.md)), the
orphaned snapshots and their pending files are deleted, and the check still fails, so that a deletion never goes
unnoticed; `SnapshotAcceptance.New` only adds snapshots, so it keeps them:

```text title="Failure message"
Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
  /src/Tests/AlbumTests.Describes.Let It Be.snapshot.txt (deleted)
```

A test run that only runs some of the tests, e.g. a single test in the IDE, cannot be told apart from a complete one,
so the check only runs on a build server (when `CI`, `TF_BUILD`, `JENKINS_URL` or `TEAMCITY_VERSION` is set), where
the environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` is ignored, so that it only reports. To check locally after
running all tests, e.g. to delete the orphaned snapshots together with `AWEXPECT_ACCEPT_SNAPSHOTS=all`, force it:

```csharp
await OrphanedSnapshots.CheckAsync(force: true);
```

Only the directory of the test project is searched, without `bin`, `obj`, hidden directories and the directories of
other projects in it. A snapshot counts as used as soon as `MatchesSnapshot()` is called for it, even if it is not
compared, e.g. after `.Or` was already met. The check cannot know about tests that did not reach `MatchesSnapshot()`:

- A skipped test, or one that fails before calling `MatchesSnapshot()`, leaves its snapshot unused.
- A test that only exists for some target frameworks, e.g. behind `#if NET8_0_OR_GREATER`, leaves its snapshot unused
  in the test runs of the other target frameworks, so only check in the run of a target framework that has all tests.
- A snapshot per environment (see [Snapshots per environment](03-naming-snapshots.md#snapshots-per-environment)), e.g.
  `AlbumTests.Describes@net48@windows.snapshot.txt`, is only checked by a test run in that environment, here on
  Windows for `net48`; the test runs of the other environments skip it.
- A file name that only differs in case from a used snapshot counts as used, as it is the same file on Windows and
  macOS.
- Test projects that share a directory see each other's snapshots, so give each test project its own directory.
