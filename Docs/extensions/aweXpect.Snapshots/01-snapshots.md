# Snapshots

## Matching a snapshot

You can verify that a string matches the snapshot that is stored next to the test:

```csharp
public class GreetingTests
{
    [Fact]
    public async Task Greets()
    {
        string greeting = "Hello, World!";

        await Expect.That(greeting).MatchesSnapshot();
    }
}
```

The snapshot is the file `GreetingTests.Greets.snapshot.txt` in the directory of the test file: the first part is the
name of the test file without its extension, the second part the name of the test method. Both are determined at
compile time, so no reflection is needed and the expectation also works under Native AOT.

Before the first run the snapshot does not exist yet, so the expectation fails with:

```text title="Failure message"
Expected that greeting
matches the snapshot "/src/Tests/GreetingTests.Greets.snapshot.txt",
but the snapshot did not exist

Actual:
Hello, World!

Pending snapshot:
/src/Tests/GreetingTests.Greets.snapshot.pending.txt
```

If the snapshot contained "Hello, world!" instead, the expectation would fail with:

```text title="Failure message"
Expected that greeting
matches the snapshot "/src/Tests/GreetingTests.Greets.snapshot.txt",
but it was "Hello, World!", which differs at index 7:
          ↓ (actual)
  "Hello, World!"
  "Hello, world!"
          ↑ (expected)

Pending snapshot:
/src/Tests/GreetingTests.Greets.snapshot.pending.txt
```

A snapshot with several lines lists every line that differs.

## Accepting a snapshot

A missing or different snapshot writes the actual value to `<Class>.<Method>.snapshot.pending.txt` next to the
snapshot. Review it and rename it to `<Class>.<Method>.snapshot.txt` to accept it. The next run that matches the
snapshot deletes a leftover pending file.

Add the pending files to your `.gitignore`, so that they are not committed by accident:

```text title=".gitignore"
*.snapshot.pending.txt
```

To accept many snapshots at once, set the environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` (to anything but `0` or
`false`) and run the tests. A missing or different snapshot is then replaced by the actual value, and the expectation
still fails, so that an accepted snapshot never goes unnoticed:

```text title="Failure message"
Expected that greeting
matches the snapshot "/src/Tests/GreetingTests.Greets.snapshot.txt",
but the snapshot did not exist

Actual:
Hello, World!

Accepted snapshot:
/src/Tests/GreetingTests.Greets.snapshot.txt
```

The next run compares with the accepted snapshot. Review the changed snapshots in version control before committing
them.

The environment variable is ignored on a build server (when `CI`, `TF_BUILD`, `JENKINS_URL` or `TEAMCITY_VERSION` is
set), so that a variable that was forgotten in a pipeline cannot change the snapshots there. To decide it in code
instead, which also applies on a build server:

```csharp
await Expect.That(greeting).MatchesSnapshot().AcceptingChanges();          // accepts
await Expect.That(greeting).MatchesSnapshot().AcceptingChanges(isLocalRun); // accepts if isLocalRun is true

Customize.aweXpect.Snapshots().AcceptChanges.Set(true);                     // for the current async flow
```

`AcceptingChanges(…)` takes precedence over `Customize.aweXpect.Snapshots().AcceptChanges`, which takes precedence
over the environment variable. `AcceptingChanges(…)` is always the last option of the expectation, so that it is easy to
find and remove again.

## Naming a snapshot

The cases of a parameterized test share the name of the test method, so give each case its own snapshot with
`.Named(…)`:

```csharp
[Theory]
[InlineData("de-DE")]
[InlineData("en-US")]
public async Task Greets(string culture)
{
    string greeting = Greet(culture);

    await Expect.That(greeting).MatchesSnapshot().Named(culture);
}
```

The name is appended to the file names, e.g. `GreetingTests.Greets.de-DE.snapshot.txt`. The same applies to several
snapshots in one test method.

A snapshot that was already used for another value in the same test run fails, so that the cases cannot overwrite each
other's pending snapshot:

```text title="Failure message"
Expected that greeting
matches the snapshot "/src/Tests/GreetingTests.Greets.snapshot.txt",
but the snapshot was already used for another value in this test run, so name each one with .Named(…)
```

The same value again is allowed, so that a retried or repeated test still compares with its snapshot.

The test file and method are set by the compiler through optional parameters, so a name must not be passed as an
argument: `MatchesSnapshot("login")` throws an `ArgumentException` that points to `.Named(…)`.

## Files

The snapshot files are encoded as UTF-8. The pending file ends with an additional line break, which is removed again
when the snapshot is read, so that an editor which adds a final line break does not change the snapshot.

The newline style is ignored when comparing, because version control can change it when the snapshot is checked out.

A snapshot that cannot be read, e.g. because another process locks it, fails the expectation with the reason. A pending
file that cannot be written is named as such in the failure, and a leftover pending file that cannot be deleted does not
fail a matching snapshot.

A snapshot cannot be negated: `DoesNotComplyWith(it => it.MatchesSnapshot())` always fails, as accepting the pending
snapshot would turn any success into a failure.

## Deterministic builds

A deterministic build, e.g. with `ContinuousIntegrationBuild=true` on a build server, replaces the path of the
repository with `/_/` in the compiled code, so the path of the test file, e.g. `/_/Tests/GreetingTests.cs`, no longer
leads to its snapshots. The package records the real path of the repository in the test assembly when it is built, so
the snapshots are found in such a build as well, without any configuration.

A snapshot is identified by the path of its test file relative to the repository, e.g. `Tests/GreetingTests.cs`, which
is the same on every machine and in every build.

This needs two things, which a usual setup provides:

- The test project references the `aweXpect.Snapshots` package, directly or through another project, as the build
  targets that record the path come with the package.
- The repository provides source control information, which the .NET SDK does for a git repository.

Without them, a snapshot is identified by the full path of its test file, and in a deterministic build the expectation
fails with:

```text title="Failure message"
Expected that greeting
matches the snapshot "/_/Tests/GreetingTests.Greets.snapshot.txt",
but the snapshot could not be read: The path "/_/Tests/GreetingTests.cs" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.
```

## Storing snapshots elsewhere

By default the snapshots are files next to the tests. To keep them elsewhere, e.g. in a shared folder or a database,
derive from `SnapshotStore` in the namespace `aweXpect.Snapshots`:

```csharp
public sealed class FolderSnapshotStore(string root) : SnapshotStore
{
    public override ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
    {
        string path = GetPath(id, "snapshot");
        return new ValueTask<string?>(File.Exists(path) ? File.ReadAllText(path) : null);
    }

    public override ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
    {
        string path = GetPath(id, "pending");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, actual);
        return default;
    }

    public override ValueTask AcceptAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
    {
        string path = GetPath(id, "snapshot");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, actual);
        File.Delete(GetPath(id, "pending"));
        return default;
    }

    public override string Describe(SnapshotId id) => GetPath(id, "snapshot");

    public override string? DescribeMismatch(SnapshotId id) => GetPath(id, "pending");

    private string GetPath(SnapshotId id, string kind)
        => Path.Combine(root, Path.GetDirectoryName(id.SourceFile) ?? "", $"{id}.{kind}.txt");
}
```

The operations describe what happened in the expectation, so the store decides how it keeps a value that awaits review
apart from the reviewed snapshot:

| Member                | Called                                         | If it throws                                     |
|-----------------------|------------------------------------------------|--------------------------------------------------|
| `ReadAsync`           | to read the reviewed snapshot, `null` if none  | the expectation fails with the exception         |
| `RecordMismatchAsync` | when the snapshot is missing or different      | the failure names the error                      |
| `AcceptAsync`         | instead, while accepting changes               | the failure names the error                      |
| `RecordMatchAsync`    | when the snapshot matches, e.g. to clean up    | the error is ignored, as the snapshot matched    |
| `Describe`            | to name the snapshot in the failure            |                                                  |
| `DescribeMismatch`    | to name the recorded mismatch, `null` to omit  |                                                  |

`RecordMatchAsync`, `Describe` and `DescribeMismatch` are optional. A cancellation of the evaluation still aborts it.

A `SnapshotId` consists of the path of the test file relative to the repository (`SourceFile`, e.g.
`Tests/GreetingTests.cs`, see [Deterministic builds](#deterministic-builds)), the name of the test method
(`MemberName`) and the name from `.Named(…)` (`Name`). It is a record, so it can be used as a key, and its `ToString()`
returns `GreetingTests.Greets` or `GreetingTests.Greets.de-DE`.

Set the store for all tests, e.g. in an assembly-level setup, or only for the current async flow:

```csharp
Customize.aweXpect.Global.Snapshots().Store.Set(new FolderSnapshotStore("/shared/snapshots"));

using (Customize.aweXpect.Snapshots().Store.Set(new FolderSnapshotStore("/shared/snapshots")))
{
    await Expect.That(greeting).MatchesSnapshot();
}
```

The store is taken when `MatchesSnapshot()` is called.
