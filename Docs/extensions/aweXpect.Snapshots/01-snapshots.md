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

## Files

The snapshot files are encoded as UTF-8. The pending file ends with an additional line break, which is removed again
when the snapshot is read, so that an editor which adds a final line break does not change the snapshot.

The newline style is ignored when comparing, because version control can change it when the snapshot is checked out.

A snapshot that cannot be read, e.g. because another process locks it, fails the expectation with the reason, also when
it is negated. A pending file that cannot be written is named as such in the failure, and a leftover pending file that
cannot be deleted does not fail a matching snapshot.
