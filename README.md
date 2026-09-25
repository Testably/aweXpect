# aweXpect

[![Nuget](https://img.shields.io/nuget/v/aweXpect?label=aweXpect)](https://www.nuget.org/packages/aweXpect)
[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=Core)](https://www.nuget.org/packages/aweXpect.Core)
[![Build](https://github.com/Testably/aweXpect/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect/main)

Assert unit tests in natural language using awesome expectations.

## Getting started

1. Install the [`aweXpect`](https://www.nuget.org/packages/aweXpect) nuget package
   ```ps
   dotnet add package aweXpect
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

- Complete async support, e.g. `IAsyncEnumerable` `HttpResponseMessage` or similar async types
- No need to distinguish between `action.Throws()` and `await asyncAction.ThrowsAsync()`
- The evaluation is only triggered after the complete fluent chain is loaded, which has some nice benefits:
	- `Because` can be registered once as a general method that can be applied at the end of the expectation instead of
	  cluttering all methods with the `because` and `becauseArgs` parameters
	- `WithCancellation` can also be registered at the end an applies a `CancellationToken` to all async methods which
	  allows cancellation of `IAsyncEnumerable` evaluations
	- Expectations can be combined directly (via `Expect.ThatAll`) instead of relying on global state (
	  e.g. [assertion scopes](https://fluentassertions.com/introduction#assertion-scopes))

### Performant

A focus on performance allows you to execute your tests as fast as possible.  
Special care is taken for the happy case (succeeding tests) to be as performant as possible. See
the [benchmarks](https://docs.testably.org/aweXpect/benchmarks) for more details.

### Extensible

We added lots of extensibility points to allow you to build custom extensions.  
The [aweXpect.Core](https://www.nuget.org/packages/aweXpect.Core/) package is intended to be a stable source for
extensions, so that the risk of version conflicts between different extensions can be reduced.

You can extend the functionality for any types, by adding extension methods on `IThat<TType>`.
More information can be found in the [extensibility guide](https://docs.testably.org/aweXpect/write-extension).

**Extension projects**

- [aweXpect.Json](https://github.com/aweXpect/aweXpect.Json)  
  [![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)  
  Expectations for the System.Text.Json namespace.

- [aweXpect.Web](https://github.com/aweXpect/aweXpect.Web)  
  [![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web)  
  Expectations for HttpClient.

- [aweXpect.Reflection](https://github.com/aweXpect/aweXpect.Reflection)  
  [![Nuget](https://img.shields.io/nuget/v/aweXpect.Reflection)](https://www.nuget.org/packages/aweXpect.Reflection)  
  Expectations for reflection types.

- [aweXpect.Testably](https://github.com/aweXpect/aweXpect.Testably)  
  [![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)  
  Expectations for the file and time system
  from [Testably.Abstractions](https://github.com/Testably/Testably.Abstractions).

- [aweXpect.Mockolate](https://github.com/aweXpect/aweXpect.Mockolate)  
  [![Nuget](https://img.shields.io/nuget/v/aweXpect.Mockolate)](https://www.nuget.org/packages/aweXpect.Mockolate)  
  Expectations to verify interactions with mocks from [Mockolate](https://github.com/aweXpect/Mockolate).
