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
but the snapshot was already used for another value in this test run, so name each one with .Named(…) or .NamedAfter(…)
```

The same value again is allowed, so that a retried or repeated test still compares with its snapshot.

The test file and method are set by the compiler through optional parameters, so a name must not be passed as an
argument: `MatchesSnapshot("login")` throws an `ArgumentException` that points to `.Named(…)`.

## Naming after the arguments

`.NamedAfter(…)` names the snapshot after up to four arguments of the test. The compiler passes the name of each
argument, so it works with every test framework and under Native AOT:

```csharp
[Theory]
[InlineData("Abbey Road", 1969)]
[InlineData("Let It Be", 1970)]
public async Task Describes(string title, int year)
{
    string description = Describe(new Album(title, year));

    await Expect.That(description).MatchesSnapshot().NamedAfter(title, year);
}
```

Each argument is written as `<name>=<value>`, separated by `_`, e.g.
`AlbumTests.Describes.title=Abbey Road_year=1969.snapshot.txt`. A member access is named like `nameof`, so
`.NamedAfter(album.Title)` gives `Title=Abbey Road`; any other expression is used as written. Values are written in
the invariant culture, `null` as `null`, and escaped like any other name.
