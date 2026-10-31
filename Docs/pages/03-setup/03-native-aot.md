---
description: How aweXpect supports equivalency and event recording when a project is published with trimming or Native AOT.
---

# Native AOT and trimming

Publishing with trimming or Native AOT removes code and metadata that is only reached through reflection. aweXpect
needs reflection for equivalency and for recording events, so a source generator that ships with the
`aweXpect.Core` package generates the registrations they need when your project is compiled. This needs no
configuration.

## Equivalency

Equivalency has to know the members of the compared types. Reflection provides them under the JIT, but publishing
with trimming or Native AOT enabled removes members that are only reached reflectively, so a comparison would
silently verify less than it claims to.

For every call site that passes a value to `IsEquivalentTo`, `IsNotEquivalentTo`, `AreEquivalentTo` or switches to
`Equivalent()`, the generator registers the public fields and properties of the argument's type, of the subject's type
and of every type reachable through their members. A type without any comparable member fails loudly instead of
passing without verifying anything.

<details>
<summary>Collection types, framework types and the failure message</summary>

A type that has a registration is compared through it, every other type is reflected over under the JIT and fails
with an error naming the fix where reflection is switched off.

For a collection type, the generator registers the members that the type declares itself, which are
[compared in addition to its items](../04-values/13-equivalency.md#collections-and-dictionaries). Where reflection is
switched off, a collection type without such a registration is compared by its items alone, because an iterator or
another collection that only exists as a runtime type cannot be registered. Name a collection type the generator did
not see in [`GenerateMetadata`](#when-registration-fails) to compare its own members as well.

The walk follows every member type the comparison would visit, including framework types. A member of type
`Exception`, for example, registers the types reachable from its properties, because reflection would compare them
too.

The registration also feeds the failure message: a registered object is rendered from its registered members, so the
message keeps listing them after trimming, while an unregistered object is rendered as `{ *unregistered* }` where
reflection is switched off. Either way the message lists public instance members only, without static members,
indexers or properties that lack a public getter. A record whose `ToString()` the compiler generated is rendered from
its members, too, because that `ToString()` depends on the current culture and writes a collection as its type name.
Where reflection is switched off, only a registered record is recognized; an unregistered one keeps its own
`ToString()`.

</details>

## Events

A recording has to know the events of its subject and attach a handler to each of them. Reflection provides both
under the JIT, but publishing with trimming or Native AOT enabled removes events that are only reached reflectively,
and the handler for an event with value-type parameters cannot be bound without runtime code generation.

For every call site of `Record()`, the generator registers the public events of the subject's static type together
with a handler factory, so the recording neither looks the events up nor binds a handler reflectively.

<details>
<summary>Subjects and events that stay on reflection</summary>

A subject whose runtime type has a registration is recorded through it, every other subject is reflected over under the
JIT, and a registered handler takes any number of parameters.

- An event whose handler returns a value or takes a parameter by reference, a pointer or a ref struct cannot be
  recorded by the reflective path either, and keeps its type on reflection.
- A `struct` subject is never registered, because a handler added to a boxed copy never sees the caller's value.

</details>

## When registration fails

In a project that enables trimming or Native AOT (`PublishTrimmed` or `PublishAot`), reflection over a type without a
registration is switched off, also when its tests run under the JIT, e.g. in `dotnet test`. A comparison or a
recording that reaches such a type fails with an error that names the type and asks you to register it:

```csharp
using aweXpect.Core.Metadata;

[assembly: GenerateMetadata(typeof(Track))]
```

`GenerateMetadata` registers the members and the events of the type. The generator only sees the types declared in
your source, so you need it for:

- the instance behind a member or subject declared as `object`, an interface, an abstract class or a base type, which
  only reveals the declared type,
- a value that reaches the comparison or `Record()` through your own extension method, unless the extension's
  parameter or type parameter carries `[RequiresMemberMetadata]` (equivalency) or `[RequiresEventMetadata]` (events).

A `private`, `protected` or `file`-local type cannot be referenced by generated code and stays on reflection
regardless; the generator warns with `aweXpect2001` when a named type yields no registration.

The `AweXpectReflectionFallback` property of your project changes when reflection is switched off:

```xml
<PropertyGroup>
  <AweXpectReflectionFallback>true</AweXpectReflectionFallback>
</PropertyGroup>
```

- `true` switches reflection on in a project that enables trimming or Native AOT. A trimmed application then reflects
  over whatever the trimmer left, which is best effort: a type whose members were all removed still fails with an
  error that asks you to root it, but a type that lost only some of them is compared through the rest. For a
  recording, the error for an unknown event name, and for an event that a recording of all events did not find, asks
  you to root the type instead.
- `false` switches reflection off in a project that is not published with trimming or Native AOT. A normal
  `dotnet test` run then fails with the same errors as a trimmed one, so you find the types to register without
  publishing.

<details>
<summary>The runtime switch, internal members and older target frameworks</summary>

The property sets the `aweXpect.ReflectionFallback.IsSupported` runtime switch, which forces the fallback either way.

A comparison that requests `IncludeMembers.Internal` is switched off in the same way as a type without a registration,
because only public members are registered. For a recording, reflection is switched off because the trimmer removes
events that only reflection reaches and the reflective recorder needs runtime code generation.

The registration needs `ModuleInitializerAttribute` and C# 9, so nothing is generated for a project that targets
.NET Framework or .NET Standard 2.0 unless it polyfills the attribute. Those targets cannot be trimmed or published
with Native AOT and keep using reflection.

</details>

For what an extension author has to consider, see [Native AOT for extensions](../11-extending/12-native-aot.md).
