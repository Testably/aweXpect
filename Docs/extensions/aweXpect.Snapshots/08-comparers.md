# Comparers

By default, a text snapshot matches when it is equal apart from the newline style, and
[binary data](01-matching-snapshots.md#binary-data-and-files) when every byte is equal. A comparer decides it instead,
e.g. for cover art that is rendered with slightly different pixels on every machine. Derive from `SnapshotComparer` in
the namespace `aweXpect.Snapshots`:

```csharp
public sealed class CoverArtComparer(double minimumSimilarity) : SnapshotComparer
{
    public override ValueTask<SnapshotComparison> CompareAsync(SnapshotComparisonContext context,
        CancellationToken cancellationToken)
    {
        double similarity = Images.Similarity(context.Snapshot.Bytes, context.Actual.Bytes);
        return new ValueTask<SnapshotComparison>(similarity >= minimumSimilarity
            ? SnapshotComparison.Equal
            : SnapshotComparison.Different($"only {similarity:P1} of the pixels are the same"));
    }
}
```

`SnapshotComparisonContext` has the `Id` of the snapshot (e.g. its `Extension`), the reviewed `Snapshot` and the
`Actual` value, after the rules of the expectation were applied, e.g. with masked GUIDs. Both are a `SnapshotContent`
like for [reporters](09-reporters.md): the `Bytes` as the store keeps them, and the `Text` unless it is binary data.

Use the comparer for one expectation with `.ComparedWith(…)`, for strings, objects and binary data:

```csharp
await Expect.That(cover).MatchesSnapshot().WithExtension("png").ComparedWith(new CoverArtComparer(0.99));
```

or for all snapshots that a condition selects, with the shorthand `ForExtension` (ignoring the case of the extension)
or with `When` and a predicate that sees the whole context:

```csharp
Customize.aweXpect.Global.Snapshots().Comparers.Set([
    SnapshotComparer.When(context => context.Id.Extension == "png" && context.Actual.Bytes.Length > 1_000,
        new CoverArtComparer(0.99)),
    SnapshotComparer.ForExtension("svg", new SheetMusicComparer()),
]);
```

The comparer of the expectation comes first, then the first comparer of the setting whose condition matches, in the
order of the list, and otherwise the built-in comparison.

- A comparer is only called when the snapshot exists; a missing snapshot fails as before.
- When the comparer considers the value equal, the snapshot matches and is kept unchanged, even while accepting
  changes, and a leftover pending snapshot is deleted. Otherwise the failure shows its description, e.g.
  `but it differed: only 97.2% of the pixels are the same`, and the value is recorded for review or accepted like any
  other difference.
- A snapshot that is used again in the same test run for a value with other bytes, e.g. by a repeated test, is no
  conflict when the comparer considers both values equal: it gets the first value of the test run as `Snapshot`. The
  first value is only kept while a comparer is set for the expectation or in the setting. Tests run in parallel, so a
  comparer can be called concurrently and must be thread-safe.
- An exception of a comparer or of its condition fails the expectation both ways, with the exception as its cause, and
  neither records nor accepts the value.

## Testing a comparer

`SnapshotContent.FromText(…)` and `SnapshotContent.FromBytes(…)` create the contents like a store keeps them, so that a
comparer can be tested on its own:

```csharp
SnapshotComparisonContext context = new(new SnapshotId("AlbumTests.cs", "Cover"),
    SnapshotContent.FromBytes(File.ReadAllBytes("abbey-road.png")),
    SnapshotContent.FromBytes(RenderCover("Abbey Road")));

SnapshotComparison comparison = await new CoverArtComparer(0.99).CompareAsync(context, CancellationToken.None);

await Expect.That(comparison.IsEqual).IsTrue();
```
