# Accepting snapshots

A missing or different snapshot writes the actual value to `<Class>.<Method>.snapshot.pending.txt` next to the
snapshot. Review it and rename it to `<Class>.<Method>.snapshot.txt` to accept it. The next run that matches the
snapshot deletes a leftover pending file.

The pending snapshot is only written, and a change only accepted, when the whole expectation fails. When the snapshot
is combined with other expectations and another branch is met, e.g. `.Or.IsEqualTo(…)` or `Expect.ThatAny(…)`, the
test passes and nothing is written, so that a passing test never changes a snapshot.

Add the pending files to your `.gitignore`, so that they are not committed by accident:

```text title=".gitignore"
*.snapshot.pending.txt
```

To accept many snapshots at once, set the environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` (to anything but `0` or
`false`) and run the tests. A missing or different snapshot is then replaced by the actual value, and the expectation
still fails, so that an accepted snapshot never goes unnoticed:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.snapshot.txt",
but the snapshot did not exist

Actual:
Abbey Road by The Beatles

Accepted snapshot:
/src/Tests/AlbumTests.Describes.snapshot.txt
```

The next run compares with the accepted snapshot. Review the changed snapshots in version control before committing
them.

The environment variable is ignored on a build server (when `CI`, `TF_BUILD`, `JENKINS_URL` or `TEAMCITY_VERSION` is
set), so that a variable that was forgotten in a pipeline cannot change the snapshots there. To decide it in code
instead, which also applies on a build server:

```csharp
await Expect.That(description).MatchesSnapshot().AcceptingChanges();          // accepts
await Expect.That(description).MatchesSnapshot().AcceptingChanges(isLocalRun); // accepts if isLocalRun is true

Customize.aweXpect.Snapshots().AcceptChanges.Set(true);                        // for the current async flow
```

`AcceptingChanges(…)` takes precedence over `Customize.aweXpect.Snapshots().AcceptChanges`, which takes precedence
over the environment variable. `AcceptingChanges(…)` is always the last option of the expectation, so that it is easy to
find and remove again.
