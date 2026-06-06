# Configuration

All customizations live under `Customize.aweXpect.Reflection()`. Every `Set(…)` returns a scope that
restores the previous value when disposed, so a customization can be applied globally or per test.

## Assembly exclusions

By default, assemblies whose name matches one of the following prefixes are excluded from
`In.AllLoadedAssemblies()`:

`mscorlib`, `System`, `Microsoft`, `netstandard`, `WindowsBase`, `JetBrains`, `xunit`, `Castle`,
`DynamicProxyGenAssembly2`.

Both the assembly scanning and the dependency assertions (`DependsOnlyOn` / `DependOnlyOn` /
`WhichDependOnlyOn`, on both assemblies and types) use the same prefixes with the same matching: a prefix
matches at a name-segment boundary, so `System` covers `System` and `System.Text.Json`, but not an assembly
named `SystemsBiology`. A prefix written with a trailing dot (e.g. `MyCompany.`) is boundary-safe by
construction and covers everything starting with it. Empty prefixes are ignored.

Customize this via `Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes`. `Set(…)` replaces the
list and returns a scope that restores the previous value when disposed:

```csharp
using (Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes
    .Set(new[] { "mscorlib", "System", "Microsoft", "MyCompany.Generated" }))
{
    // In.AllLoadedAssemblies() applies the custom prefixes within this scope
}
```

## Compiler-generated members

By default, compiler-generated types and members are **excluded** from every navigation
(`.Types()`, `.Methods()`, `.Fields()`, …). This hides closures, async/iterator state machines,
anonymous types, local functions, auto-property backing fields and the generated members of records
(`ToString`, `Equals`, `<Clone>$`, the copy-constructor, …), so convention tests see only the members
you actually wrote.

Opt specific kinds back in with the `[Flags]` enum `CompilerGeneratedMembers`
(`None`, `Types`, `Constructors`, `Methods`, `Properties`, `Fields`, `Events`, `All`):

```csharp
using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
    .Set(CompilerGeneratedMembers.Types | CompilerGeneratedMembers.Methods))
{
    // closures, state machines and compiler-generated methods are now visible
}
```

Operators (`op_*`) and property/event accessors (`get_`, `set_`, `add_`, `remove_`) are *user-written*
but likewise excluded by default. Include them via the separate `SpecialNameMembers` enum
(`None`, `Operators`, `Accessors`, `All`), which only affects `.Methods()`:

```csharp
using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers()
    .Set(SpecialNameMembers.Operators))
{
    // operator methods are now visible in .Methods()
}
```

## Dependency resolver

The type-level dependency assertions compute a type's dependencies with a built-in signature-level
resolver (base type, interfaces, field/property/event types, method/constructor signatures, generic
arguments and applied attributes). Method-body references are not detected by the default; supply a
custom resolver, e.g. backed by [Mono.Cecil](https://github.com/jbevain/cecil) (this library takes no
dependency on it; reference the package yourself), for IL/body-level accuracy:

```csharp
// Replace the resolver within a scope
using (Customize.aweXpect.Reflection().DependencyResolver()
    .Set(type => MyCecilResolver.GetUsedTypes(type)))
{
    // body-level references now count as dependencies
}

// Or augment instead of replace: compose on the current default
var resolver = Customize.aweXpect.Reflection().DependencyResolver();
var builtin = resolver.Get()!;
using (resolver.Set(type => builtin(type).Concat(MyCecilResolver.GetBodyTypes(type))))
{
    // built-in signature dependencies plus the body-level extras
}

// Setting null reverts to the built-in default, e.g. to opt a single test out
// of a globally configured resolver
using (Customize.aweXpect.Reflection().DependencyResolver().Set(null))
{
    // the signature-level default applies within this scope
}
```

`Get()` always returns the resolver currently in effect (the built-in default when none is configured),
so composing on it works regardless of what an outer scope has set up.

A Mono.Cecil-backed resolver boils down to reading the type's assembly and mapping the IL references
back to runtime types:

```csharp
public static IEnumerable<Type> GetUsedTypes(Type type)
{
    using var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(type.Assembly.Location);
    var definition = assembly.MainModule.GetType(type.FullName!.Replace('+', '/'));
    foreach (var instruction in definition.Methods
                 .Where(method => method.HasBody)
                 .SelectMany(method => method.Body.Instructions))
    {
        // map the method/field/type references in instruction.Operand back to System.Type
        // (and also walk the signature surface: base type, interfaces, fields, …)
    }
}
```

Every resolver's output is normalized like the built-in's (array/by-ref/pointer element types and
generic arguments are unwrapped, the result is de-duplicated) and cached per type for the lifetime of
the resolver delegate, so a custom resolver needs no caching of its own. It must, however, be **pure**:
deterministic for a given `Type` within its scope; that is what makes the caching safe.
