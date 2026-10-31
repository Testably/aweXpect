# aweXpect

[![Nuget](https://img.shields.io/nuget/v/aweXpect?label=aweXpect)](https://www.nuget.org/packages/aweXpect)
[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=Core)](https://www.nuget.org/packages/aweXpect.Core)
[![Build](https://github.com/Testably/aweXpect/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect)

Assert unit tests in natural language using awesome expectations.

```csharp
await Expect.That(message).IsEqualTo("It's a compliment");
```

```text
Expected that message
is equal to "It's a compliment",
but it was "It's a complement", which differs at index 12:
           ↓ (actual)
  "…a complement"
  "…a compliment"
           ↑ (expected)
```

## Getting started

1. Install the [`aweXpect`](https://www.nuget.org/packages/aweXpect) nuget package
   ```ps
   dotnet add package aweXpect --prerelease
   ```

2. Add the following `using` statement:
   ```csharp
   using aweXpect;
   ```
   This brings the static `Expect` class and lots of extension methods into scope.


3. See the [documentation](https://docs.testably.org/aweXpect/getting-started#write-your-first-expectation) or
   the [migration guide](https://docs.testably.org/aweXpect/migration) for more information.

## Features

### Async everything

By using async assertions per default, we have a consistent API and other perks:

- Complete async support, e.g. `IAsyncEnumerable`, `HttpResponseMessage` (via
  [aweXpect.Web](https://docs.testably.org/aweXpect/extensions/aweXpect.Web)) or similar async types
- No need to distinguish between `action.Throws()` and `await asyncAction.ThrowsAsync()`
- The evaluation is only triggered after the complete fluent chain is loaded, which has some nice benefits:
	- `Because` is applied once at the end of the expectation instead of being a parameter of every method
	- `WithCancellation` can also be registered at the end and applies a `CancellationToken` to all async methods which
	  allows cancellation of `IAsyncEnumerable` evaluations
	- Expectations can be combined directly (via `Expect.ThatAll`) instead of relying on global state
	  (e.g. [assertion scopes](https://fluentassertions.com/introduction#assertion-scopes))

### Performant

A focus on performance allows you to execute your tests as fast as possible.  
Special care is taken for the happy case (succeeding tests) to be as performant as possible. See
the [benchmarks](https://docs.testably.org/aweXpect/benchmarks) for more details.

### Any runner

xUnit, NUnit, MSTest and TUnit are detected automatically: a failed expectation usually throws the assertion exception
of the test framework. `Fail` and `Skip` fail or skip a test without an expectation. The supported versions and more
information can be found in the [test frameworks guide](https://docs.testably.org/aweXpect/setup/test-frameworks).

### Native AOT and trimming

`aweXpect` and `aweXpect.Core` are trimmable and AOT compatible on .NET 8 and later, see the
[Native AOT guide](https://docs.testably.org/aweXpect/setup/native-aot).

### Extensible

We added lots of extensibility points to allow you to build custom extensions.  
The [aweXpect.Core](https://www.nuget.org/packages/aweXpect.Core/) package is intended to be a stable source for
extensions, so that the risk of version conflicts between different extensions can be reduced.

You can extend the functionality for any types, by adding extension methods on `IThat<TType>`.
More information can be found in the [extensibility guide](https://docs.testably.org/aweXpect/extending).

## Extension packages

Expectations for types outside the base class library are provided by separate packages. See the
[extension packages](https://docs.testably.org/aweXpect/extensions) in the documentation for an overview.

**Core** extensions live in the aweXpect repository and are released together with `aweXpect`, always with the
same version. **Separate** extensions have their own repository and release cycle.

| Package                                                                                  | Version                                                                                                            | Release  | Expectations                                                                                                                                                                                                       |
|------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------|----------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [aweXpect.Json](https://docs.testably.org/aweXpect/extensions/aweXpect.Json)             | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)             | core     | JSON strings, `JsonElement`, `JsonNode` and the JSON serialization of objects: `IsEqualTo(…).AsJson()`, `IsValidJson`, `Matches`, `HasProperty`, `IsObject`, `IsArray`, `IsJsonSerializable`, `IsJsonSerializedAs` |
| [aweXpect.Xml](https://docs.testably.org/aweXpect/extensions/aweXpect.Xml)               | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Xml)](https://www.nuget.org/packages/aweXpect.Xml)               | core     | XML strings, `XElement` and `XDocument`: `IsEqualTo(…).AsXml()`, `IsValidXml`, `Matches`, `HasAttribute`, `HasElement`                                                                                             |
| [aweXpect.Web](https://docs.testably.org/aweXpect/extensions/aweXpect.Web)               | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)               | core     | `HttpRequestMessage`, `HttpResponseMessage`, `Uri` and `IPAddress`: `HasMethod`, `HasRequestUri`, `HasStatusCode`, `HasHeader`, `HasContent`, `HasHost`, `HasQueryParameter`                                       |
| [aweXpect.Reflection](https://docs.testably.org/aweXpect/extensions/aweXpect.Reflection) | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection) | core     | assemblies, types and members, for architecture and convention tests                                                                                                                                               |
| [aweXpect.Snapshots](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots)   | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots)](https://www.nuget.org/packages/aweXpect.Snapshots)   | core     | strings, objects and public APIs compared with a reviewed snapshot stored next to the test: `MatchesSnapshot`, `MatchesPublicApiSnapshot`                                                                          |
| [aweXpect.Logging](https://docs.testably.org/aweXpect/extensions/aweXpect.Logging)       | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Logging)](https://www.nuget.org/packages/aweXpect.Logging)       | separate | the records of a `FakeLogger` or `FakeLogCollector`: `Logged`, `LoggedWarning`, `DidNotLog`, `WithMessage`, `Within`, `Then`                                                                                       |
| [aweXpect.Testably](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably)     | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)     | separate | the file and time systems of [Testably.Abstractions](https://github.com/Testably/Testably.Abstractions): `HasFile`, `HasDirectory`, timers, watcher events                                                         |
| [aweXpect.Mockolate](https://docs.testably.org/aweXpect/extensions/aweXpect.Mockolate)   | [![Nuget](https://img.shields.io/nuget/v/aweXpect.Mockolate)](https://www.nuget.org/packages/aweXpect.Mockolate)   | separate | interactions with [Mockolate](https://github.com/Testably/Mockolate) mocks: `Once`, `Never`, `AtLeast`, `Then`, `AllInteractionsAreVerified`                                                                       |
