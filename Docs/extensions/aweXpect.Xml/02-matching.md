# Matching XML

## Elements and documents

You can verify that an [`XElement`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) matches an
expected XML:

```csharp
XElement track = XElement.Parse("""<track title="Let It Be" artist="The Beatles"><genre>rock</genre></track>""");

await Expect.That(track).Matches("""<track title="Let It Be" />""");
await Expect.That(track).MatchesExactly("""
                                        <track artist="The Beatles" title="Let It Be">
                                          <genre>rock</genre>
                                        </track>
                                        """);
```

`Matches` ignores the attributes and child nodes of the subject that are not in the expected XML, while
`MatchesExactly` fails for them. Missing nodes, different values and a different order of the expected child elements
fail both. If the title was "Yesterday" instead, `Matches` would fail with:

```text title="Failure message"
Expected that track
matches <track title="Let It Be" />,
but it differed as /track/@title was "Yesterday" instead of "Let It Be"
```

The elements are compared by their content like by [`AsXml()`](./01-strings.md#string-comparison-as-xml): attributes
regardless of their order, elements and attributes by their namespace and local name instead of their prefix, and
without whitespace-only text between elements, comments and processing instructions. The paths of the differences
start at the subject, also when it is nested in a document.

For an [`XDocument`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument), its root element is
compared, so that a document without a root element fails, also when the expectation is negated:

```csharp
XDocument document = XDocument.Parse("""<?xml version="1.0"?><track title="Let It Be" />""");

await Expect.That(document).Matches("""<track title="Let It Be" />""");
```

The expected string must be valid XML without a document type definition (DTD), otherwise an `ArgumentException` is
thrown when the expectation is specified.

You can also verify that an element does not match an expected XML:

```csharp
XElement track = XElement.Parse("""<track title="Let It Be" />""");

await Expect.That(track).DoesNotComplyWith(it => it.Matches("""<track title="Yesterday" />"""));
```
