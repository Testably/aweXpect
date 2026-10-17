# aweXpect.Xml

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Xml)](https://www.nuget.org/packages/aweXpect.Xml)
[![Build](https://github.com/Testably/aweXpect.Xml/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Xml/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Xml&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Xml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Xml&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Xml)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Xml%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Xml/main)

Expectations for XML strings and `System.Xml.Linq` for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

```csharp
string xml = """<track title="Let It Be" artist="The Beatles" />""";

// verify that a string is a well-formed XML document
await Expect.That(xml).IsValidXml();

// verify that a string is equal to the expected XML, compared by its content instead of its formatting
await Expect.That(xml).IsEqualTo("""<track artist="The Beatles" title="Let It Be"></track>""").AsXml();
```

If the element was not closed, i.e. `<track title="Let It Be" artist="The Beatles">`, the expectation would fail with:

```text title="Failure message"
Expected that xml
is valid XML,
but it could not be parsed: Unexpected end of file has occurred. The following elements are not closed: track. Line 1, position 47.
```

## Overview

| Expectation                                                                       | Subject                 | Summary                                                       |
|-----------------------------------------------------------------------------------|-------------------------|---------------------------------------------------------------|
| [`IsEqualTo(…).AsXml()`](../../Docs/extensions/aweXpect.Xml/01-strings.md#string-comparison-as-xml)        | `string`                | is equivalent to the expected XML, by its content             |
| [`IsValidXml`](../../Docs/extensions/aweXpect.Xml/01-strings.md#validation)                               | `string`                | can be parsed as a well-formed XML document                   |
| [`IsValidXmlMatching`](../../Docs/extensions/aweXpect.Xml/01-strings.md#validation)                       | `string`                | is valid XML that matches the expected XML                    |
| [`IsValidXmlMatchingExactly`](../../Docs/extensions/aweXpect.Xml/01-strings.md#validation)                | `string`                | is valid XML that matches the expected XML exactly            |
| [`Matches`](../../Docs/extensions/aweXpect.Xml/02-matching.md#elements-and-documents)                     | `XElement`, `XDocument` | contains the expected XML, ignoring additional nodes          |
| [`MatchesExactly`](../../Docs/extensions/aweXpect.Xml/02-matching.md#elements-and-documents)              | `XElement`, `XDocument` | is equivalent to the expected XML                             |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/):

- [XML strings](../../Docs/extensions/aweXpect.Xml/01-strings.md): comparing and validating strings as XML
- [Matching XML](../../Docs/extensions/aweXpect.Xml/02-matching.md): matching `XElement` and `XDocument` against an expected XML
