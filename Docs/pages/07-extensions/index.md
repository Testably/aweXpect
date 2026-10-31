# Extension packages

Expectations for types outside the base class library are provided by separate packages. Each package adds extension
methods on `IThat<T>` for its types, so once it is referenced, its expectations appear on `Expect.That(…)` next to the
built-in ones.

**Core** extensions live in the aweXpect repository and are released together with `aweXpect`, always with the
same version. **Separate** extensions have their own repository and release cycle.

| Package                                                         | Release  | Expectations                                                                                                                                                                                                       |
|-----------------------------------------------------------------|----------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [aweXpect.Json](/aweXpect/extensions/aweXpect.Json)             | core     | JSON strings, `JsonElement`, `JsonNode` and the JSON serialization of objects: `IsEqualTo(…).AsJson()`, `IsValidJson`, `Matches`, `HasProperty`, `IsObject`, `IsArray`, `IsJsonSerializable`, `IsJsonSerializedAs` |
| [aweXpect.Xml](/aweXpect/extensions/aweXpect.Xml)               | core     | XML strings, `XElement` and `XDocument`: `IsEqualTo(…).AsXml()`, `IsValidXml`, `Matches`, `HasAttribute`, `HasElement`                                                                                             |
| [aweXpect.Web](/aweXpect/extensions/aweXpect.Web)               | core     | `HttpRequestMessage`, `HttpResponseMessage`, `Uri` and `IPAddress`: `HasMethod`, `HasRequestUri`, `HasStatusCode`, `HasHeader`, `HasContent`, `HasHost`, `HasQueryParameter`                                       |
| [aweXpect.Reflection](/aweXpect/extensions/aweXpect.Reflection) | core     | assemblies, types and members, for architecture and convention tests                                                                                                                                               |
| [aweXpect.Snapshots](/aweXpect/extensions/aweXpect.Snapshots)   | core     | strings, objects and public APIs compared with a reviewed snapshot stored next to the test: `MatchesSnapshot`, `MatchesPublicApiSnapshot`                                                                          |
| [aweXpect.Logging](/aweXpect/extensions/aweXpect.Logging)       | separate | the records of a `FakeLogger` or `FakeLogCollector`: `Logged`, `LoggedWarning`, `DidNotLog`, `WithMessage`, `Within`, `Then`                                                                                       |
| [aweXpect.Testably](/aweXpect/extensions/aweXpect.Testably)     | separate | the file and time systems of [Testably.Abstractions](/Abstractions): `HasFile`, `HasDirectory`, timers, watcher events                                                                                             |
| [aweXpect.Mockolate](/aweXpect/extensions/aweXpect.Mockolate)   | separate | interactions with [Mockolate](/Mockolate) mocks: `Once`, `Never`, `AtLeast`, `Then`, `AllInteractionsAreVerified`                                                                                                  |

[aweXpect.Chronology](/aweXpect/extensions/aweXpect.Chronology) adds no expectations, but readable `TimeSpan` and
`DateTime` values like `5.Seconds()` or `24.December(2024)`. It does not depend on aweXpect, and many samples in
this documentation use it.

## Write your own

If no package covers your types, you can add expectations yourself with the same API these packages use.
[Extending aweXpect](../11-extending/index.md) walks you through your first expectation and the conventions for its
failure messages.
