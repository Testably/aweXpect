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
fail both, unless you [ignore the element order](#element-order). If the title was "Yesterday" instead, `Matches`
would fail with:

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

## Ignoring members

You can ignore members whose values change between runs, e.g. generated ids or timestamps. The options are available
for `Matches`, `MatchesExactly`, `IsValidXmlMatching`, `IsValidXmlMatchingExactly` and
[`AsXml()`](./01-strings.md#string-comparison-as-xml):

```csharp
string order = """<order id="42" createdAt="2026-10-17"><item sku="A1" etag="x1" /></order>""";

await Expect.That(order).IsEqualTo("""<order id="1"><item sku="A1" /></order>""")
    .AsXml(o => o
        .IgnoringMember("/order/@id")
        .IgnoringMember("@createdAt")
        .Ignoring(path => path.EndsWith("/@etag")));
```

`IgnoringMember` matches whole trailing segments of the [path](./01-strings.md#string-comparison-as-xml):
`"@createdAt"` ignores the attribute on every element and `"item/@id"` the attribute of every `item`, while a path
starting with `/`, e.g. `"/order/@id"`, only ignores this member. A segment without a position, e.g. `item`, matches
every element of the name, and `item[2]` only the second. The names are compared case-sensitively, as in XML. Like
the paths, they are local names, so that `"item"` ignores an `item` in every namespace, unlike
[`HasElement`](./03-navigation.md#names-and-namespaces), which looks up the name with its namespace. `Ignoring`
receives the path of every compared member instead.

An ignored attribute, element or text, and everything below it, is neither compared nor reported as missing or
unexpected, and an ignored element is not considered for the order of its siblings. The ignored members are listed
when the expectation fails:

```text title="Failure message"
XML options:
 - ignore members: ["/order/@id", "@createdAt", path => path.EndsWith("/@etag")]
```

A member to ignore that matched no compared path is named in a failure, with the path it most likely meant, as a
misspelled member to ignore would otherwise ignore nothing without notice:

```text title="Failure message"
XML options:
 - ignore members: ["@createdat"]
   "@createdat" matched no member (did you mean "@createdAt"?)
```

## Element order

By default, the order of the child elements and texts is significant. You can ignore it for all compared elements:

```csharp
XElement order = XElement.Parse("""<order><item sku="B2" /><item sku="A1" /></order>""");

await Expect.That(order).MatchesExactly("""<order><item sku="A1" /><item sku="B2" /></order>""",
    o => o.IgnoringElementOrder());
```

Each expected child is paired with the first unpaired child of the subject with the same name that matches it. The
remaining children of the same name are paired in their order, so that their differences are reported, e.g.
`/order/item[1]/@sku was "C3" instead of "A1"`, and the children that are still unpaired are reported as missing or
unexpected. This greedy pairing can fail for `Matches`, which ignores additional nodes, when an expected child matches
several children of the subject, even if another pairing would succeed.

The texts are paired like the elements, so that the order of the texts in mixed content, e.g. `<p>a<b/>c</p>`, is
ignored as well.

As the children are no longer paired by their position, a position in a [member to ignore](#ignoring-members), e.g.
`item[2]`, refers to the child at this position in each document.
