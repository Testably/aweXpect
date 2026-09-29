# `null` subjects

A `null` subject fails an expectation **and its negation**, unless the expectation is about `null`:

- An expectation that inspects the subject, e.g. `IsEmpty()`, `HasLength(3)` or `IsGreaterThan(2)`, has nothing to
  inspect, so both `IsEmpty()` and `IsNotEmpty()` fail for a `null` subject.
- Equality and identity comparisons treat `null` as an ordinary value, so `IsEqualTo(null)` succeeds for a `null`
  subject and `IsNotEqualTo("foo")` succeeds as well.
- The explicit `null` checks, such as `IsNull()` or `IsNullOrEmpty()`, are satisfied by a `null` subject.

```csharp
string? subject = null;

await Expect.That(subject).IsNull();
await Expect.That(subject).IsEqualTo(null);
await Expect.That(subject).IsNotEqualTo("Abbey Road");
```
