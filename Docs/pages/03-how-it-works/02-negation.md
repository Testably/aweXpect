# Negation

Almost every expectation has a negated counterpart that starts with `IsNot…` or `DoesNot…`, e.g. `IsNotEqualTo`,
`DoesNotContain` or `DoesNotStartWith`. The `Has…` properties take a negated comparison instead, e.g.
`HasLength().NotEqualTo(9)`.

`DoesNotComplyWith` negates any expectation, including your own:

```csharp
string subject = "Abbey Road";

await Expect.That(subject).DoesNotComplyWith(it => it.StartsWith("Let").And.EndsWith("Be"));
```

`DoesNotComplyWith` is the exact inverse: it succeeds as soon as the nested expectation fails. A named negation can be
stricter, e.g. [`DoesNotContainKeys`](./03-collections/04-dictionaries.md#values) fails as soon as any of the keys is contained.
