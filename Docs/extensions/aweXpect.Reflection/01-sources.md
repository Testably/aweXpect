# Selecting types and members

Every expectation starts from a **selection**: the assemblies, types or members the rule applies to.
This page covers the two entry points (`In` and `Types`) and navigating between assemblies, types and
members.

## Sources: the `In` helper

`In` builds the collection of reflection objects you want to reason about. Every source returns a lazily
evaluated collection that you can navigate and filter further.

| Source                                                                                        | Returns                                                                                                   |
|-----------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|
| `In.AllLoadedAssemblies()`                                                                    | all currently loaded assemblies (system assemblies [excluded](./04-configuration.md#assembly-exclusions)) |
| `In.Assemblies(a1, a2, …)` / `In.Assemblies(collection)`                                      | the given assemblies                                                                                      |
| `In.AssemblyContaining<T>()` / `In.AssemblyContaining(typeof(T))`                             | the assembly that declares `T`                                                                            |
| `In.Type<T>()` / `In.Type(typeof(T))`                                                         | a single type                                                                                             |
| `In.Types<T1, T2>()` / `In.Types<T1, T2, T3>()` / `In.Types(t1, t2, …)`                       | the given types                                                                                           |
| `In.Constructors(…)` / `In.Events(…)` / `In.Fields(…)` / `In.Methods(…)` / `In.Properties(…)` | the given members                                                                                         |

## Sources: the `Types` helper

While `In` starts from concrete reflection objects, `Types` selects types *by criteria*, the natural entry
point for architecture rules:

| Source                                             | Returns                                                                        |
|----------------------------------------------------|--------------------------------------------------------------------------------|
| `Types.InNamespace("ns")`                          | all types within a namespace and its sub-namespaces (across loaded assemblies) |
| `Types.InAllLoadedAssemblies()`                    | all types in all currently loaded assemblies                                   |
| `Types.InAssemblies(a1, a2, …)`                    | all types in the given assemblies                                              |
| `Types.InAssemblyContaining<T>()` / `…(typeof(T))` | all types in the assembly that declares `T`                                    |

`Types.InNamespace(…)` searches all loaded assemblies by default; chain one of the same `In*` methods directly
after it to clarify the assembly source (it can only be specified once, before any further filters):

```csharp
// Defaults to all loaded assemblies
await Expect.That(Types.InNamespace("MyApp.Domain")).AreSealed();

// Clarify the assembly source
await Expect.That(Types.InNamespace("MyApp.Domain").InAssemblyContaining<MyApp.Startup>())
    .AreSealed();
```

## Navigating to members

From a collection of assemblies or types you can navigate to the members they contain, and from members
back to their declaring types.

| From               | Navigate with                                                              | Yields                   |
|--------------------|----------------------------------------------------------------------------|--------------------------|
| assemblies         | `.Types()`                                                                 | the contained types      |
| assemblies / types | `.Methods()`, `.Properties()`, `.Fields()`, `.Events()`, `.Constructors()` | the contained members    |
| members            | `.DeclaringTypes()`                                                        | the declaring types      |
| types              | `.Assemblies()`                                                            | the declaring assemblies |

Assemblies also expose shorthand terminals that select a type *kind* directly:
`.Classes()`, `.Interfaces()`, `.Enums()`, `.Structs()`, `.Records()`, `.RecordStructs()`.

```csharp
// All types in all (non-system) assemblies
In.AllLoadedAssemblies().Types()

// Navigate from members back to their declaring types
In.AllLoadedAssemblies().Methods().DeclaringTypes()

// Shorthand: all public classes
In.AllLoadedAssemblies().Public.Classes()
```
