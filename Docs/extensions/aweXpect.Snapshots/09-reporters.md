# Reporters

A reporter is told about the outcome of every snapshot, e.g. to open a diff tool or to collect the changed snapshots
of a test run. Derive from `SnapshotReporter` in the namespace `aweXpect.Snapshots` and add it to the reporters:

```csharp
public sealed class ChangedSnapshotsReporter : SnapshotReporter
{
    public ConcurrentBag<SnapshotId> Changed { get; } = new();

    public override ValueTask OnMismatchAsync(SnapshotMismatch mismatch, CancellationToken cancellationToken)
    {
        Changed.Add(mismatch.Id);
        return default;
    }
}

Customize.aweXpect.Global.Snapshots().Reporters.Set([
    .. Customize.aweXpect.Global.Snapshots().Reporters.Get(),
    new ChangedSnapshotsReporter(),
]);
```

| Member            | Called                                                                                             |
|-------------------|----------------------------------------------------------------------------------------------------|
| `OnMismatchAsync` | after the store recorded a missing or different snapshot, e.g. wrote the pending file              |
| `OnMatchAsync`    | when the snapshot matches, before the store records the match, e.g. deletes a leftover pending file |

Like the pending file, a mismatch is only reported when the whole expectation fails, e.g. not when another branch of an
`.Or` is met, and not for a negated snapshot. An accepted change is not reported, as it needs no review. Several
reporters are called in the order of the list, and never on a build server, the same detection as for
[accepting changes](02-accepting-snapshots.md). An exception of a reporter never changes the result of the test: it is
written to the trace writer of `Customize.aweXpect.EnableTracing(…)` and otherwise ignored.

`SnapshotMismatch` has the `Id` and the `Store` of the snapshot, the reviewed `Snapshot` (`null` when it did not exist)
and the `Actual` value, after the rules of the expectation were applied, e.g. with masked GUIDs. Both are a
`SnapshotContent` with the `Bytes` as the store keeps them (text as UTF-8 with an additional final line break), and,
unless it is [binary data](01-matching-snapshots.md#binary-data-and-files), the `Text` that was compared (`IsText`).
`SnapshotMatch` has the `Id` and the `Store`.

The reporter is not told about files, as a store need not keep any. A store that keeps the snapshots in files, like the
default store, implements `IFileSnapshotStore`, whose `GetSnapshotFile(id)` and `GetPendingFile(id)` return their
paths:

```csharp
if (mismatch.Store is IFileSnapshotStore files)
{
    Console.WriteLine($"Review {files.GetPendingFile(mismatch.Id)}");
}
```
