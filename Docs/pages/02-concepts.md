---
sidebar_position: 1.5
---

# Concepts

Describes how an expectation is built, what its failure message says and the rules that apply to all expectations.

## Anatomy of an expectation

Every expectation starts with `Expect.That(subject)`, continues with what you expect and is awaited:

```csharp
string subject = "Let It Be";

await Expect.That(subject).StartsWith("Abbey").Because("it is the album title");
```

The expectation is only evaluated when it is awaited. A failure throws the exception of your test framework, with a
message that reads like a sentence. The expectation above fails with:

```text title="Failure message"
Expected that subject
starts with "Abbey", because it is the album title,
but it was "Let It Be", which differs at index 0:
   ↓ (actual)
  "Let It Be"
  "Abbey"
   ↑ (expected prefix)
```

- The first line names the subject with the expression you passed to `Expect.That`.
- The second line states the expectation, followed by the reason from `Because(…)`.
- The line starting with "but" describes what was found instead.

## Negation

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

## Combining expectations

- `.And` and `.Or` combine expectations on the same subject, and `Expect.ThatAll` or `Expect.ThatAny` combine
  expectations on different subjects (see [multiple expectations](./advanced/01-multiple-expectations.md)).
- `Whose(member, expectations)` verifies a member of the subject and keeps the subject for further expectations.
- `Which` continues with a new subject, e.g. the single item of a collection or the thrown exception:

```csharp
IEnumerable<int> values = [42];
void Act() => throw new CustomException("my exception");

await Expect.That(values).HasSingle().Which.IsGreaterThan(41);
await Expect.That(Act).Throws<CustomException>().Which.HasMessage("my exception");
```

Awaiting an expectation returns the value it verified, e.g. the single item of a collection, so you can use it
afterwards:

```csharp
IEnumerable<int> values = [42];

int single = await Expect.That(values).HasSingle();
```

## `null` subjects

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

## Waiting and cancellation

Some expectations wait for something to happen, e.g. for a callback, an event or a condition that becomes true. See
[cancellation](./advanced/03-cancellation.md) for how long they wait by default and how to limit the time an
expectation may take.
