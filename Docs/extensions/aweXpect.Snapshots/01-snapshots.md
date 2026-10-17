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

## Matching a snapshot of an object

Any other value is written by the aweXpect formatter, completely and indented, so that a change in a single member
shows up as a changed line:

```csharp
await Expect.That(order).MatchesSnapshot();
```

```text title="OrderTests.Creates.snapshot.txt"
Order {
  Created = 2026-10-17T09:30:00.0000000Z,
  Id = 42,
  Lines = [
    Line {
      Price = 1.5,
      Product = "Pen"
    }
  ],
  Note = <null>,
  Status = Closed
}
```

- Members are written in alphabetical order, and nothing is shortened like in a failure message.
- Numbers and dates are written independently of the current culture.
- An object that contains itself is written as `*recursive*` where it repeats, and an object that is nested too deeply
  for the stack fails the expectation instead of being shortened.
- The static type decides: a `string` is compared as raw text, but a string in an `object` subject is written in quotes
  like any other value.
- A collection is written in the order it enumerates its items. The order of a hash-based collection, e.g. a
  `HashSet<string>`, can change between runs, so order it before, e.g. with `OrderBy`.
- Maskings apply to the written text, e.g. `.Masking(@"Id = ""[0-9a-f-]{36}""", @"Id = ""<id>""").AsRegex()`.

In a trimmed application or under Native AOT, the members of the subject's type are only known when the source
generator of aweXpect registers them for the call site, which it does for `MatchesSnapshot()`. A record is written with
its own `ToString()` there, as it can only be recognized with reflection, so prefer classes in such snapshots.

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

## Masking values

Values that change from run to run, such as ids or timestamps, can be replaced with a placeholder before the subject is
compared, so that the snapshot stays stable:

```csharp
await Expect.That(json).MatchesSnapshot()
    .Masking(order.Id.ToString(), "<order-id>");
```

The value is replaced as literal text. With `.AsRegex()` it is a regex pattern instead, and the replacement can use the
substitutions of `Regex.Replace`, e.g. `$1` for the first group, to keep part of the match (`$$` inserts a `$`):

```csharp
await Expect.That(log).MatchesSnapshot()
    .Masking(@"(\w+\.local):\d+", "$1:<port>").AsRegex();   // "api.local:5001" → "api.local:<port>"
```

The pattern has a timeout of one second. For other options or another timeout, pass a `Regex` instead:
`.Masking(new Regex(@"token=\w+", RegexOptions.IgnoreCase), "token=<token>")`. A pattern that times out fails the
expectation, and an invalid pattern throws an `ArgumentException` where it is written.

The maskings are applied in the order in which they are written, so put a specific masking before a general one:

```csharp
await Expect.That(report).MatchesSnapshot()
    .Masking(DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "<today>")
    .Masking(@"\d{4}-\d{2}-\d{2}", "<date>").AsRegex();
```

In the other order, the general masking would already have replaced today's date with `<date>`.

The snapshot, the pending and the accepted snapshot and the failure contain the masked value, as it is what is compared.
Two values that only differ in a masked part count as the same value for the check of [named
snapshots](#naming-a-snapshot).

### Guids and dates

`.MaskingGuids()` and `.MaskingDates()` replace every `Guid` and date with a numbered placeholder. The same value keeps
the same number, so relations stay visible in the snapshot:

```csharp
await Expect.That(log).MatchesSnapshot().MaskingGuids().MaskingDates();
```

```text title="Snapshot"
order Guid_1 created Date_1 by Guid_2
order Guid_1 shipped Date_2
```

- A `Guid` is matched in the format `"D"` (`3f2504e0-4f89-11d3-9a0c-0305e82c3301`) in upper or lower case, also inside
  braces or parentheses, which are kept. The format `"N"` without hyphens is not matched, as it cannot be told apart
  from a hash.
- A date is matched in ISO 8601: a date (`2026-10-17`) or a date with a time (`2026-10-17T09:30`), with optional seconds,
  a fraction of up to seven digits and an offset or `Z`. This covers how `DateTime`, `DateTimeOffset` and `DateOnly`
  write themselves with the round-trip format `"o"`, which the aweXpect formatter uses. Culture-specific formats, a space
  instead of the `T` and a time on its own are not matched; mask them with a regex.

The numbers are counted per subject in the order of appearance, and only after the maskings that come before, so
`.Masking(order.Id.ToString(), "<order-id>").MaskingGuids()` numbers only the other `Guid`s.

### For all snapshots

Maskings that apply to every snapshot are set with `Customize.aweXpect.Snapshots().Maskings`, e.g. in an assembly-level
setup, from the factories of `SnapshotMasking` in the namespace `aweXpect.Snapshots`:

```csharp
Customize.aweXpect.Global.Snapshots().Maskings.Set([
    SnapshotMasking.Guids(),
    SnapshotMasking.Dates(),
    SnapshotMasking.Text(Environment.MachineName, "<machine>"),
    SnapshotMasking.Regex(new Regex(@"localhost:\d+"), "localhost:<port>"),
]);
```

They are applied after the maskings of the expectation, in the order of the list, so a specific masking of the
expectation goes before a general one for all snapshots. Setting the value replaces the list; to extend an outer
setting, e.g. for one async flow, include it:

```csharp
using (Customize.aweXpect.Snapshots().Maskings.Set([
           .. Customize.aweXpect.Snapshots().Maskings.Get(),
           SnapshotMasking.Text(tenant.Name, "<tenant>"),
       ]))
{
    await Expect.That(report).MatchesSnapshot();
}
```

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
