# Multiple expectations

You can combine multiple expectations in different ways:

## On the same property

Simply use `.And` or `.Or` to combine multiple expectations, e.g.

```csharp
string subject = "something different";
await Expect.That(subject).StartsWith("some").And.EndsWith("text");
```

> ```
> Expected subject to
> start with "some" and end with "text",
> but it was "something different"
> ```

`.And` binds tighter than `.Or`, so `A.And.B.Or.C` is evaluated as `(A && B) || C`.

`.Or` short-circuits: as soon as one alternative is met, the following ones are not evaluated anymore, which allows
using it as a guard, e.g.

```csharp
await Expect.That(subject).IsNull().Or.Whose(x => x.Length, x => x.IsEqualTo(2));
```

`.And` does not short-circuit: all expectations are evaluated, so that the failure message can report all of them.

## On different properties of the same subject

Use the `Whose`-syntax to access different properties of a common subject and combine them again with `.And` or `.Or`,
e.g.

```csharp
  public record Album(int TrackCount, string Title);
  Album subject = new(1, "Dark Side of the Sun");
  
  await Expect.That(subject)
    .Whose(x => x.TrackCount, x => x.IsGreaterThan(1)).And
    .Whose(x => x.Title, x => x.Is("Dark Side of the Moon"));
```

> ```
> Expected subject to
> whose TrackCount be greater than 1 and whose Title be equal to "Dark Side of the Moon",
> but TrackCount was 1 and Title was "Dark Side of the Sun" which differs at index 17:
>                      ↓ (actual)
>   "Dark Side of the Sun"
>   "Dark Side of the Moon"
>                      ↑ (expected)
> ```

When the selector returns a `Task<T>` or `ValueTask<T>`, the expectations apply to the awaited result, e.g.

```csharp
  await Expect.That(subject)
    .Whose(x => x.LoadTitleAsync(), x => x.IsEqualTo("Dark Side of the Moon"));
```

## On different subjects

Use the `Expect.ThatAll` or `Expect.ThatAny` syntax to combine arbitrary expectations, e.g.

```csharp
  string subjectA = "ABC";
  string subjectB = "XYZ";
  
  await Expect.ThatAll(
    Expect.That(subjectA).Is("ABC"),
    Expect.That(subjectB).Is("DEF"));
```

> ```
> Expected all of the following to succeed:
>  [01] Expected subjectA to be equal to "ABC"
>  [02] Expected subjectB to be equal to "DEF"
> but
>  [02] it was "XYZ" which differs at index 0:
>          ↓ (actual)
>         "XYZ"
>         "DEF"
>          ↑ (expected)
> ```
