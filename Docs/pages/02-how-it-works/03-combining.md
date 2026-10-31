---
description: Combine expectations on the same subject, on its members, on a new subject or on different subjects.
---

# Combining expectations

Expectations can be combined on the same subject, on its members or on different subjects:

| Syntax                                                       | Combines                                                            |
|--------------------------------------------------------------|---------------------------------------------------------------------|
| [`.And`, `.Or`](#on-the-same-subject)                        | expectations on the same subject                                    |
| [`Whose(member, …)`](#on-members-of-the-subject)             | expectations on a member, and keeps the subject for further ones    |
| [`Which`](#on-a-new-subject)                                 | continues with a new subject, e.g. the single item or the exception |
| [`Expect.ThatAll`, `Expect.ThatAny`](#on-different-subjects) | expectations on different subjects                                  |

## On the same subject

Use `.And` or `.Or` to combine multiple expectations on the same subject:

```csharp
string title = "Let It Be";

await That(title).StartsWith("Let").And.EndsWith("Road");
```

```text title="Failure message"
Expected that title
starts with "Let" and ends with "Road",
but it was "Let It Be", which differs at index 8:
           ↓ (actual)
  "Let It Be"
       "Road"
           ↑ (expected suffix)
```

`.And` binds tighter than `.Or`, so `A.And.B.Or.C` is evaluated as `(A && B) || C`.

`.Or` short-circuits: as soon as one alternative is met, the following ones are not evaluated anymore, which allows
using it as a guard:

```csharp
string? title = null;

await That(title).IsNull().Or.Whose(x => x.Length, x => x.IsEqualTo(9));
```

`.And` does not short-circuit: all expectations are evaluated, so that the failure message can report all of them.

<details>
<summary>Expectations that only apply under a condition</summary>

aweXpect has no built-in way to skip part of a chain. When a part only applies under a condition, you can add an
extension method like this one to your test project instead of duplicating the whole expectation in an `if`/`else`:

```csharp
using aweXpect.Core;
using aweXpect.Results;

public static AndOrResult<TType, TThat, TSelf> AndIf<TType, TThat, TSelf>(
  this AndOrResult<TType, TThat, TSelf> result,
  bool condition,
  Func<TThat, Expectation> expectations)
  where TSelf : AndOrResult<TType, TThat, TSelf>
{
  if (condition)
  {
    expectations(result.And);
  }

  return result;
}
```

```csharp
string title = "Let It Be (Remastered)";
bool isRemastered = true;

await That(title).StartsWith("Let It Be")
  .AndIf(isRemastered, x => x.EndsWith("(Remastered)"));
```

When the condition is `false`, the expectations in the callback are neither evaluated nor part of the failure message.
The callback has to return the expectation, so an empty callback does not compile.

</details>

## On members of the subject

Use `Whose` to verify different members of a common subject and combine them again with `.And` or `.Or`:

```csharp
record Album(int TrackCount, string Title);
Album album = new(1, "Dark Side of the Sun");

await That(album)
  .Whose(x => x.TrackCount, x => x.IsGreaterThan(1)).And
  .Whose(x => x.Title, x => x.IsEqualTo("Dark Side of the Moon"));
```

```text title="Failure message"
Expected that album
whose TrackCount is greater than 1 and whose Title is equal to "Dark Side of the Moon",
but TrackCount was 1 and Title was "Dark Side of the Sun", which differs at index 17:
                ↓ (actual)
  "…Side of the Sun"
  "…Side of the Moon"
                ↑ (expected)
```

When the selector returns a `Task<T>` or `ValueTask<T>`, the expectations apply to the awaited result:

```csharp
await That(album)
  .Whose(x => x.LoadTitleAsync(), x => x.IsEqualTo("Dark Side of the Moon"));
```

After a type check with `Is<T>()` or `IsExactly<T>()`, `Whose(…)` verifies a member of the cast value, and
`AndWhose(…)` adds further members, e.g. `That(subject).Is<Album>().Whose(x => x.Title, …).AndWhose(…)`.

## On a new subject

`Which` continues with a new subject, e.g. the single item of a collection or the thrown exception:

```csharp
IEnumerable<int> playCounts = [42];
void Act() => throw new CustomException("Yesterday");

await That(playCounts).HasSingle().Which.IsGreaterThan(41);
await That(Act).Throws<CustomException>().Which.HasMessage("Yesterday");
```

## On different subjects

Use `Expect.ThatAll` or `Expect.ThatAny` to combine arbitrary expectations. `ThatAll` requires all of them to
succeed, `ThatAny` at least one:

```csharp
string album = "Abbey Road";
string song = "Something";

await Expect.ThatAll(
  That(album).IsEqualTo("Abbey Road"),
  That(song).IsEqualTo("Yesterday"));
await Expect.ThatAny(
  That(album).IsEqualTo("Let It Be"),
  That(song).IsEqualTo("Something"));
```

```text title="Failure message"
Expected all of the following to succeed:
 [01] Expected that album is equal to "Abbey Road"
 [02] Expected that song is equal to "Yesterday"
but
 [02] it was "Something", which differs at index 0:
         ↓ (actual)
        "Something"
        "Yesterday"
         ↑ (expected)
```

Where awaiting is impossible, a combination can be [verified synchronously](./00-index.md#when-you-cannot-await) like a
single expectation.

A combination offers the same `Because`, `WithTimeout` and `WithCancellation` as a single expectation:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

Task<string> album = Task.FromResult("Abbey Road");
Task<string> song = Task.FromResult("Something");

await Expect.ThatAll(
  That(album).IsEqualTo("Abbey Road"),
  That(song).IsEqualTo("Something"))
  .Because("the playlist starts with this song")
  .WithTimeout(5.Seconds());
```

- `Because(reason)` adds the reason to the first line of the failure message, e.g.
  `Expected all of the following to succeed, because the playlist starts with this song:`. The combined expectations
  keep their own reasons.
- `WithTimeout(timeout)` limits the whole combination.
- `WithCancellation(cancellationToken)` is passed to every combined expectation.

<details>
<summary>How the timeout and the cancellation apply to the combined expectations</summary>

The expectations are evaluated one after another, so each one only gets the time that is left of the timeout. Once it
elapsed, an expectation that does not wait is still verified, while one that would wait fails with "did not finish
within …". The tighter timeout wins, as for a [single expectation](./06-time-and-cancellation.md#timeout): a combined
expectation keeps a shorter timeout of its own. A combined expectation with a `CancellationToken` of its own observes
both tokens.

</details>

## Using the result

Awaiting an expectation returns the value it verified, e.g. the single item of a collection, so you can use it
afterwards:

```csharp
IEnumerable<int> playCounts = [42];

int single = await That(playCounts).HasSingle();
```

After an `.Or`, the value can be the `default` of its type when an earlier alternative was the one that was met; the
analyzer rule [`aweXpect0008`](../08-analyzers.md#awexpect0008) warns when such a value is used.
