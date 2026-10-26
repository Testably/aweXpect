# Customization values

You can add your own [customizations](../08-setup/02-configuration.md) on top of the `AwexpectCustomization`
class by adding extension methods.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Customization;
```

</details>

## Add a simple customization value

You can add a simple customizable value (e.g. an `int`):

```csharp
public static class MyCustomizationExtensions
{
    public static ICustomizationValueSetter<int> MyCustomization(
        this AwexpectCustomization awexpectCustomization)
        => new CustomizationValue<int>(awexpectCustomization, "MyExtension.MyCustomization", 42);
}
```

`CustomizationValue<TValue>` from `aweXpect.Customization` stores the value under the key. The key identifies the
value, so choose one that no other package uses, e.g. prefixed with the name of your package. A `null` that was set is
returned as `null`, not as the default value, so pass a `validate` callback that rejects `null` if your value must not
be `null`, as the group below does.

This allows expectations to access the value:

```csharp
 // will return the default value of 42
int myCustomization = Customize.aweXpect.MyCustomization().Get();
```

And users can customize the value:

```csharp
using (Customize.aweXpect.MyCustomization().Set(43))
{
    // will now return 43
    int myCustomization = Customize.aweXpect.MyCustomization().Get();
}
// will now return again the default value of 42, because the customization lifetime was disposed
_ = Customize.aweXpect.MyCustomization().Get();
```

You can also use this mechanism for complex objects like classes, but then they can only be changed as a whole and not
property by property.

## Add a list-valued customization value

For a list, use `CustomizationListValue<TItem>` instead of a `CustomizationValue<IReadOnlyList<TItem>>`:

```csharp
public static class MyListCustomizationExtensions
{
    public static ICustomizationValueSetter<IReadOnlyList<string>> IgnoredTrackTitles(
        this AwexpectCustomization awexpectCustomization)
        => new CustomizationListValue<string>(awexpectCustomization, "MyExtension.IgnoredTrackTitles",
            Array.AsReadOnly<string>(["Intro", "Outro"]));
}
```

`Set` stores a read-only copy of the list and rejects `null`. Otherwise a caller who keeps the list they set, or who
casts the returned list to a mutable one, could change the value outside its scope. The default value is returned as
it is, so pass a read-only list. A `validate` callback receives the copy, e.g. to reject `null` items.

## Add a customization group

To offer several related values, add a group: a class that exposes one `ICustomizationValueSetter<TValue>` per value.
Each value is stored under its own key, so it can be set and restored independently of the other values of the group:

```csharp
public static class MusicStoreCustomizationExtensions
{
    public static MusicStoreCustomization MusicStore(this AwexpectCustomization awexpectCustomization)
        => new(awexpectCustomization);

    public class MusicStoreCustomization
    {
        internal MusicStoreCustomization(IAwexpectCustomization awexpectCustomization)
        {
            MaximumRadioDuration = new CustomizationValue<TimeSpan>(
                awexpectCustomization,
                "MyExtension.MusicStore.MaximumRadioDuration",
                TimeSpan.FromMinutes(3));
            DefaultGenre = new CustomizationValue<string>(
                awexpectCustomization,
                "MyExtension.MusicStore.DefaultGenre",
                "Rock",
                value =>
                {
                    if (value is null)
                    {
                        throw Tracing.WriteException(
                            new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
                    }
                });
        }

        public ICustomizationValueSetter<TimeSpan> MaximumRadioDuration { get; }
        public ICustomizationValueSetter<string> DefaultGenre { get; }
    }
}
```

The `validate` callback checks a value before `Set` stores it, so that `Set(null)` throws and changes nothing.
Expectations access each value on its own:

```csharp
 // will return the default value of 3 minutes
TimeSpan maximumRadioDuration = Customize.aweXpect.MusicStore().MaximumRadioDuration.Get();
```

And users customize each value on its own:

```csharp
using (Customize.aweXpect.MusicStore().DefaultGenre.Set("Pop"))
{
    // will use "Pop" as the default genre
    // but keep any configured maximum radio duration
}
```

Both kinds of customizations work with [global defaults](../08-setup/02-configuration.md#global-defaults)
without any change: `Customize.aweXpect.Global.MyCustomization().Set(43)` or
`Customize.aweXpect.Global.MusicStore().DefaultGenre.Set(…)` stores the value for all async flows, because
`Global` is an `AwexpectCustomization` as well. Their
[lifetimes](../08-setup/02-configuration.md#lifetimes-and-async-flows) behave like the ones of the built-in
values, also when they are disposed out of order.

## Add a value to a built-in group

The built-in groups `Equivalency()`, `Formatting()` and `Settings()` implement `IAwexpectCustomization` as well, so
you can add a value to one of them with an extension method on the group:

```csharp
public static class MySettingsCustomizationExtensions
{
    public static ICustomizationValueSetter<string[]> ExcludedTypePrefixes(
        this AwexpectCustomization.SettingsCustomization settings)
        => new CustomizationValue<string[]>(settings, "MyExtension.Settings.ExcludedTypePrefixes", []);
}
```

Users then find the value next to the built-in ones, e.g. `Customize.aweXpect.Settings().ExcludedTypePrefixes()`. A
group stores its values in the customization it was taken from, so
`Customize.aweXpect.Global.Settings().ExcludedTypePrefixes().Set(…)` stores the value for all async flows.

## Exclude the framework assemblies

If your extension scans assemblies, skip the ones that the user excluded with
[`ExcludedAssemblyPrefixes`](../08-setup/02-configuration.md#settings). `AssemblyPrefixes.IsExcluded` from
`aweXpect.Core.Extending` matches an assembly name with the same rule as aweXpect, at a segment boundary of the name:

```csharp
public static class MyAssemblyScanner
{
    public static IEnumerable<Assembly> ScannedAssemblies()
    {
        IReadOnlyList<string> prefixes = Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Get();
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !AssemblyPrefixes.IsExcluded(assembly.GetName().Name, prefixes));
    }
}
```

The overload without prefixes, e.g. `AssemblyPrefixes.IsExcluded("System.Text")`, reads the current setting on each
call, so read the prefixes once when you check many names.
