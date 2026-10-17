# Naming snapshots

The cases of a parameterized test share the name of the test method, so give each case its own snapshot with
`.Named(…)`:

```csharp
[Theory]
[InlineData("Abbey Road")]
[InlineData("Let It Be")]
public async Task Describes(string title)
{
    string description = Describe(new Album(title));

    await Expect.That(description).MatchesSnapshot().Named(title);
}
```

The name is appended to the file names, e.g. `AlbumTests.Describes.Abbey Road.snapshot.txt`. The same applies to several
snapshots in one test method. Characters that a file name cannot hold on every operating system
(`< > : " / \ | ? *` and control characters), as well as `%` and `.`, are escaped as `%XX` in the file name, e.g.
`.Named("AC/DC")` is stored in `ArtistTests.Describes.AC%2FDC.snapshot.txt`.

A snapshot that was already used for another value in the same test run fails, so that the cases cannot overwrite each
other's pending snapshot:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.snapshot.txt",
but the snapshot was already used for another value in this test run, so name each one with .Named(…)
```

The same value again is allowed, so that a retried or repeated test still compares with its snapshot.

The test file and method are set by the compiler through optional parameters, so a name must not be passed as an
argument: `MatchesSnapshot("login")` throws an `ArgumentException` that points to `.Named(…)`.
