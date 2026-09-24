# Customization

You can customize certain behavior or specify default values to use globally.

All customizations are located in the static `Customize.aweXpect` class. Customization values are grouped and have a dedicated `Get` and `Set` method.
The `Set` method always returns a lifetime scope which is an `IDisposable` object that will revert the customization value to its previous value upon disposal.

The customization options are applied in an [async context](https://learn.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1) which means that they don't directly influence other parallel tests.

```csharp
using aweXpect.Customization;

using (Customize.aweXpect.Formatting().MaximumStringLength.Set(500))
{
    // strings of up to 500 characters are shown in full here
}
```


## Equivalency

Under `Customize.aweXpect.Equivalency()` you have:
- **DefaultEquivalencyOptions**  
  The [equivalency options](/docs/expectations/equivalency#customizing-the-global-defaults) that are used when an
  expectation does not configure them.


## Formatting

Under `Customize.aweXpect.Formatting()` you have:
- **MaximumNumberOfCollectionItems**  
  The maximum number of displayed items in a collection.
  The remaining items are summarized at the end of the list: `(… and 7 more)` when the total number of items is known,
  and `(… and maybe more)` when it is not, e.g. for a lazy sequence or when the expectation stopped enumerating early.

- **MaximumStringLength**  
  The maximum length of a displayed `string` before it gets truncated.

- **MinimumNumberOfCharactersAfterStringDifference**  
  The minimum number of characters included after the first mismatch in the string difference.


## Json

Under `Customize.aweXpect.Json()`, which comes with the separate
[`aweXpect.Json`](https://github.com/aweXpect/aweXpect.Json) package, you have:
- **DefaultJsonDocumentOptions**  
  The default [options](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) used to parse a `JsonDocument`. 

- **DefaultJsonSerializerOptions**  
  The default [options](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) used for the `JsonSerializer`.


## Reflection

Under `Customize.aweXpect.Reflection()` you have:
- **ExcludedAssemblyPrefixes**  
  The assembly namespace prefixes that are excluded during reflection.


## Settings

Under `Customize.aweXpect.Settings()` you have:
- **TestCancellation**  
  A cancellation logic that is applied for all tests. This can be one of the following:
  - `FromTimeout(TimeSpan timeout)`  
    This will cancel the `CancellationToken` that is used internally and forwarded to the [delegates](/docs/expectations/delegates) after the given timeout.
  - `FromCancellationToken(Func<CancellationToken> cancellationTokenFactory)`  
    This will use the returned `CancellationToken` internally and also forward it to the [delegates](/docs/expectations/delegates).

- **DefaultCheckInterval**  
  The default interval for repeatedly checking the condition on an object.

- **DefaultEventuallyTimeout**  
  The default timeout until the expectations of [`Eventually()`](/docs/expectations/delegates) on a delegate must be met.

- **DefaultSignalerTimeout**  
  The default timeout for the [`Signaler`](/docs/expectations/advanced/callbacks).

- **DefaultTimeComparisonTolerance**  
  The default tolerance when a date or time subject is compared directly, see [Default Tolerance](/docs/expectations/common-types/datetime-offset#default-tolerance).
  *Note: In Windows the `DateTime` resolution is [about 10 to 15 milliseconds](https://stackoverflow.com/q/3140826/4003370)*
