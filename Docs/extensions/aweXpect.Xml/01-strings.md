---
description: Compare strings as XML by their content and verify that a string is a well-formed XML document.
---

# XML strings

Describes the possible expectations for strings that contain XML.

| Expectation                                         | Negated                   | Summary                                                              |
|-----------------------------------------------------|---------------------------|----------------------------------------------------------------------|
| [`IsEqualTo(…).AsXml()`](#string-comparison-as-xml) | `IsNotEqualTo(…).AsXml()` | equal to the expected XML, compared by content                       |
| [`IsValidXml`](#validation)                         | `IsNotValidXml`           | a well-formed XML document, optionally with expectations on its root |
| [`IsValidXmlMatching`](#validation)                 |                           | valid XML that matches the expected XML                              |
| [`IsValidXmlMatchingExactly`](#validation)          |                           | valid XML that matches the expected XML exactly                      |

## String comparison as XML

You can verify that a string is equal to the expected XML, compared by its content instead of its formatting:

```csharp
string album = """<album id="42"><track title="Come Together" durationSeconds="259"/></album>""";
string expected = """
                  <album id="42">
                    <track durationSeconds="259" title="Come Together" />
                  </album>
                  """;

await That(album).IsEqualTo(expected).AsXml();
```

If the duration was `259.0` instead, the expectation would fail with:

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
siblings of the same name, if there are several, e.g. `/album/track[2]/@title`, and every text as `text()`.

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

As XML is compared by its content, `AsXml()` cannot be combined with `IgnoringCase()`, `Using(comparer)` or another
match type like `AsWildcard()`, and it is not available on `Contains` or `DoesNotContain` for a string subject.

`AsXml()` is also available on other string comparisons, e.g. on the items of a collection or on the message of an
exception:

```csharp
await That(albums).All().AreEqualTo("""<album id="42" />""").AsXml();
await That(albums).Contains("""<album id="42" />""").AsXml();
await That(Act).Throws<InvalidOperationException>().WithMessage("""<error code="42" />""").AsXml();
```

The options are available there as well, e.g. to find an item that contains the expected XML:

```csharp
await That(albums).Contains("""<album id="42" />""").AsXml(o => o.IgnoringAdditionalNodes());
```

<details>
<summary>Pairing of child elements and DTDs</summary>

The child elements are paired by their path, so an additional element is reported as unexpected instead of shifting
the elements after it, and a different order is reported once for their parent.

Like for [validation](#validation), a document type definition (DTD) is prohibited in both strings.

</details>

## Validation

You can verify that a string is a well-formed XML document:

```csharp
string track = """<track title="Let It Be" artist="The Beatles" />""";

await That(track).IsValidXml();
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
not validated against a schema.

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

await That(track).IsValidXml(root => root.Matches("""<track title="Let It Be" />"""));
```

The expectations are only verified when the string is valid XML.

Or you can verify that the string is valid XML and [matches](./02-matching.md) an expected XML in one step:

```csharp
string track = """<track title="Let It Be" artist="The Beatles" />""";

await That(track).IsValidXmlMatching("""<track title="Let It Be" />""");
await That(track).IsValidXmlMatchingExactly("""<track artist="The Beatles" title="Let It Be" />""");
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

await That(track).IsNotValidXml();
```

If the string is valid XML, e.g. `<track title="Let It Be" />`, the expectation fails with:

```text title="Failure message"
Expected that track
is not valid XML,
but it was "<track title=\"Let It Be\" />"
```

<details>
<summary>Parsing, `null` subjects and negation</summary>

An encoding in the XML declaration is not checked, as the string is already decoded. Like with
[`XDocument.Parse`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument.parse), whitespace-only text
between elements is dropped for the expectations on the root element, unless `xml:space="preserve"` keeps it.

With expectations on the root element and with `IsValidXmlMatching` or `IsValidXmlMatchingExactly`, a string that is
not valid XML fails also when the expectation is negated, e.g. within `DoesNotComplyWith(…)`, as there is no root
element to verify. Only the negation of `IsValidXml()` without expectations is met by a string that is not valid XML.

`IsNotValidXml` also succeeds for a document with a document type definition, but fails for a `null` subject like
`IsValidXml`.

</details>
