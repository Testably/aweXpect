# Boolean

Describes the possible expectations for boolean values.

## Equality

You can verify that the `bool` is equal to another one or not:

```csharp
bool subject = false;

await Expect.That(subject).IsEqualTo(false);
await Expect.That(subject).IsNotEqualTo(true);
```

## True / False

You can verify that the `bool` is `true` or `false`:

```csharp
await Expect.That(false).IsFalse();
await Expect.That(true).IsTrue();
```

Awaiting a `bool` without any expectation is a shorthand for `IsTrue()`:

```csharp
bool isValid = true;

await Expect.That(isValid);
```

The negation is only available for nullable booleans:

```csharp
bool? subject = null;

await Expect.That(subject).IsNotFalse()
  .Because("it could be true or null");
await Expect.That(subject).IsNotTrue()
  .Because("it could be false or null");
```

## Implication

You can verify that `a` implies `b` (*find [here](https://mathworld.wolfram.com/Implies.html) a mathematical
explanation*):

```csharp
bool a = false;
bool b = true;

await Expect.That(a).Implies(b);
```
