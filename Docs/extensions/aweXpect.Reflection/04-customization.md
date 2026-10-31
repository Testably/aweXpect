---
description: Customize the assembly exclusions, the compiler-generated members and the dependency resolver of aweXpect.Reflection.
---

# Customization

Set a value under `Customize.aweXpect.Reflection()` for the current
[async flow](/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Reflection()` for
[all tests](/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                                      | Default                           | Summary                                                                                      |
|----------------------------------------------------------------------------|-----------------------------------|----------------------------------------------------------------------------------------------|
| [`Settings().ExcludedAssemblyPrefixes`](#assembly-exclusions)              | `System`, `Microsoft`, …          | assemblies excluded from `In.AllLoadedAssemblies()` and ignored by the `DependOnlyOn` family |
| [`IncludedCompilerGeneratedMembers`](#compiler-generated-members)          | `CompilerGeneratedMembers.None`   | the compiler-generated types and members included in the navigations                         |
| [`IncludedSpecialNameMembers`](#compiler-generated-members)                | `SpecialNameMembers.None`         | the operators and accessors included in `.Methods()`                                         |
| [`DependencyResolver`](#dependency-resolver)                               | built-in signature-level resolver | how the type-level dependency expectations determine the dependencies of a type              |
| [`ExcludedAttributeTypes`](./03-architecture-rules.md#type-dependencies)   | none                              | additional compiler-emitted attributes that never count as dependencies                      |

`Customize.aweXpect.Reflection()` and the enums `CompilerGeneratedMembers` and `SpecialNameMembers` live in the
namespace `aweXpect.Reflection`. The assembly exclusions are shared with aweXpect.Core and live under
`Customize.aweXpect.Settings()`. For example, to include the operators in `.Methods()` for all tests:

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
list:

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
    .Set(CecilDependencyResolver.GetUsedTypes))
{
    // the references in method bodies count as dependencies, too
}

// Or augment instead of replace: compose on the current default
ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> resolver = Customize.aweXpect.Reflection().DependencyResolver;
Func<Type, IEnumerable<Type>> builtin = resolver.Get()!;
using (resolver.Set(type => builtin(type).Concat(CecilDependencyResolver.GetUsedTypes(type))))
{
    // the built-in dependencies plus the ones the Cecil resolver finds
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

`CecilDependencyResolver.GetUsedTypes` reads a type with Mono.Cecil: its signature surface and the IL of its method
bodies, i.e. the types of local variables, created or cast types, and the types of called methods and accessed fields.
It reads the assembly file, so it skips an assembly that was loaded from memory or bundled into a single-file app.
Code in lambdas, async methods and iterators is compiled into nested types, which are resolved as types of their own.

```csharp
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

/// <summary>
///     Example of a custom resolver for <c>Customize.aweXpect.Reflection().DependencyResolver</c>, backed by
///     Mono.Cecil: in addition to the declared signature surface it reads method bodies from the IL, so
///     references that only occur inside a body (object creations, static calls, locals) are detected as
///     dependencies, too, which the built-in signature-level resolver cannot see.
/// </summary>
/// <remarks>
///     The output does not need to be unwrapped, de-duplicated or cached here: the library normalizes and
///     memoizes every resolver's output per type.
/// </remarks>
public static class CecilDependencyResolver
{
    public static IEnumerable<Type> GetUsedTypes(Type type)
    {
        if (type.FullName is null || string.IsNullOrEmpty(type.Assembly.Location))
        {
            yield break;
        }

        using AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
        TypeDefinition? definition = assembly.MainModule.GetType(type.FullName.Replace('+', '/'));
        if (definition is null)
        {
            yield break;
        }

        foreach (TypeReference reference in EnumerateTypeReferences(definition))
        {
            foreach (TypeReference flattened in Flatten(reference))
            {
                if (ResolveRuntimeType(flattened) is { } resolved)
                {
                    yield return resolved;
                }
            }
        }
    }

    private static IEnumerable<TypeReference> EnumerateTypeReferences(TypeDefinition definition)
    {
        if (definition.BaseType is not null)
        {
            yield return definition.BaseType;
        }

        foreach (InterfaceImplementation @interface in definition.Interfaces)
        {
            yield return @interface.InterfaceType;
        }

        foreach (FieldDefinition field in definition.Fields)
        {
            yield return field.FieldType;
        }

        foreach (PropertyDefinition property in definition.Properties)
        {
            yield return property.PropertyType;
        }

        foreach (EventDefinition @event in definition.Events)
        {
            yield return @event.EventType;
        }

        foreach (MethodDefinition method in definition.Methods)
        {
            yield return method.ReturnType;

            foreach (ParameterDefinition parameter in method.Parameters)
            {
                yield return parameter.ParameterType;
            }

            if (!method.HasBody)
            {
                continue;
            }

            foreach (VariableDefinition variable in method.Body.Variables)
            {
                yield return variable.VariableType;
            }

            foreach (Instruction instruction in method.Body.Instructions)
            {
                TypeReference? operandType = instruction.Operand switch
                {
                    TypeReference typeReference => typeReference,
                    MemberReference memberReference => memberReference.DeclaringType,
                    _ => null,
                };
                if (operandType is not null)
                {
                    yield return operandType;
                }
            }
        }
    }

    /// <summary>
    ///     Strips array/by-ref/pointer wrappers and flattens generic instances into their definition and
    ///     arguments, so that every part can be resolved by full name.
    /// </summary>
    private static IEnumerable<TypeReference> Flatten(TypeReference reference)
    {
        while (reference is TypeSpecification specification and not GenericInstanceType)
        {
            reference = specification.ElementType;
        }

        if (reference is Mono.Cecil.GenericParameter)
        {
            yield break;
        }

        if (reference is GenericInstanceType generic)
        {
            yield return generic.ElementType;
            foreach (TypeReference argument in generic.GenericArguments)
            {
                foreach (TypeReference flattened in Flatten(argument))
                {
                    yield return flattened;
                }
            }
        }
        else
        {
            yield return reference;
        }
    }

    private static Type? ResolveRuntimeType(TypeReference reference)
    {
        string assemblyName = reference.Scope switch
        {
            AssemblyNameReference assemblyReference => assemblyReference.FullName,
            ModuleDefinition module => module.Assembly.FullName,
            _ => reference.Module.Assembly.FullName,
        };

        return Type.GetType($"{reference.FullName.Replace('/', '+')}, {assemblyName}", false);
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
