# XML strings

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

await Expect.That(track).IsValidXml().Which(root => root.Satisfies(t => t!.Name == "track"));
```

The expectations are only verified when the string is valid XML. Like with
[`XDocument.Parse`](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument.parse), whitespace-only text
between elements is dropped, unless `xml:space="preserve"` keeps it.

You can also verify that a string is not valid XML:

```csharp
string track = "Let It Be";

await Expect.That(track).DoesNotComplyWith(it => it.IsValidXml());
```
