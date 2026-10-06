# Native AOT and trimming

Publishing with trimming or Native AOT removes code and metadata that is only reached through reflection. aweXpect
needs reflection for equivalency and for recording events, so a source generator that ships with the `aweXpect`
package generates the registrations they need when your project is compiled.

## Equivalency

Equivalency has to know the members of the compared types. Reflection provides them under the JIT, but publishing
with trimming or Native AOT enabled removes members that are only reached reflectively, so a comparison would
silently verify less than it claims to.

A source generator that ships with the `aweXpect` package closes this gap: for every call site that passes a value to
`IsEquivalentTo`, `IsNotEquivalentTo`, `AreEquivalentTo` or switches to `Equivalent()`, it registers the public
fields and properties of the argument's type, of the subject's type and of every type reachable through their
members. The registration runs when your assembly is loaded and needs no configuration. A type that has a
registration is compared through it, every other type is reflected over under the JIT and fails with an error
naming the fix where reflection is switched off, as described below. A type without any comparable member fails
loudly instead of passing without verifying anything. The registration also feeds the failure message: a registered
object is rendered from its registered members, so the message keeps listing them after trimming, while an
unregistered object is rendered as `{ *unregistered* }` where reflection is switched off. Either way the message lists
public instance members only, without static members, indexers or properties that lack a public getter.

For a collection type, the generator registers the members that the type declares itself, which are
[compared in addition to its items](../04-values/13-equivalency.md#collections-and-dictionaries). Where reflection is
switched off, a collection type without such a registration is compared by its items alone, because an iterator or
another collection that only exists as a runtime type cannot be registered. Name a collection type the generator did
not see in `GenerateMetadata`, as described below, to compare its own members as well.

<details>
<summary>Types the generator cannot see, and how reflection is switched off</summary>

Some types cannot be seen by the generator, because it works from the types declared in your source:

- a member declared as `object`, an interface or a base type only reveals the declared type; the instance it holds at
  runtime is compared through reflection,
- a `private`, `protected` or `file`-local type cannot be referenced by generated code and is compared through
  reflection,
- a value that reaches the comparison through your own extension method is only registered if the extension's
  parameter or type parameter carries `[RequiresMemberMetadata]`.

A type the generator merely did not see, such as the runtime type behind an `object` member or a value passed through
an unmarked extension, can be named explicitly to register it anyway:

```csharp
using aweXpect.Core.Metadata;

[assembly: GenerateMetadata(typeof(Track))]
```

A type the generated code cannot reference stays on reflection regardless, and the generator warns with `aweXpect2001`
when a named type yields no registration.

The registration needs `ModuleInitializerAttribute` and C# 9, so nothing is generated for a project that targets
.NET Framework or .NET Standard 2.0 unless it polyfills the attribute. Those targets cannot be trimmed or published
with Native AOT and keep using reflection.

The walk follows every member type the comparison would visit, including framework types. A member of type
`Exception`, for example, registers the types reachable from its properties, because reflection would compare them
too. Members whose getter is marked with `RequiresUnreferencedCode` or `RequiresDynamicCode` cannot be registered, so
their type stays on the reflection path.

Reflection over a type without a registration is switched off in a project that enables trimming or Native AOT
(`PublishTrimmed` or `PublishAot`), because the trimmer removes members that only reflection reaches, and a comparison
would silently verify less than it claims to. This also applies when such a project runs under the JIT, e.g. in
`dotnet test`. Such a comparison fails with an error that names the type and asks you to register it. The same applies
to a comparison that requests `IncludeMembers.Internal`, because only public members are registered. The
`aweXpect.ReflectionFallback.IsSupported` runtime switch forces the fallback either way, and the
`AweXpectReflectionFallback` property of your project sets that switch:

```xml
<PropertyGroup>
  <AweXpectReflectionFallback>true</AweXpectReflectionFallback>
</PropertyGroup>
```

With the fallback forced on, a trimmed application reflects over whatever the trimmer left, which is best effort: a
type whose members were all removed still fails with an error that asks you to root it, but a type that lost only
some of them is compared through the rest.

</details>

## Events

A recording has to know the events of its subject and attach a handler to each of them. Reflection provides both
under the JIT, but publishing with trimming or Native AOT enabled removes events that are only reached reflectively,
and the handler for an event with value-type parameters cannot be bound without runtime code generation.

The source generator that ships with the `aweXpect` package closes this gap: for every call site of `Record()`, it
registers the public events of the subject's static type together with a handler factory, so the recording neither
looks the events up nor binds a handler reflectively. The registration runs when your assembly is loaded and needs no
configuration. A subject whose runtime type has a registration is recorded through it, every other subject is
reflected over as before, and a registered handler takes any number of parameters.

<details>
<summary>Types the generator cannot see, and how reflection is switched off</summary>

The generator works from the declared type, so the same limits apply as for
[equivalency](#equivalency):

- a subject declared as an interface, an abstract class or a base type only reveals the declared type; the recording
  looks up the runtime type of the instance, which stays on reflection,
- a `private`, `protected` or `file`-local type cannot be referenced by generated code and is reflected over,
- a subject that reaches `Record()` through your own extension method is only registered if the extension's parameter
  or type parameter carries `[RequiresEventMetadata]`,
- an event whose handler returns a value or takes a parameter by reference, a pointer or a ref struct cannot be recorded
  by the reflective path either, and keeps its type on reflection,
- a `struct` subject is never registered, because a handler added to a boxed copy never sees the caller's value.

A type the generator did not see can be named explicitly with `[assembly: GenerateMetadata(typeof(MyClass))]`, which
registers its members and its events.

Reflection over a subject without a registration is switched off in a project that enables trimming or Native AOT, also
when it runs under the JIT, because the trimmer removes events that only reflection reaches and the reflective recorder
needs runtime code generation. Recording such a subject fails with an error that names the type and asks you to register
it. The `aweXpect.ReflectionFallback.IsSupported` runtime switch forces the fallback either way, as described for
[equivalency](#equivalency); with the fallback forced on, the error for an unknown event name, and for an event that a
recording of all events did not find, asks you to root the type instead.

</details>

For what an extension author has to consider, see [Native AOT for extensions](../11-extending/06-native-aot.md).
