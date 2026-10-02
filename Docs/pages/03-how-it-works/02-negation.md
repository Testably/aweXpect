# Negation

Almost every expectation has a negated counterpart that starts with `IsNot…` or `DoesNot…`, e.g. `IsNotEqualTo`,
`DoesNotContain` or `DoesNotStartWith`. The `Has…` properties take a negated comparison instead, e.g.
`HasLength().NotEqualTo(9)`, and the quantifiers of a collection use `None()`. The table at the top of each reference
page lists the negated counterparts.

`DoesNotComplyWith` negates any expectation, including your own:

```csharp
string title = "Abbey Road";

await Expect.That(title).DoesNotComplyWith(it => it.StartsWith("Let").And.EndsWith("Be"));
```

`DoesNotComplyWith` is the exact inverse: it succeeds as soon as the nested expectation fails, except for a `null`
subject (see below). A named negation can be stricter, e.g.
[`DoesNotContainKeys`](../05-collections/04-dictionaries.md#values) fails as soon as any of the keys is contained, while
`DoesNotComplyWith(d => d.ContainsKeys(…))` only fails when all of them are.

A `null` subject fails most expectations and their negations alike, also inside `DoesNotComplyWith`, see
[`null` subjects](./04-null-subjects.md).
