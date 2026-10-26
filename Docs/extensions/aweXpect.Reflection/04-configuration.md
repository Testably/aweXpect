# Configuration

The customizations of this library live under `Customize.aweXpect.Reflection()`, which this library provides in the
namespace `aweXpect.Reflection`, together with the enums `CompilerGeneratedMembers` and `SpecialNameMembers`. The
assembly exclusions are shared with aweXpect.Core and live under `Customize.aweXpect.Settings()`. Every `Set(…)` returns
a scope that restores the previous value when disposed.

| Customization                                                              | Default                           | Summary                                                                                      |
|----------------------------------------------------------------------------|-----------------------------------|----------------------------------------------------------------------------------------------|
| [`Settings().ExcludedAssemblyPrefixes`](#assembly-exclusions)              | `System`, `Microsoft`, …          | assemblies excluded from `In.AllLoadedAssemblies()` and ignored by the `DependOnlyOn` family |
| [`IncludedCompilerGeneratedMembers`](#compiler-generated-members)          | `CompilerGeneratedMembers.None`   | the compiler-generated types and members included in the navigations                         |
| [`IncludedSpecialNameMembers`](#compiler-generated-members)                | `SpecialNameMembers.None`         | the operators and accessors included in `.Methods()`                                         |
| [`DependencyResolver`](#dependency-resolver)                               | built-in signature-level resolver | how the type-level dependency expectations determine the dependencies of a type              |
| [`ExcludedAttributeTypes`](./03-architecture-rules.md#type-dependencies)   | none                              | additional compiler-emitted attributes that never count as dependencies                      |

A value set on `Customize.aweXpect` applies to the current async flow only, e.g. a single test. To apply a
customization to all tests, set it on `Customize.aweXpect.Global`, e.g. once in an assembly-level setup; a value set
in the current async flow takes precedence over the global one:

```csharp
using aweXpect.Customization;
using aweXpect.Reflection;

Customize.aweXpect.Global.Reflection().IncludedSpecialNameMembers
    .Set(SpecialNameMembers.Operators);
```

## Assembly exclusions

By default, assemblies whose name matches one of the following prefixes are excluded from
`In.AllLoadedAssemblies()`:

`mscorlib`, `System`, `Microsoft`, `netstandard`, `WindowsBase`, `JetBrains`, `xunit`, `Castle`,
`DynamicProxyGenAssembly2`.

Both the assembly scanning and the dependency expectations (`DependsOnlyOn` / `DependOnlyOn` /
`WhichDependOnlyOn`, on both assemblies and types) use the same prefixes with the same matching: a prefix
matches at a name-segment boundary, so `System` covers `System` and `System.Text.Json`, but not an assembly
named `SystemsBiology`.

Customize this via `Customize.aweXpect.Settings().ExcludedAssemblyPrefixes`. `Set(…)` replaces the
list and returns a scope that restores the previous value when disposed:

```csharp
using (Customize.aweXpect.Settings().ExcludedAssemblyPrefixes
    .Set(new[] { "mscorlib", "System", "Microsoft", "MusicStore.Generated" }))
{
    // In.AllLoadedAssemblies() applies the custom prefixes within this scope
}
```

<details>
<summary>Prefixes with a trailing dot</summary>

A prefix written with a trailing dot (e.g. `MusicStore.`) is boundary-safe by construction and covers everything
starting with it.

</details>

## Compiler-generated members

By default, compiler-generated types and members are **excluded** from every navigation
(`.Types()`, `.Methods()`, `.Fields()`, …). This hides closures, async/iterator state machines,
anonymous types, local functions, auto-property backing fields and the generated members of records
(`ToString`, `Equals`, `<Clone>$`, the copy-constructor, …), so convention tests see only the members
you actually wrote.

Opt specific kinds back in with the `[Flags]` enum `CompilerGeneratedMembers`
(`None`, `Types`, `Constructors`, `Methods`, `Properties`, `Fields`, `Events`, `All`):

```csharp
using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers
    .Set(CompilerGeneratedMembers.Types | CompilerGeneratedMembers.Methods))
{
    // closures, state machines and compiler-generated methods are now visible
}
```

Operators (`op_*`) and property/event accessors (`get_`, `set_`, `add_`, `remove_`) are *user-written*
but likewise excluded by default. Include them via the separate `SpecialNameMembers` enum
(`None`, `Operators`, `Accessors`, `All`), which only affects `.Methods()`:

```csharp
using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers
    .Set(SpecialNameMembers.Operators))
{
    // operator methods are now visible in .Methods()
}
```

## Dependency resolver

The type-level dependency expectations compute a type's dependencies with a built-in signature-level
resolver (base type, interfaces, field/property/event types, method/constructor signatures, generic
arguments and applied attributes). Method-body references are not detected by the default; supply a
custom resolver, e.g. backed by [Mono.Cecil](https://github.com/jbevain/cecil) (this library takes no
dependency on it; reference the package yourself), for IL/body-level accuracy:

```csharp
// Replace the resolver within a scope
using (Customize.aweXpect.Reflection().DependencyResolver
    .Set(type => MyCecilResolver.GetBodyTypes(type)))
{
    // only the body-level references count as dependencies
}

// Or augment instead of replace: compose on the current default
var resolver = Customize.aweXpect.Reflection().DependencyResolver;
var builtin = resolver.Get()!;
using (resolver.Set(type => builtin(type).Concat(MyCecilResolver.GetBodyTypes(type))))
{
    // built-in signature dependencies plus the body-level extras
}

// Setting null reverts to the built-in default, e.g. to opt a single test out
// of a globally configured resolver
using (Customize.aweXpect.Reflection().DependencyResolver.Set(null))
{
    // the signature-level default applies within this scope
}
```

`Get()` always returns the resolver currently in effect (the built-in default when none is configured),
so composing on it works regardless of what an outer scope has set up.

<details>
<summary>Example: a resolver backed by Mono.Cecil</summary>

`MyCecilResolver.GetBodyTypes` reads the IL of the methods of a type with Mono.Cecil and returns the types that their
bodies reference: the types of local variables, created or cast types, and the types of called methods and accessed
fields. Combine it with the built-in resolver as shown above to keep the signature dependencies. It reads the assembly
file, so it does not work for an assembly that was loaded from memory or bundled into a single-file app.

```csharp
using System.Collections.Concurrent;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class MyCecilResolver
{
    private static readonly ConcurrentDictionary<Assembly, ModuleDefinition> Modules = new();

    public static IEnumerable<Type> GetBodyTypes(Type type)
        => GetBodyReferences(type).Select(ToType).OfType<Type>();

    private static IEnumerable<TypeReference> GetBodyReferences(Type type)
    {
        ModuleDefinition module = Modules.GetOrAdd(type.Assembly, a => ModuleDefinition.ReadModule(a.Location));
        TypeDefinition? definition = module.GetType(type.FullName!.Replace('+', '/'));
        if (definition is null)
        {
            yield break;
        }

        foreach (MethodDefinition method in WithGeneratedTypes(definition)
                     .SelectMany(t => t.Methods)
                     .Where(m => m.HasBody))
        {
            foreach (VariableDefinition variable in method.Body.Variables)
            {
                yield return variable.VariableType;
            }

            foreach (Instruction instruction in method.Body.Instructions)
            {
                switch (instruction.Operand)
                {
                    case TypeReference typeReference:
                        yield return typeReference;
                        break;
                    case MemberReference memberReference: // a called method or an accessed field
                        yield return memberReference.DeclaringType;
                        break;
                }
            }
        }
    }

    // Lambdas, async methods and iterators are compiled into nested types, whose bodies belong to the type.
    private static IEnumerable<TypeDefinition> WithGeneratedTypes(TypeDefinition type)
        => type.NestedTypes
            .Where(IsCompilerGenerated)
            .SelectMany(WithGeneratedTypes)
            .Prepend(type);

    private static bool IsCompilerGenerated(TypeReference type)
        => type.Name.StartsWith("<");

    private static Type? ToType(TypeReference reference)
    {
        reference = reference.GetElementType(); // strips arrays, by-refs, pointers and generic arguments
        string? assemblyName = reference.Scope switch
        {
            AssemblyNameReference name => name.FullName,
            ModuleDefinition module => module.Assembly.FullName,
            _ => null,
        };
        return reference is GenericParameter || IsCompilerGenerated(reference) || assemblyName is null
            ? null
            : Type.GetType($"{reference.FullName.Replace('/', '+')}, {assemblyName}", throwOnError: false);
    }
}
```

</details>

:::warning[The resolver must be pure]
Every resolver's output is normalized like the built-in's (array/by-ref/pointer element types and
generic arguments are unwrapped, the result is de-duplicated) and cached per type for the lifetime of
the resolver delegate, so a custom resolver needs no caching of its own. It must, however, be **pure**:
deterministic for a given `Type` within its scope; that is what makes the caching safe.
:::
