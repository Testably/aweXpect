# aweXpect.Xml

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Xml)](https://www.nuget.org/packages/aweXpect.Xml)
[![Build](https://github.com/Testably/aweXpect.Xml/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Xml/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Xml&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Xml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Xml&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Xml)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Xml%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Xml/main)

Expectations for XML strings and `System.Xml.Linq` for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

```csharp
// compare XML strings by their structure instead of their formatting
await Expect.That(xml).IsEqualTo(expected).AsXml();

// verify that a string is valid XML
await Expect.That(xml).IsValidXml();
```

## Overview

| Expectation             | Subject  | Summary                                         |
|-------------------------|----------|-------------------------------------------------|
| `IsEqualTo(…).AsXml()`  | `string` | equal to the expected XML, compared by structure |
| `IsValidXml`            | `string` | can be parsed as XML                            |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml/).
