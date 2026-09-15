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
> whose .TrackCount be greater than 1 and whose .Title be equal to "Dark Side of the Moon",
> but .TrackCount was 1 and .Title was "Dark Side of the Sun" which differs at index 17:
>                      ↓ (actual)
>   "Dark Side of the Sun"
>   "Dark Side of the Moon"
>                      ↑ (expected)
> ```

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
