# Native AOT for extensions

aweXpect reflects over a type only when it has no registration in `TypeMetadataRegistry`, and switches that fallback
off when an application is published with trimming or Native AOT enabled, so that a comparison or recording never
silently verifies less than it claims to (see [Native AOT and trimming](../03-how-it-works/08-native-aot.md)). An
extension takes part in this in two ways.

An extension method whose argument reaches an equivalency comparison or an event recording only reveals an open type
parameter at its own call site, so it has to carry the marker that lets the source generator register the type at the
consumer's call site: `[RequiresMemberMetadata]` for a value that is compared, `[RequiresEventMetadata]` for a subject
that is recorded:

```csharp
using aweXpect.Core.Metadata;
using aweXpect.Recording;

public static IEventRecording<T> Watch<T>([RequiresEventMetadata] this T subject)
    where T : notnull
    => subject.Record().Events();
```

The source generator that follows these markers ships in the `aweXpect` package, not in `aweXpect.Core`. It runs when
the consumer's project is compiled, and only if that project gets the `aweXpect` package, is compiled with C# 9 or
later and has the `ModuleInitializerAttribute` (.NET 5 or later, or an own `internal` declaration). A consumer that
only references `aweXpect.Core` and your extension gets no registrations, so the types stay on the reflection
fallback.

An extension that reflects over a subject itself should guard the reflection with `ReflectionFallback.IsSupported`
and fail with a message that names the `aweXpect.ReflectionFallback.IsSupported` runtime switch otherwise, so that it
behaves the same way as the built-in expectations.
