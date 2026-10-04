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

await Expect.That(title).StartsWith("Let").And.EndsWith("Road");
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

await Expect.That(title).IsNull().Or.Whose(x => x.Length, x => x.IsEqualTo(9));
```

`.And` does not short-circuit: all expectations are evaluated, so that the failure message can report all of them.

## On members of the subject

Use `Whose` to verify different members of a common subject and combine them again with `.And` or `.Or`:

```csharp
record Album(int TrackCount, string Title);
Album album = new(1, "Dark Side of the Sun");

await Expect.That(album)
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
await Expect.That(album)
  .Whose(x => x.LoadTitleAsync(), x => x.IsEqualTo("Dark Side of the Moon"));
```

## On a new subject

`Which` continues with a new subject, e.g. the single item of a collection or the thrown exception:

```csharp
IEnumerable<int> playCounts = [42];
void Act() => throw new CustomException("Yesterday");

await Expect.That(playCounts).HasSingle().Which.IsGreaterThan(41);
await Expect.That(Act).Throws<CustomException>().Which.HasMessage("Yesterday");
```

## On different subjects

Use `Expect.ThatAll` or `Expect.ThatAny` to combine arbitrary expectations. `ThatAll` requires all of them to
succeed, `ThatAny` at least one:

```csharp
string album = "Abbey Road";
string song = "Something";

await Expect.ThatAll(
  Expect.That(album).IsEqualTo("Abbey Road"),
  Expect.That(song).IsEqualTo("Yesterday"));
await Expect.ThatAny(
  Expect.That(album).IsEqualTo("Let It Be"),
  Expect.That(song).IsEqualTo("Something"));
```

```text title="Failure message of ThatAll"
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

## Using the result

Awaiting an expectation returns the value it verified, e.g. the single item of a collection, so you can use it
afterwards:

```csharp
IEnumerable<int> playCounts = [42];

int single = await Expect.That(playCounts).HasSingle();
```
