---
description: Review and accept pending snapshots, accept many snapshots at once with AWEXPECT_ACCEPT_SNAPSHOTS and handle snapshots on a build server.
---

# Accepting snapshots

A missing or different snapshot writes the actual value to `<TestFile>.<Method>.awexpect-snapshot.pending.txt` next
to the snapshot. Review it and rename it to `<TestFile>.<Method>.awexpect-snapshot.txt` to accept it. The next run that
matches the snapshot deletes a leftover pending file.

Add the pending files to your `.gitignore`, so that they are not committed by accident; the pattern also covers
snapshots with [another extension](01-matching-snapshots.md#file-extension), e.g. `.awexpect-snapshot.pending.json`:

```text title=".gitignore"
*.awexpect-snapshot.pending.*
```

To accept many snapshots at once, set the environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` to one of the levels of
`SnapshotAcceptance` and run the tests:

| Level  | Variable                | A missing snapshot           | A different snapshot         |
|--------|-------------------------|------------------------------|------------------------------|
| `None` | `none`, `0` or `false`  | writes the pending file      | writes the pending file      |
| `New`  | `new`                   | is accepted                  | writes the pending file      |
| `All`  | `all`, `1` or `true`    | is accepted                  | is accepted                  |

An empty or unset variable accepts nothing.
`New` adds the snapshots of new tests without touching the reviewed ones. An accepted snapshot is replaced by the
actual value, and the expectation still fails, so that it never goes unnoticed:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but the snapshot did not exist

Actual:
Abbey Road by The Beatles

Accepted snapshot:
/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt
```

The next run compares with the accepted snapshot. Review the changed snapshots in version control before committing
them.

<details>
<summary>Snapshots combined with other expectations</summary>

The pending snapshot is only written, and a change only accepted, when the whole expectation fails. When the snapshot
is combined with other expectations and another branch is met, e.g. `.Or.IsEqualTo(…)` or `Expect.ThatAny(…)`, the
test passes and nothing is written, so that a passing test never changes a snapshot.

</details>

## On a build server

The environment variable is ignored on a build server (when `CI`, `TF_BUILD`, `JENKINS_URL` or `TEAMCITY_VERSION` is
set), so that a variable that was forgotten in a pipeline cannot change the snapshots there. To decide it in code
instead, which also applies on a build server:

```csharp
await That(description).MatchesSnapshot().Accepting(SnapshotAcceptance.All);  // missing or changed
await That(description).MatchesSnapshot().Accepting(SnapshotAcceptance.New);  // only missing
await That(description).MatchesSnapshot().Accepting(SnapshotAcceptance.None); // never accepts

Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.New);               // for this async flow
```

`Accepting(…)` takes precedence over `Customize.aweXpect.Snapshots().Acceptance`, which takes precedence over the
environment variable, so `Accepting(SnapshotAcceptance.None)` keeps a snapshot from being accepted while the variable
is set. The setting defaults to `null`, which leaves it to the environment variable.

To review a mismatch after a run on a build server, attach its pending file to the result of the test, when the
[test framework supports attachments](/aweXpect/setup/test-frameworks#the-running-test).
MSTest 4 needs the [opt-in](04-naming-snapshots.md#naming-after-the-test-case) `AweXpectExperimentalMSTest` for it:

```csharp
Customize.aweXpect.Snapshots().AttachPendingFiles.Set(true); // for this async flow
```
