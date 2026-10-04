# Customization values

You can add your own [customizations](../03-how-it-works/07-configuration.md) on top of the `AwexpectCustomization`
class by adding extension methods.

The samples on this page use the following namespaces:

```csharp
using System.Text.Json;
using aweXpect.Customization;
```

## Add a simple customization value

You can add a simple customizable value (e.g. an `int`):

```csharp
public static class MyCustomizationExtensions
{
    public static ICustomizationValueSetter<int> MyCustomization(this AwexpectCustomization awexpectCustomization)
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

## Add a customization group

To offer several related values, add a group: a class that exposes one `ICustomizationValueSetter<TValue>` per value.
Each value is stored under its own key, so it can be set and restored independently of the other values of the group:

```csharp
public static class JsonAwexpectCustomizationExtensions
{
    public static JsonCustomization Json(this AwexpectCustomization awexpectCustomization)
        => new(awexpectCustomization);

    public class JsonCustomization
    {
        internal JsonCustomization(IAwexpectCustomization awexpectCustomization)
        {
            DefaultJsonDocumentOptions = new CustomizationValue<JsonDocumentOptions>(awexpectCustomization,
                "MyExtension.Json.DefaultJsonDocumentOptions", new JsonDocumentOptions { AllowTrailingCommas = true });
            DefaultJsonSerializerOptions = new CustomizationValue<JsonSerializerOptions>(awexpectCustomization,
                "MyExtension.Json.DefaultJsonSerializerOptions", new JsonSerializerOptions { AllowTrailingCommas = true },
                value =>
                {
                    if (value is null)
                    {
                        throw new ArgumentNullException(nameof(value), "The 'value' cannot be null.");
                    }
                });
        }

        public ICustomizationValueSetter<JsonDocumentOptions> DefaultJsonDocumentOptions { get; }
        public ICustomizationValueSetter<JsonSerializerOptions> DefaultJsonSerializerOptions { get; }
    }
}
```

The `validate` callback checks a value before `Set` stores it, so that `Set(null)` throws and changes nothing.
Expectations access each value on its own:

```csharp
 // will return the default value 'true'
bool allowTrailingCommas = Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas;
```

And users customize each value on its own:

```csharp
JsonSerializerOptions mySerializerOptions = new();
using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(mySerializerOptions))
{
    // will use `mySerializerOptions` for the `JsonSerializerOptions`
    // but keep any configured `JsonDocumentOptions`
}
```

Both kinds of customizations work with [global defaults](../03-how-it-works/07-configuration.md#global-defaults)
without any change: `Customize.aweXpect.Global.MyCustomization().Set(43)` or
`Customize.aweXpect.Global.Json().DefaultJsonSerializerOptions.Set(…)` stores the value for all async flows, because
`Global` is an `AwexpectCustomization` as well. Their
[lifetimes](../03-how-it-works/07-configuration.md#lifetimes-and-async-flows) behave like the ones of the built-in
values, also when they are disposed out of order.
