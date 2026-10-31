---
description: Make an extension work under trimming and Native AOT by marking generic parameters and registering the metadata of its own types.
---

# Native AOT for extensions

Under trimming or Native AOT, aweXpect compares and records only the types that have a registration (see
[Native AOT and trimming](../03-setup/03-native-aot.md)). An extension takes part in this in two ways:

- It marks a generic parameter whose argument reaches an equivalency comparison or an event recording with
  `[RequiresMemberMetadata]` or `[RequiresEventMetadata]`, see [generic parameters](#generic-parameters).
- It registers the types that it declares itself, see [registering metadata yourself](#registering-metadata-yourself).

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Recording;
```

</details>

## Generic parameters

An extension method whose argument reaches an equivalency comparison or an event recording only reveals an open type
parameter at its own call site, so it has to carry the marker that lets the source generator register the type at the
consumer's call site: `[RequiresMemberMetadata]` for a value that is compared, `[RequiresEventMetadata]` for a subject
that is recorded:

```csharp
public static IEventRecording<T> Watch<T>([RequiresEventMetadata] this T subject)
    where T : notnull
    => subject.Record().Events();
```

The marker for a compared value goes on the parameter that reaches the comparison:

```csharp no-compile
public static AndOrResult<T, IThat<T>> IsEquivalentToTrack<T>(this IThat<T> subject,
    [RequiresMemberMetadata] T expected)
    => // ...
```

## Registering metadata yourself

For the types that your extension declares itself, you don't depend on the source generator: register their public
members in `TypeMetadataRegistry` from a [module initializer](./11-initialization.md#module-initializers).
`RegisterBatch` publishes the registrations together, so that no comparison sees a type with only some of them:

```csharp
internal static class MusicStoreMetadata
{
#pragma warning disable CA2255 // The initializer of a class library is intended here
    [ModuleInitializer]
    internal static void Register()
        => TypeMetadataRegistry.RegisterBatch(() =>
        {
            TypeMetadataRegistry.RegisterProperty<Track, string>("Title", track => track.Title);
            TypeMetadataRegistry.RegisterProperty<Track, TimeSpan>("Duration", track => track.Duration);
        });
#pragma warning restore CA2255
}
```

A comparison uses the registered members of a type instead of reflecting over it, so register every public member
that it should compare. The other methods cover the other kinds of metadata:

- `RegisterField` registers a public field, like `RegisterProperty` does for a property.
- `RegisterExplicitProperty` registers a property that the type implements explicitly for an interface, under its
  name qualified by the interface, e.g. `MusicStore.IHasTitle.Title`.
- `RegisterEvent` registers an event, so that it can be recorded. It receives a factory for a handler of the event
  type and the delegates that add and remove it.
- `RegisterCollection` registers a collection type, so that a comparison with another collection also compares the
  members the type declares itself, besides its items.
- `RegisterRecord` registers a record whose `ToString()` the compiler generated, so that a failure message writes its
  registered members instead of that culture-dependent rendering. Leave it out for a record with a `ToString()` or
  `PrintMembers` of its own.
- `RegisterDictionary` and `RegisterSet` register the dictionaries and sets of the given type arguments, so that a
  comparison reads their key comparer or comparer.

## The source generator

aweXpect reflects over a type only when it has no registration in `TypeMetadataRegistry`, and switches that fallback
off when an application is published with trimming or Native AOT enabled, so that a comparison or recording never
silently verifies less than it claims to.

The source generator that follows the markers ships in the `aweXpect.Core` package. It runs when the consumer's
project is compiled, and only if that project gets the analyzers of `aweXpect.Core`, is compiled with C# 9 or later
and has the `ModuleInitializerAttribute` (.NET 5 or later, or an own `internal` declaration). A project gets them by
referencing `aweXpect.Core`, `aweXpect` or one of the extension packages of aweXpect, which all pass them on. Your
extension passes them on when it references `aweXpect.Core` as shown in
[referencing aweXpect.Core](./00-index.md#referencing-awexpectcore).

<details>
<summary>Several packages that pass the analyzers on</summary>

NuGet restores `aweXpect.Core` only once, however many packages pass its analyzers on, so the generator runs once per
project.

</details>

## Reflection in an extension

An extension that reflects over a subject itself should guard the reflection with `ReflectionFallback.IsSupported`
from the namespace `aweXpect.Core.Extending`, and otherwise throw the exception of `ReflectionFallback.NotSupported`,
so that it behaves the same way as the built-in expectations. Its message names what cannot be found, an optional
remedy and the `aweXpect.ReflectionFallback.IsSupported` runtime switch:

```csharp no-compile
if (!ReflectionFallback.IsSupported)
{
	throw ReflectionFallback.NotSupported(
		$"The tracks of {Formatter.Format(album.GetType())}",
		"Register the album type.");
}
```

The constant `ReflectionFallback.SwitchName` holds the name of the switch.
