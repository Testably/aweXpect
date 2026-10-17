# Storing snapshots elsewhere

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
| `AcceptAsync`         | instead, while accepting changes               | the failure names the error, and the mismatch is recorded instead |
| `RecordMatchAsync`    | when the snapshot matches, e.g. to clean up    | the error is ignored, as the snapshot matched    |
| `Describe`            | to name the snapshot in the failure            |                                                  |
| `DescribeMismatch`    | to name the recorded mismatch, `null` to omit  |                                                  |
| `ListAsync`           | to list the snapshots of the test project for the [check](05-orphaned-snapshots.md), `null` if it cannot | the check fails with the exception |
| `DeleteAsync`         | to delete an orphaned snapshot and its mismatch, while accepting changes | the check names the error and continues |

`RecordMatchAsync`, `Describe`, `DescribeMismatch`, `ListAsync` and `DeleteAsync` are optional. A store that does not
list its snapshots fails `OrphanedSnapshots.CheckAsync()`, so that a check that was asked for never passes without
checking anything. A cancellation of the evaluation still aborts it.

A `SnapshotId` consists of the path of the test file relative to the repository (`SourceFile`, e.g.
`Tests/AlbumTests.cs`, see [Deterministic builds](06-deterministic-builds.md)), the name of the test method
(`MemberName`), the name from `.Named(…)` (`Name`) and the environment from `.UniqueFor` (`Variant`, e.g.
`net8.0@windows`, see [Snapshots per environment](03-naming-snapshots.md#snapshots-per-environment)). It is a record,
so it can be used as a key, and its `ToString()` returns `AlbumTests.Describes`, `AlbumTests.Describes.Abbey Road` or
`AlbumTests.Describes.Abbey Road@net8.0@windows`. A listed snapshot whose `Variant` is for another environment is
skipped by the check.

Set the store for all tests, e.g. in an assembly-level setup, or only for the current async flow:

```csharp
Customize.aweXpect.Global.Snapshots().Store.Set(new FolderSnapshotStore("/shared/snapshots"));

using (Customize.aweXpect.Snapshots().Store.Set(new FolderSnapshotStore("/shared/snapshots")))
{
    await Expect.That(description).MatchesSnapshot();
}
```

The store is taken when `MatchesSnapshot()` is called.
