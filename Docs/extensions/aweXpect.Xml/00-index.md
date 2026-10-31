import PackageInstall from '@site/src/components/PackageInstall';

# aweXpect.Xml

[aweXpect.Xml](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Xml) contains expectations for XML strings and `System.Xml.Linq`.

## Installation

<PackageInstall package="aweXpect.Xml" prerelease />

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well, e.g. for the `IsEqualTo` of `IsEqualTo(…).AsXml()`.
The expectations live in the namespace `aweXpect`, so `using aweXpect;` is all you need.

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
await Expect.That(track).HasAttribute("artist").EqualTo("The Beatles").And
    .HasElement("genre", genre => genre.HasValue().EqualTo("rock"));
```

If the element was not closed, i.e. `<track title="Let It Be" artist="The Beatles">`, the expectation would fail with:

```text title="Failure message"
Expected that xml
is valid XML,
but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: track. Line 1, position 47.
```

## Overview

Expectations for [`string`](./01-strings.md):

| Expectation                                                        | Negated                   | Summary                                                              |
|--------------------------------------------------------------------|---------------------------|----------------------------------------------------------------------|
| [`IsEqualTo(…).AsXml()`](./01-strings.md#string-comparison-as-xml) | `IsNotEqualTo(…).AsXml()` | equal to the expected XML, compared by content                       |
| [`IsValidXml`](./01-strings.md#validation)                         | `IsNotValidXml`           | a well-formed XML document, optionally with expectations on its root |
| [`IsValidXmlMatching`](./01-strings.md#validation)                 |                           | valid XML that matches the expected XML                              |
| [`IsValidXmlMatchingExactly`](./01-strings.md#validation)          |                           | valid XML that matches the expected XML exactly                      |

Expectations for `XElement` and `XDocument`:

| Expectation                                                      | Negated                | Summary                                                                                       |
|------------------------------------------------------------------|------------------------|-----------------------------------------------------------------------------------------------|
| [`Matches`](./02-matching.md#elements-and-documents)             |                        | matches the expected XML, additional nodes allowed                                            |
| [`MatchesExactly`](./02-matching.md#elements-and-documents)      |                        | matches the expected XML exactly                                                              |
| [`HasAttribute`](./03-navigation.md#attributes)                  | `DoesNotHaveAttribute` | has the attribute, optionally with expectations on its value                                  |
| [`HasAttribute(…).EqualTo(…)`, …](./03-navigation.md#attributes) |                        | has the attribute, whose value is compared with a string operator, e.g. `StartingWith`        |
| [`HasElement`](./03-navigation.md#child-elements)                | `DoesNotHaveElement`   | has the child element, optionally with expectations on it                                     |
| [`HasValue`](./03-navigation.md#values)                          |                        | the value satisfies the expectations                                                          |
| [`HasValue().EqualTo(…)`, …](./03-navigation.md#values)          |                        | the value is compared with a string operator, e.g. `EndingWith` or `Containing(…).AtLeast(2)` |

The comparisons can be relaxed with the options `IgnoringAdditionalNodes()`, `IgnoringElementOrder()` and
`IgnoringMember(path)` or `IgnoringMembers(path => …)`, see [Matching XML](./02-matching.md#options).

## Documentation

- [XML strings](./01-strings.md): comparing and validating strings as XML
- [Matching XML](./02-matching.md): `Matches` and `MatchesExactly`, and the options to relax the comparison
- [Attributes and child elements](./03-navigation.md): verifying single attributes, child elements and values
