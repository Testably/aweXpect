# `null` subjects

A `null` subject fails an expectation **and its negation**, unless the expectation is about `null`:

- An expectation that inspects the subject, e.g. `IsEmpty()`, `HasLength(3)` or `IsGreaterThan(2)`, has nothing to
  inspect, so both `IsEmpty()` and `IsNotEmpty()` fail for a `null` subject.
- Equality and identity comparisons treat `null` as an ordinary value, so `IsEqualTo(null)` succeeds for a `null`
  subject and `IsNotEqualTo("Abbey Road")` succeeds as well.
- The explicit `null` checks, such as `IsNull()` or `IsNullOrEmpty()`, are satisfied by a `null` subject.

```csharp
string? title = null;

await Expect.That(title).IsNull();
await Expect.That(title).IsEqualTo(null);
await Expect.That(title).IsNotEqualTo("Abbey Road");
```

This also applies inside [`DoesNotComplyWith`](./02-negation.md). How an extension follows the same rule is described
in [constraints and results](../11-extending/02-constraints-and-results.md#null-subjects).
