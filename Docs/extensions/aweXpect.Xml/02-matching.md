# Matching XML

Describes the possible expectations for matching an `XElement` or an `XDocument` with an expected XML, and the
[options](#options) to relax the comparison.

| Expectation                                 | Negated | Summary                                            |
|---------------------------------------------|---------|----------------------------------------------------|
| [`Matches`](#elements-and-documents)        |         | matches the expected XML, additional nodes allowed |
| [`MatchesExactly`](#elements-and-documents) |         | matches the expected XML exactly                   |

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
`MatchesExactly` fails for them: `Matches` is `MatchesExactly` with the option
[`IgnoringAdditionalNodes()`](#additional-nodes). Missing nodes, different values and a different order of the
expected child elements fail both, unless you [ignore the element order](#element-order). If the title was "Yesterday"
instead, `Matches` would fail with:

```text title="Failure message"
Expected that track
matches <track title="Let It Be" />,
but it differed as /track/@title was "Yesterday" instead of "Let It Be"

XML options:
 - ignore additional nodes
```

The elements are compared by their content like by [`AsXml()`](./01-strings.md#string-comparison-as-xml): attributes
regardless of their order, elements and attributes by their namespace and local name instead of their prefix, and
without comments and processing instructions. Whitespace-only text between elements is ignored.

For an [`XDocument`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument), its root element is
compared:

```csharp
XDocument document = XDocument.Parse("""<?xml version="1.0"?><track title="Let It Be" />""");

await Expect.That(document).Matches("""<track title="Let It Be" />""");
```

You can also verify that an element does not match an expected XML:

```csharp
XElement track = XElement.Parse("""<track title="Let It Be" />""");

await Expect.That(track).DoesNotComplyWith(it => it.Matches("""<track title="Yesterday" />"""));
```

<details>
<summary>Whitespace, paths and documents without a root element</summary>

Whitespace-only text between elements is ignored also when the subject was loaded with
`LoadOptions.PreserveWhitespace`, unless `xml:space="preserve"` on the subject or one of its ancestors keeps it. The
paths of the differences start at the subject, also when it is nested in a document. A document without a root element
fails, also when the expectation is negated.

</details>

## Options

The options are available wherever `XmlOptions` are accepted, i.e. `Matches`, `MatchesExactly`, `IsValidXmlMatching`,
`IsValidXmlMatchingExactly` and [`AsXml(o => …)`](./01-strings.md#string-comparison-as-xml). They work like the
[options of aweXpect](https://docs.testably.org/aweXpect/how-it-works/options):

| Option                                                                    | Configures                                                   |
|---------------------------------------------------------------------------|--------------------------------------------------------------|
| [`IgnoringAdditionalNodes()`](#additional-nodes)                          | ignores additional attributes and child nodes of the subject |
| [`IgnoringMember(path)`, `IgnoringMembers(path => …)`](#ignoring-members) | leaves members out of the comparison                         |
| [`IgnoringElementOrder()`](#element-order)                                | the child elements and texts can be in any order             |

### Additional nodes

You can ignore the attributes and child nodes of the subject that are not in the expected XML. The option is available
for `MatchesExactly`, `IsValidXmlMatchingExactly` and [`AsXml()`](./01-strings.md#string-comparison-as-xml), and
`Matches` and `IsValidXmlMatching` use it by default:

```csharp
string album = """<album id="42" year="1969"><track title="Come Together" /></album>""";

await Expect.That(album).IsEqualTo("""<album id="42" />""").AsXml(o => o.IgnoringAdditionalNodes());
```

The additional nodes are ignored on every level, and the expected nodes are still compared, so that missing nodes,
different values and a different order of the expected child elements fail the expectation. The option is listed when
the expectation fails:

```text title="Failure message"
XML options:
 - ignore additional nodes
```

With `IgnoringAdditionalNodes(false)`, `Matches` and `IsValidXmlMatching` compare exactly, like `MatchesExactly` and
`IsValidXmlMatchingExactly`.

### Ignoring members

You can ignore members whose values change between runs, e.g. generated ids or timestamps. The options are available
for `Matches`, `MatchesExactly`, `IsValidXmlMatching`, `IsValidXmlMatchingExactly` and
[`AsXml()`](./01-strings.md#string-comparison-as-xml):

```csharp
string album = """<album id="42" createdAt="2026-10-17"><track title="Come Together" etag="x1" /></album>""";

await Expect.That(album).IsEqualTo("""<album id="1"><track title="Come Together" /></album>""")
    .AsXml(o => o
        .IgnoringMember("/album/@id")
        .IgnoringMember("@createdAt")
        .IgnoringMembers(path => path.EndsWith("/@etag")));
```

`IgnoringMember` matches whole trailing segments of the [path](./01-strings.md#string-comparison-as-xml):
`"@createdAt"` ignores the attribute on every element and `"track/@id"` the attribute of every `track`, while a path
starting with `/`, e.g. `"/album/@id"`, only ignores this member. A segment without a position, e.g. `track`, matches
every element of the name, and `track[2]` only the second. The names are compared case-sensitively, as in XML. Like
the paths, they are local names, so that `"track"` ignores a `track` in every namespace, unlike
[`HasElement`](./03-navigation.md#names-and-namespaces), which looks up the name with its namespace. `IgnoringMembers`
receives the path of every compared member instead.

An ignored attribute, element or text, and everything below it, is neither compared nor reported as missing or
unexpected, and an ignored element is not considered for the order of its siblings. The ignored members are listed
when the expectation fails:

```text title="Failure message"
XML options:
 - ignore members: ["/album/@id", "@createdAt", path => path.EndsWith("/@etag")]
```

A member to ignore that matched no compared path is named in a failure, with the path it most likely meant, as a
misspelled member to ignore would otherwise ignore nothing without notice:

```text title="Failure message"
XML options:
 - ignore members: ["@createdat"]
   "@createdat" matched no member (did you mean "@createdAt"?)
```

<details>
<summary>Members that only occur in the subject</summary>

As [additional nodes](#additional-nodes) of the subject are not compared when they are ignored, e.g. by `Matches`, a
member to ignore that only occurs in the subject matches no compared path either, and is named as well, although it
is not needed.

</details>

### Element order

By default, the order of the child elements and texts is significant. You can ignore it for all compared elements:

```csharp
XElement album = XElement.Parse("""
                                <album>
                                  <track title="Something" />
                                  <track title="Come Together" />
                                </album>
                                """);

await Expect.That(album).MatchesExactly("""
                                        <album>
                                          <track title="Come Together" />
                                          <track title="Something" />
                                        </album>
                                        """,
    o => o.IgnoringElementOrder());
```

The texts are paired like the elements, so that the order of the texts in mixed content, e.g. `<p>a<b/>c</p>`, is
ignored as well.

<details>
<summary>How the children are paired</summary>

Each expected child is paired with the first unpaired child of the subject with the same name that matches it. The
remaining children of the same name are paired in their order, so that their differences are reported, e.g.
`/album/track[1]/@title was "Here Comes the Sun" instead of "Come Together"`, and the children that are still
unpaired are reported as missing or unexpected. This greedy pairing can fail when [additional nodes](#additional-nodes)
are ignored, e.g. by `Matches`, and an expected child matches several children of the subject, even if another pairing
would succeed.

As the children are no longer paired by their position, a position in a [member to ignore](#ignoring-members), e.g.
`track[2]`, refers to the child at this position in each document.

</details>
