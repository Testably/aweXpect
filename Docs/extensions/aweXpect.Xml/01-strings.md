# XML strings

Describes the possible expectations for strings that contain XML.

| Expectation                                         | Negated                   | Summary                                                              |
|-----------------------------------------------------|---------------------------|----------------------------------------------------------------------|
| [`IsEqualTo(…).AsXml()`](#string-comparison-as-xml) | `IsNotEqualTo(…).AsXml()` | equal to the expected XML, compared by content                       |
| [`IsValidXml`](#validation)                         |                           | a well-formed XML document, optionally with expectations on its root |
| [`IsValidXmlMatching`](#validation)                 |                           | valid XML that matches the expected XML                              |
| [`IsValidXmlMatchingExactly`](#validation)          |                           | valid XML that matches the expected XML exactly                      |

## String comparison as XML

You can verify that a string is equal to the expected XML, compared by its content instead of its formatting:

```csharp
string album = """<album id="42"><track title="Come Together" durationSeconds="259.0"/></album>""";
string expected = """
                  <album id="42">
                    <track durationSeconds="259" title="Come Together" />
                  </album>
                  """;

await Expect.That(album).IsEqualTo(expected).AsXml();
```

As the duration differs, the expectation fails with:

```text title="Failure message"
Expected that album
is XML equivalent to <album id="42">
  <track durationSeconds="259" title="Come Together" />
</album>,
but it differed as /album/track/@durationSeconds was "259.0" instead of "259"

Actual:
<album id="42"><track title="Come Together" durationSeconds="259.0"/></album>

Expected:
<album id="42">
  <track durationSeconds="259" title="Come Together" />
</album>
```

The documents are equivalent, when they only differ in

- the order of the attributes,
- the prefixes of namespaces, as elements and attributes are compared by their namespace and local name,
- whitespace-only text between elements, unless `xml:space="preserve"` keeps it,
- comments, processing instructions and the XML declaration,
- CDATA sections instead of escaped text, or `<a/>` instead of `<a></a>`.

Texts and attribute values are compared exactly, including leading and trailing whitespace, and the order of the
child elements is significant, unless you [ignore the element order](./02-matching.md#element-order).

Each difference names the path of the node: every element by its local name, followed by its position among the
siblings of the same name, if there are several, e.g. `/album/track[2]/@title`, and every text as `text()`. The child
elements are paired by this path, so an additional element is reported as unexpected instead of shifting the elements
after it, and a different order is reported once for their parent.

You can [ignore members](./02-matching.md#ignoring-members) whose values change between runs, e.g.
`AsXml(o => o.IgnoringMember("@createdAt"))`, or [ignore the additional nodes](./02-matching.md#additional-nodes) of
the subject to compare it with a part of the XML, e.g. `AsXml(o => o.IgnoringAdditionalNodes())`.

If the subject is not valid XML, the expectation fails with the reason:

```text title="Failure message"
Expected that album
is XML equivalent to <album id="42" />,
but it was "<album id=\"42\">", which could not be parsed as XML: Unexpected end of file has occurred. The following elements are not closed: album. Line 1, position 16.

Actual:
<album id="42">

Expected:
<album id="42" />
```

The expected string must be valid XML, otherwise an `ArgumentException` is thrown. Like for [validation](#validation),
a document type definition (DTD) is prohibited in both strings. As XML is compared by its content, `AsXml()` cannot be
combined with `IgnoringCase()`, `Using(comparer)` or another match type like `AsWildcard()`, and it is not available
on `Contains` or `DoesNotContain` for a string subject.

`AsXml()` is also available on other string comparisons, e.g. on the items of a collection or on the message of an
exception:

```csharp
await Expect.That(albums).All().AreEqualTo("""<album id="42" />""").AsXml();
await Expect.That(albums).Contains("""<album id="42" />""").AsXml();
await Expect.That(Act).Throws<InvalidOperationException>()
    .WithMessage("""<error code="42" />""").AsXml();
```

The options are available there as well, e.g. to find an item that contains the expected XML:

```csharp
await Expect.That(albums).Contains("""<album id="42" />""")
    .AsXml(o => o.IgnoringAdditionalNodes());
```

## Validation

You can verify that a string is a well-formed XML document:

```csharp
string track = """<track title="Let It Be" artist="The Beatles" />""";

await Expect.That(track).IsValidXml();
```

If the closing tag was missing, i.e. `<track title="Let It Be" artist="The Beatles">`, the expectation would fail
with:

```text title="Failure message"
Expected that track
is valid XML,
but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: track. Line 1, position 47.
```

This verifies that the string can be read to its end by an
[`XmlReader`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader) without exceptions. The string must
contain exactly one root element; a fragment like `<a/><b/>` or an empty string is not valid XML. The document is
not validated against a schema, and an encoding in the XML declaration is not checked, as the string is already
decoded.

For security reasons, a document with a document type definition (`<!DOCTYPE ...>`) is not valid XML: DTD processing
is prohibited and external resources are never resolved, so that verifying untrusted input is not exposed to XML
external entity (XXE) or entity expansion attacks:

```text title="Failure message"
Expected that track
is valid XML,
but it contained a document type definition (DTD), which is prohibited for security reasons
```

You can add further expectations on the root
[`XElement`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) parsed from the subject:

```csharp
string track = """<track title="Let It Be" artist="The Beatles" />""";

await Expect.That(track).IsValidXml(
    root => root.Matches("""<track title="Let It Be" />"""));
```

The expectations are only verified when the string is valid XML. Like with
[`XDocument.Parse`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument.parse), whitespace-only text
between elements is dropped, unless `xml:space="preserve"` keeps it.

Or you can verify that the string is valid XML and [matches](./02-matching.md) an expected XML in one step:

```csharp
string track = """<track title="Let It Be" artist="The Beatles" />""";

await Expect.That(track).IsValidXmlMatching("""<track title="Let It Be" />""");
await Expect.That(track).IsValidXmlMatchingExactly(
    """<track artist="The Beatles" title="Let It Be" />""");
```

`IsValidXmlMatching("""<track title="Yesterday" />""")` would fail with:

```text title="Failure message"
Expected that track
is valid XML which matches <track title="Yesterday" />,
but it differed as /track/@title was "Let It Be" instead of "Yesterday"

XML options (it):
 - ignore additional nodes
```

You can also verify that a string is not valid XML:

```csharp
string track = "Let It Be";

await Expect.That(track).DoesNotComplyWith(it => it.IsValidXml());
```
