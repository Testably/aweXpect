# Boolean

Describes the possible expectations for `bool` and `bool?` values.

| Expectation               | Negated                     | Summary                               |
|---------------------------|-----------------------------|---------------------------------------|
| [`IsEqualTo`](#equality)  | `IsNotEqualTo`              | equal to the expected value           |
| [`IsTrue`](#true--false)  | `IsNotTrue` (only `bool?`)  | `true`                                |
| [`IsFalse`](#true--false) | `IsNotFalse` (only `bool?`) | `false`                               |
| [`IsNull`](#true--false)  | `IsNotNull` (only `bool?`)  | `null`                                |
| [`Implies`](#implication) | `DoesNotImply`              | `false`, or the other value is `true` |

## Equality

You can verify that the `bool` is equal to another one or not:

```csharp
bool isPlayed = false;

await Expect.That(isPlayed).IsEqualTo(false);
await Expect.That(isPlayed).IsNotEqualTo(true);
```

## True / False

You can verify that the `bool` is `true` or `false`:

```csharp
await Expect.That(false).IsFalse();
await Expect.That(true).IsTrue();
```

Awaiting a `bool` without any expectation is a shorthand for `IsTrue()`:

```csharp
bool isInLibrary = true;

await Expect.That(isInLibrary);
```

The negations and the `null` checks are only available for a `bool?`:

```csharp
bool? isPlayed = null;

await Expect.That(isPlayed).IsNotFalse()
  .Because("it could be true or null");
await Expect.That(isPlayed).IsNotTrue()
  .Because("it could be false or null");
await Expect.That(isPlayed).IsNull();
```

:::note
On a `bool?`, `IsNotTrue()` and `IsNotFalse()` succeed for `null`.
:::

## Implication

You can verify that `a` [implies](https://mathworld.wolfram.com/Implies.html) `b` or not:

```csharp
bool isPlayed = false;
bool isInLibrary = true;

await Expect.That(isPlayed).Implies(isInLibrary);
await Expect.That(isInLibrary).DoesNotImply(isPlayed);
```
