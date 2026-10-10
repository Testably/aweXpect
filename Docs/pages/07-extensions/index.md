# Extension packages

Expectations for types outside the base class library are provided by separate packages. Each package adds extension
methods on `IThat<T>` for its types, so once it is referenced, its expectations appear on `Expect.That(…)` next to the
built-in ones.

| Package                                                         | Expectations                                                                                                                    |
|-----------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------|
| [aweXpect.Json](/aweXpect/extensions/aweXpect.Json)             | JSON strings and `JsonElement`: `IsEqualTo(…).AsJson()`, `IsValidJson`, `Matches`, `IsObject`, `IsArray`, `IsJsonSerializable` |
| [aweXpect.Web](/aweXpect/extensions/aweXpect.Web)               | `HttpRequestMessage` and `HttpResponseMessage`: `HasMethod`, `HasRequestUri`, `HasStatusCode`, `HasHeader`, `HasContent`        |
| [aweXpect.Reflection](/aweXpect/extensions/aweXpect.Reflection) | assemblies, types and members, for architecture and convention tests                                                            |
| [aweXpect.Testably](/aweXpect/extensions/aweXpect.Testably)     | the file and time systems of [Testably.Abstractions](/Abstractions): `HasFile`, `HasDirectory`, timers, watcher events          |
| [aweXpect.Mockolate](/aweXpect/extensions/aweXpect.Mockolate)   | interactions with [Mockolate](/Mockolate) mocks: `Once`, `Never`, `AtLeast`, `Then`, `AllInteractionsAreVerified`               |

## Write your own

If no package covers your types, you can add expectations yourself with the same API these packages use.
[Extending aweXpect](../11-extending/index.md) walks you through your first expectation and the conventions for its
failure messages.
