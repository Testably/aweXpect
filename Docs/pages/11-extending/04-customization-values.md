# Customization values

You can add your own [customizations](./advanced/02-customization.md) on top of the `AwexpectCustomization`
class by adding extension methods.

## Add a simple customization value

You can add a simple customizable value (e.g. an `int`):

```csharp
public static class MyCustomizationExtensions
{
    public static ICustomizationValueSetter<int> MyCustomization(this AwexpectCustomization awexpectCustomization)
        => new CustomizationValue<int>(awexpectCustomization, nameof(MyCustomization), 42);

    internal class CustomizationValue<TValue>(IAwexpectCustomization awexpectCustomization, string key, TValue defaultValue)
        : ICustomizationValueSetter<TValue>
    {
        public TValue Get()
            => awexpectCustomization.Get(key, defaultValue);

        public CustomizationLifetime Set(TValue value)
            => awexpectCustomization.Set(key, value);
    }
}
```

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

## Add a customization group

You can also add a group of customization values that can be changed individually or as a whole:

```csharp
public static class JsonAwexpectCustomizationExtensions
{
    public static JsonCustomization Json(this AwexpectCustomization awexpectCustomization)
        => new(awexpectCustomization);

    public class JsonCustomization : ICustomizationValueUpdater<JsonCustomizationValue>
    {
        private readonly IAwexpectCustomization _awexpectCustomization;

        internal JsonCustomization(IAwexpectCustomization awexpectCustomization)
        {
            _awexpectCustomization = awexpectCustomization;
            DefaultJsonDocumentOptions = new CustomizationValue<JsonDocumentOptions>(this,
                p => p.DefaultJsonDocumentOptions,
                (p, v) => p with { DefaultJsonDocumentOptions = v });
            DefaultJsonSerializerOptions = new CustomizationValue<JsonSerializerOptions>(this,
                p => p.DefaultJsonSerializerOptions,
                (p, v) => p with { DefaultJsonSerializerOptions = v });
        }

        public ICustomizationValueSetter<JsonDocumentOptions> DefaultJsonDocumentOptions { get; }
        public ICustomizationValueSetter<JsonSerializerOptions> DefaultJsonSerializerOptions { get; }

        public JsonCustomizationValue Get()
            => _awexpectCustomization.Get(nameof(Json), new JsonCustomizationValue());

        public CustomizationLifetime Update(Func<JsonCustomizationValue, JsonCustomizationValue> update)
            => _awexpectCustomization.Set(nameof(Json), update(Get()));
    }

    public record JsonCustomizationValue
    {
        public JsonDocumentOptions DefaultJsonDocumentOptions { get; set; } = new()
        {
            AllowTrailingCommas = true
        };
        public JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new()
        {
            AllowTrailingCommas = true
        };
    }

    private sealed class CustomizationValue<TValue>(
        JsonCustomization group,
        Func<JsonCustomizationValue, TValue> getter,
        Func<JsonCustomizationValue, TValue, JsonCustomizationValue> setter)
        : ICustomizationValueSetter<TValue>
    {
        public TValue Get() => getter(group.Get());

        public CustomizationLifetime Set(TValue value)
            => group.Update(p => setter(p, value));
    }
}
```

Disposing the lifetime of a single value restores the group as it was before, so dispose the lifetimes in the reverse
order in which you created them. Once all lifetimes of the group in an async flow are disposed, the flow uses the
global values again.

Both kinds of customizations work with [global defaults](./advanced/02-customization.md#global-defaults) without any
change: `Customize.aweXpect.Global.MyCustomization().Set(43)` or `Customize.aweXpect.Global.Json().Update(…)` stores
the value for all async flows, because `Global` is an `AwexpectCustomization` as well.

This allows expectations to access values either individually or for the whole group:

```csharp
 // both will return the default value 'true'
bool myCustomization1 = Customize.aweXpect.Json().Get().DefaultJsonDocumentOptions.AllowTrailingCommas;
bool myCustomization2 = Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas;
```

And users can customize either individual values or the whole group:

```csharp
// update a single value (keeping the other values)
JsonSerializerOptions mySerializerOptions = new();
using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(mySerializerOptions))
{
    // will use `mySerializerOptions` for the `JsonSerializerOptions`
    // but keep any configured `JsonDocumentOptions`
}

// ...or update the whole group
JsonAwexpectCustomizationExtensions.JsonCustomizationValue myCustomization = new();
using (Customize.aweXpect.Json().Update(_ => myCustomization))
{
    // will use all properties from `myCustomization`
}
```
