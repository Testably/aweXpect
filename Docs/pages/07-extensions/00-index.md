---
description: The extension packages that add expectations for JSON, XML, HTTP, reflection, snapshots, logging, file systems and mocks.
---

import PackageVersion from '@site/src/components/PackageVersion';

# Extension packages

Expectations for types outside the base class library are provided by separate packages. Each package adds extension
methods on `IThat<T>` for its types, so once it is referenced, its expectations appear on `That(…)` next to the
built-in ones.

## Core extensions

These packages live in the aweXpect repository and are released together with `aweXpect`, always with the same version.

| Package                                                         | Checks                                                                   |
|-----------------------------------------------------------------|--------------------------------------------------------------------------|
| [aweXpect.Json](/aweXpect/extensions/aweXpect.Json)             | JSON strings, `JsonElement`, `JsonNode` and the serialization of objects |
| [aweXpect.Reflection](/aweXpect/extensions/aweXpect.Reflection) | assemblies, types and members, for architecture and convention tests     |
| [aweXpect.Snapshots](/aweXpect/extensions/aweXpect.Snapshots)   | strings, objects and public APIs against a reviewed snapshot             |
| [aweXpect.Web](/aweXpect/extensions/aweXpect.Web)               | `HttpRequestMessage`, `HttpResponseMessage`, `Uri` and `IPAddress`       |
| [aweXpect.Xml](/aweXpect/extensions/aweXpect.Xml)               | XML strings, `XElement` and `XDocument`                                  |

## Separate extensions

These packages have their own repository and release cycle.

| Package                                                       | Version                                           | Checks                                                              |
|---------------------------------------------------------------|---------------------------------------------------|---------------------------------------------------------------------|
| [aweXpect.Logging](/aweXpect/extensions/aweXpect.Logging)     | <PackageVersion package="aweXpect.Logging" />     | the records of a `FakeLogger` or `FakeLogCollector`                 |
| [aweXpect.Mockolate](/aweXpect/extensions/aweXpect.Mockolate) | <PackageVersion package="aweXpect.Mockolate" />   | interactions with [Mockolate](/Mockolate) mocks                     |
| [aweXpect.Testably](/aweXpect/extensions/aweXpect.Testably)   | <PackageVersion package="aweXpect.Testably" />    | the file and time systems of [Testably.Abstractions](/Abstractions) |

:::tip[aweXpect.Chronology]
[aweXpect.Chronology](/aweXpect/chronology) adds readable `TimeSpan` and `DateTime` values like `5.Seconds()` or `24.December(2024)`. It adds no expectations and does not depend on aweXpect; many samples in this documentation use it.
:::

## Write your own

If no package covers your types, you can add expectations yourself with the same API these packages use.
[Extending aweXpect](../11-extending/00-index.md) walks you through your first expectation and the conventions for its
failure messages.
