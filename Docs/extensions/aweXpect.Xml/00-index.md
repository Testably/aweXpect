# aweXpect.Xml

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Xml)](https://www.nuget.org/packages/aweXpect.Xml)

[aweXpect.Xml](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Xml) contains expectations for XML strings and `System.Xml.Linq`.

## At a glance

```csharp
string xml = """<track title="Let It Be" artist="The Beatles" />""";

// verify that a string is a well-formed XML document
await Expect.That(xml).IsValidXml();

// verify that a string is equal to the expected XML, compared by its content instead of its formatting
await Expect.That(xml).IsEqualTo("""<track artist="The Beatles" title="Let It Be"></track>""").AsXml();

// verify that an XElement or XDocument contains the expected XML, ignoring additional nodes
XElement track = XElement.Parse("""<track title="Let It Be" artist="The Beatles"><genre>rock</genre></track>""");
await Expect.That(track).Matches("""<track title="Let It Be"><genre>rock</genre></track>""");

// verify single attributes, child elements and their values
await Expect.That(track).HasAttribute("artist").And
    .HasElement("genre", genre => genre.HasValue(value => value.IsEqualTo("rock")));
```

If the element was not closed, i.e. `<track title="Let It Be" artist="The Beatles">`, the expectation would fail with:

```text title="Failure message"
Expected that xml
is valid XML,
but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: track. Line 1, position 47.
```

## Overview

| Expectation                                                        | Subject                 | Summary                                                              |
|--------------------------------------------------------------------|-------------------------|----------------------------------------------------------------------|
| [`IsEqualTo(…).AsXml()`](./01-strings.md#string-comparison-as-xml) | `string`                | equal to the expected XML, compared by content                       |
| [`IsValidXml`](./01-strings.md#validation)                         | `string`                | a well-formed XML document, optionally with expectations on its root |
| [`IsNotValidXml`](./01-strings.md#validation)                      | `string`                | not a well-formed XML document                                       |
| [`IsValidXmlMatching`](./01-strings.md#validation)                 | `string`                | valid XML that matches the expected XML                              |
| [`IsValidXmlMatchingExactly`](./01-strings.md#validation)          | `string`                | valid XML that matches the expected XML exactly                      |
| [`Matches`](./02-matching.md#elements-and-documents)               | `XElement`, `XDocument` | matches the expected XML, additional nodes allowed                   |
| [`MatchesExactly`](./02-matching.md#elements-and-documents)        | `XElement`, `XDocument` | matches the expected XML exactly                                     |
| [`HasAttribute`](./03-navigation.md#attributes)                    | `XElement`, `XDocument` | has the attribute, optionally with expectations on its value         |
| [`DoesNotHaveAttribute`](./03-navigation.md#attributes)            | `XElement`, `XDocument` | does not have the attribute                                          |
| [`HasElement`](./03-navigation.md#child-elements)                  | `XElement`, `XDocument` | has the child element, optionally with expectations on it            |
| [`DoesNotHaveElement`](./03-navigation.md#child-elements)          | `XElement`, `XDocument` | does not have the child element                                      |
| [`HasValue`](./03-navigation.md#values)                            | `XElement`, `XDocument` | the value satisfies the expectations                                 |

The comparisons can be relaxed with the options `IgnoringAdditionalNodes()`, `IgnoringElementOrder()` and
`IgnoringMember(path)` or `Ignoring(path => …)`, see [Matching XML](./02-matching.md#options).

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/):

- [XML strings](./01-strings.md): comparing and validating strings as XML
- [Matching XML](./02-matching.md): `Matches` and `MatchesExactly`, and the options to relax the comparison
- [Attributes and child elements](./03-navigation.md): verifying single attributes, child elements and values
