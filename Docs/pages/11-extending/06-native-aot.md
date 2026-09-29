# Native AOT for extensions

aweXpect reflects over a type only when it has no registration in `TypeMetadataRegistry`, and switches that fallback
off when an application is published with trimming or Native AOT enabled, so that a comparison or recording never
silently verifies less than it claims to. An extension takes part in this in two ways.

An extension method whose argument reaches an equivalency comparison or an event recording only reveals an open type
parameter at its own call site, so it has to carry the marker that lets the source generator register the type at the
consumer's call site: `[RequiresMemberMetadata]` for a value that is compared, `[RequiresEventMetadata]` for a subject
that is recorded:

```csharp
public static IEventRecording<T> Watch<T>([RequiresEventMetadata] this T subject)
    where T : notnull
    => subject.Record().Events();
```

An extension that reflects over a subject itself should guard the reflection with `ReflectionFallback.IsSupported`
and fail with a message that names the `aweXpect.ReflectionFallback.IsSupported` runtime switch otherwise, so that it
behaves the same way as the built-in expectations.
