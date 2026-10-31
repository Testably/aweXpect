# aweXpect.Xml

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Xml)](https://www.nuget.org/packages/aweXpect.Xml)

[aweXpect.Xml](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Xml) extends [aweXpect](https://docs.testably.org/aweXpect) with expectations for XML strings and `System.Xml.Linq`.

## Installation

```shell
dotnet add package aweXpect.Xml --prerelease
```

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well, e.g. for the `IsEqualTo` of `IsEqualTo(…).AsXml()`.
The expectations live in the namespace `aweXpect`, so `using aweXpect;` is all you need.

The examples write `That(...)` instead of `Expect.That(...)`, which needs `using static aweXpect.Expect;` (see [Getting started](https://docs.testably.org/aweXpect/getting-started#install-the-package)).

## At a glance

```csharp
string xml = """<track title="Let It Be" artist="The Beatles" />""";

// verify that a string is a well-formed XML document
await That(xml).IsValidXml();

// verify that a string is equal to the expected XML, compared by its content instead of its formatting
await That(xml).IsEqualTo("""<track artist="The Beatles" title="Let It Be"></track>""").AsXml();

// verify that an XElement or XDocument contains the expected XML, ignoring additional nodes
XElement track = XElement.Parse("""<track title="Let It Be" artist="The Beatles"><genre>rock</genre></track>""");
await That(track).Matches("""<track title="Let It Be"><genre>rock</genre></track>""");

// verify single attributes, child elements and their values
await That(track).HasAttribute("artist").EqualTo("The Beatles").And
    .HasElement("genre", genre => genre.HasValue().EqualTo("rock"));
```

If the element was not closed, i.e. `<track title="Let It Be" artist="The Beatles">`, the expectation would fail with:

```text
Expected that xml
is valid XML,
but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: track. Line 1, position 47.
```

## Overview

Expectations for [`string`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/strings):

| Expectation                                                                                                           | Negated                   | Summary                                                              |
|-----------------------------------------------------------------------------------------------------------------------|---------------------------|----------------------------------------------------------------------|
| [`IsEqualTo(…).AsXml()`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/strings#string-comparison-as-xml) | `IsNotEqualTo(…).AsXml()` | equal to the expected XML, compared by content                       |
| [`IsValidXml`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/strings#validation)                         | `IsNotValidXml`           | a well-formed XML document, optionally with expectations on its root |
| [`IsValidXmlMatching`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/strings#validation)                 |                           | valid XML that matches the expected XML                              |
| [`IsValidXmlMatchingExactly`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/strings#validation)          |                           | valid XML that matches the expected XML exactly                      |

Expectations for `XElement` and `XDocument`:

| Expectation                                                                                                         | Negated                | Summary                                                                                       |
|---------------------------------------------------------------------------------------------------------------------|------------------------|-----------------------------------------------------------------------------------------------|
| [`Matches`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/matching#elements-and-documents)             |                        | matches the expected XML, additional nodes allowed                                            |
| [`MatchesExactly`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/matching#elements-and-documents)      |                        | matches the expected XML exactly                                                              |
| [`HasAttribute`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/navigation#attributes)                  | `DoesNotHaveAttribute` | has the attribute, optionally with expectations on its value                                  |
| [`HasAttribute(…).EqualTo(…)`, …](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/navigation#attributes) |                        | has the attribute, whose value is compared with a string operator, e.g. `StartingWith`        |
| [`HasElement`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/navigation#child-elements)                | `DoesNotHaveElement`   | has the child element, optionally with expectations on it                                     |
| [`HasValue`](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/navigation#values)                          |                        | the value satisfies the expectations                                                          |
| [`HasValue().EqualTo(…)`, …](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/navigation#values)          |                        | the value is compared with a string operator, e.g. `EndingWith` or `Containing(…).AtLeast(2)` |

The comparisons can be relaxed with the options `IgnoringAdditionalNodes()`, `IgnoringElementOrder()` and
`IgnoringMember(path)` or `IgnoringMembers(path => …)`, see
[Matching XML](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/matching#options).

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/):

- [XML strings](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/strings): comparing and validating strings
  as XML
- [Matching XML](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/matching): `Matches` and
  `MatchesExactly`, and the options to relax the comparison
- [Attributes and child elements](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/navigation): verifying
  single attributes, child elements and values
