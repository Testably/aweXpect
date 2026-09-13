# Write your own extension

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=aweXpect.Core)](https://www.nuget.org/packages/aweXpect.Core)

This library will never be able to cope with all ideas and use cases. Therefore, it is possible to use the [
`aweXpect.Core`](https://www.nuget.org/packages/aweXpect.Core/) package and write your own extensions.
Goal of this package is to be more stable than the main aweXpect package, so reduce the risk of version conflicts
between different extensions.

## Expectations

You can extend the expectations for any types, by adding extension methods on `IThat<TType>`.

If you want to verify that a `string` is an absolute path, you specify the following method signature:

```csharp
/// <summary>
///     Verifies that the <paramref name="subject"/> is an absolute path.
/// </summary>
public static AndOrResult<string, IThat<string>> IsAbsolutePath(this IThat<string> subject)
{
    // ...
}
```

### ExpectationBuilder

The next step is to extract the `ExpectationBuilder`. In order to keep the automatic code suggestions for developers
clear, you have to cast the `IThat<TType>` interface to `IExpectThat<TType>`, which will then give access to the
`ExpectationBuilder` property.
To improve readability you can copy the following internal extension method into your project:

```csharp
[ExcludeFromCodeCoverage]
internal static IExpectThat<T> Get<T>(this IThat<T> subject)
{
    if (subject is IExpectThat<T> thatIs)
    {
        return thatIs;
    }

    throw new NotSupportedException("IThat<T> must also implement IExpectThat<T>");
}
```

You can then use the `ExpectationBuilder` to add a `IsAbsolutePathConstraint`:

```csharp
public static AndOrResult<string, IThat<string>> IsAbsolutePath(this IThat<string> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsAbsolutePathConstraint(it, grammars)),
        subject);
```

### Constraints

The basis for expectations are constraints. You can add different constraints to the `ExpectationBuilder` that is
available for the `IThat<T>`. They differ in the input and output parameters for the `IsMetBy` method:

- `IValueConstraint<T>`   
  It receives the actual value `T` and returns a `ConstraintResult`.
- `IAsyncConstraint<T>`  
  It receives the actual value `T` and a `CancellationToken` and returns the `ConstraintResult` asynchronously.  
  *Use it when you need asynchronous functionality or access to the timeout `CancellationToken`.*
- `IContextConstraint<T>` / `IAsyncContextConstraint<T>`  
  Similar to the `IValueConstraint<T>` and `IAsyncConstraint<T>` respectively but receives an additional
  `IEvaluationContext` parameter that allows storing and receiving data between expectations.  
  *This mechanism is used for example to avoid enumerating an `IEnumerable` multiple times across multiple constraints.*

```csharp
/// <summary>
///     This example does NOT support the negated case!
/// </summary>
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult(grammars),
        IValueConstraint<string>
{
    private string? _actual;
    public ConstraintResult IsMetBy(string actual)
    {
        _actual = actual;
        Outcome = Path.IsPathRooted(actual) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is an absolute path");

    public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(it).Append(" was ");
        Formatter.Format(stringBuilder, _actual);
    }

    public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
    {
        if (_actual is TValue typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return typeof(string).IsAssignableTo(typeof(TValue));
    }

    public override ConstraintResult Negate() => this;
}
```

All constraints should also provide the expectations and results for the negated case (so that they are compatible with
`DoesNotComplyWith`).

In order to streamline common cases, the recommended practice is to use the same class also for the `ConstraintResult`;
in most cases with one of the following helper classes:

- `ConstraintResult.WithValue<T>`
  You have to set the `Actual` property in the `IsMetBy` method and overwrite `AppendNormalExpectation` and
  `AppendNegatedExpectation` as well as either the corresponding `AppendNormalResult` and `AppendNegatedResult` or the
  common method for both cases `AppendResult` (when the result text is identical in both cases)
- `ConstraintResult.WithNotNullValue<T>`
  Similar to `ConstraintResult.WithValue<T>`, but will automatically include a check that Actual is not `null` with the
  generic result text.
- `ConstraintResult.WithEqualToValue<T>`
  Ensures consistent `null`-handling when comparing two values for equality. Similar to `ConstraintResult.WithValue<T>`,
  but you have to also provide a flag, indicating if the expected value is `null` or not.

Which of the three to pick is decided by how your expectation treats a `null` subject, and that follows one rule:
a `null` subject fails every expectation, except those that compare it against a value the caller supplied.

- Your expectation **inspects the subject** - its length, its type, its items, whether it is empty. There is nothing to
  inspect when the subject is `null`, so it has to fail, in the negated case as well: `IsNotEmpty()` fails for a `null`
  subject just like `IsEmpty()` does. Use `ConstraintResult.WithNotNullValue<T>`.
- Your expectation **compares the subject** against a value the caller supplied. Then `null` is an ordinary value on
  both sides: `IsEqualTo(null)` succeeds for a `null` subject, `IsNotEqualTo(null)` fails and `IsNotEqualTo("foo")`
  succeeds. Use `ConstraintResult.WithEqualToValue<T>`.

Use `ConstraintResult.WithValue<T>` only when the subject cannot be `null` at all, for example a non-nullable `bool`,
`int` or `DateTime`.

With these the above example could be written (with support for the negated case):

```csharp
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithValue<string>(grammars),
        IValueConstraint<string>
{
    public ConstraintResult IsMetBy(string actual)
    {
        Actual = actual;
        Outcome = Path.IsPathRooted(actual) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is an absolute path");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(it).Append(" was ");
        Formatter.Format(stringBuilder, Actual);
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is no negated path");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(it).Append(" was ");
        Formatter.Format(stringBuilder, Actual);
    }
}
```

This then also allows you to write an explicit negated expectation with the same constraint using the `.Invert()`
method:

```csharp
/// <summary>
///     Verifies that the <paramref name="subject"/> is no absolute path.
/// </summary>
public static AndOrResult<string, IThat<string>> IsNoAbsolutePath(
    this IThat<string> subject)
    => new(subject.ThatIs().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsAbsolutePathConstraint(it, grammars).Invert()),
        subject);
```

## Customization

You can add you own [customizations](/docs/expectations/advanced/customization) on top of the `AwexpectCustomization`
class by adding extension methods.

### Add a simple customization value

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

*Note: you can also use this mechanism for complex objects like classes, but they can only be changed as a whole (and
not individual properties)*

### Add a customization group

You can also add a group of customization values, that can be changed individually or as a whole

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
            DefaultJsonDocumentOptions = new CustomizationValue<JsonDocumentOptions>(
                () => Get().DefaultJsonDocumentOptions,
                v => Update(p => p with { DefaultJsonDocumentOptions = v }));
            DefaultJsonSerializerOptions = new CustomizationValue<JsonSerializerOptions>(
                () => Get().DefaultJsonSerializerOptions,
                v => Update(p => p with { DefaultJsonSerializerOptions = v }));
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
        Func<TValue> getter,
        Func<TValue, CustomizationLifetime> setter)
        : ICustomizationValueSetter<TValue>
    {
        public TValue Get() => getter();
        public CustomizationLifetime Set(TValue value) => setter(value);
    }
}
```

This allows expectations to access values either individually or for the whole group:

```csharp
 // both will return the default value 'true'
int myCustomization1 = Customize.aweXpect.Json().Get().DefaultJsonDocumentOptions.AllowTrailingCommas;
int myCustomization2 = Customize.aweXpect.Json().DefaultJsonDocumentOptions.Get().AllowTrailingCommas;
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
JsonCustomizationValue myCustomization = new();
using (Customize.aweXpect.Json().Update(_ => myCustomization))
{
    // will use the all set properties from the `myCustomization`
}
```
