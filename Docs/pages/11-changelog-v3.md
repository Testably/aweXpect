---
title: What's new in v3
sidebar_position: 10
---

# What's new in v3

:::warning[Pre-release]

aweXpect v3 is currently available as a pre-release only. The API described on this page can still change before the
final v3.0.0 release.

:::

aweXpect v3 is the first release that can be published with trimming and Native AOT. Along the way it settles two
things that had grown inconsistent over the v2 releases: how a `null` subject is treated, and how the same comparison
is spelled across different result types. Most breaking changes are renames that the compiler points out; the
behavioural changes are summarized below so that you know what to look for in your test suite.

This page gives the overall picture. The complete list of changes, pull request by pull request, is in the
[GitHub releases](https://github.com/Testably/aweXpect/releases).

## Trimming and Native AOT

Both `aweXpect` and `aweXpect.Core` are trimmable and AOT compatible for `net8.0` and later. Equivalency, the
rendering of objects in failure messages and event recording no longer rely on reflection: a source generator
registers the members and events they need at compile time. Reflection stays available as a fallback under the JIT;
when you publish with trimming or Native AOT, a type without a registration fails with an error that tells you to add
`[assembly: GenerateMetadata(typeof(MyType))]`.

- **Test projects** that use the built-in expectations need no changes.
- **Extension authors** replace `IAweXpectInitializer` with a `[ModuleInitializer]` and register a hand-written
  `ITestFrameworkAdapter` explicitly. See [Write your own extension](/docs/expectations/write-extension#initialization).
- **Equivalency options** that inspected a `MemberInfo` move to `IgnoringFields` and `IgnoringProperties`.

More details are on the [Equivalency](/docs/expectations/equivalency#trimming-and-native-aot) and
[Events](/docs/expectations/events#trimming-and-native-aot) pages.

## Null subjects

v3 follows one rule: an expectation that **inspects** the subject fails for a `null` subject, in its negated form as
well, because there is nothing to inspect. An expectation that **compares** the subject against a value you supply
treats `null` as an ordinary value, so `IsEqualTo(null)` and `IsSameAs(null)` succeed for a `null` subject.

As a result, many negated expectations such as `IsNotEmpty()`, `DoesNotContain(…)` or `IsNot<T>()` now fail for a
`null` subject where they used to pass. In return, the analyzer knows every expectation that a `null` subject cannot
satisfy, so you may be able to remove `!` operators after such an expectation.

## Argument validation

An empty or `null` value to search for, such as `Contains("")` or `ContainsKeys()` without arguments, made an
expectation that could never fail. Such calls now throw at the call site: `ArgumentNullException` for `null`,
`ArgumentException` for an empty value.

Ranges, counts and tolerances are validated the same way, when the expectation is built rather than when it is
evaluated, and every one of them throws an `ArgumentOutOfRangeException`:

- a maximum below the minimum in `Between(…).And(…)` on a collection quantifier, on `HasCount()`, on `HasLength()`
  and on the other scalar `Has…()` continuations, in `ExecutesIn().Between(…).And(…)` and in `Version.IsBetween(…)`
  and `IsNotBetween(…)`,
- a negative count in `AtLeast`, `AtMost`, `Exactly`, `LessThan`, `MoreThan` and `Between` on a collection, and in
  `HasCount(…)`,
- a negative duration in `ExecutesWithin(…)` and in the `ExecutesIn()` family, of which only `Throws().Within(…)`
  used to reject one,
- a negative or `NaN` tolerance in `IsEqualTo(…).Within(…)` on a collection, and a negative
  `DefaultTimeComparisonTolerance`.

The negated forms are worth a second look: a reversed range such as `IsNotBetween(3).And(1)` used to pass for every
subject, and a negative count such as `HasCount().NotEqualTo(-1)` used to hold for every collection. Both now throw.
A reversed range on a `TimeOnly` is unaffected, because there it describes a range across midnight.

`Between(…).And(…)` on an occurrence count, as in `Contains("a").Between(4).And(3)`, already threw for a reversed
range; it now throws an `ArgumentOutOfRangeException` instead of a plain `ArgumentException`.

## Conflicting string options

`IgnoringCase()` and `Using(comparer)` could be combined although only one of them ever took effect, and a comparer
set together with `AsRegex()` or `AsWildcard()` was ignored altogether, in both cases without a trace in the failure
message. Such a combination now throws an `InvalidOperationException` at the call that creates it, in either order.
Pass a case-insensitive comparer instead of combining it with `IgnoringCase()`, and express the casing of a pattern
with `IgnoringCase()` alone.

## Consistent vocabulary

A continuation on a **value** compares (`EqualTo`, `GreaterThan`, `LessThanOrEqualTo`, `Between`, …) and a
continuation on an **occurrence count** counts (`Exactly`, `AtLeast`, `AtMost`, `Never`, …). Every scalar `Has…`
expectation offers both the shorthand `HasLength(9)` and the explicit form `HasLength().EqualTo(9)`.

None of the renames has an `[Obsolete]` forwarder; each is a compile error that is fixed once:

| v2                                                     | v3                                                           |
|--------------------------------------------------------|--------------------------------------------------------------|
| `ThrowsException()`                                    | `Throws()`                                                   |
| `For(x => x.Member, m => m.IsEqualTo(…))`              | `Whose(x => x.Member, m => m.IsEqualTo(…))`                  |
| `HasCount().MoreThan(n)`                               | `HasCount().GreaterThan(n)`                                  |
| `HasCount().AtLeast(n)`                                | `HasCount().GreaterThanOrEqualTo(n)`                         |
| `HasCount().AtMost(n)`                                 | `HasCount().LessThanOrEqualTo(n)`                            |
| `DoesNotHaveCount(n)`                                  | `HasCount().NotEqualTo(n)`                                   |
| `DoesNotHaveValue(n)` on an enum                       | `HasValue().NotEqualTo(n)`                                   |
| `ExecutesIn().Approximately(expected, tolerance)`      | `ExecutesIn(expected).Within(tolerance)`                     |
| `DoesNotExecuteWithin(d)`                              | `ExecutesIn().AtLeast(d)`                                    |
| `DoesNotThrow().AndWhoseResult`                        | `DoesNotThrow().WhoseResult`                                 |
| `AreAllUnique()` on a collection                       | `All().AreUnique()`                                          |
| `AreAllUnique()` on a dictionary                       | `Values.All().AreUnique()`                                   |
| `ContainsKeys(…).WhoseValues.ComplyWith(…)`            | `ContainsKeys(…).WhoseValues.All().ComplyWith(…)`            |
| `HasMessageContaining(…)` / `WithMessageContaining(…)` | `HasMessage().Containing(…)` / `WithMessage().Containing(…)` |
| `DoesNotHaveMessage(…)` / `WithoutMessage(…)`          | `HasMessage().NotEqualTo(…)` / `WithMessage().NotEqualTo(…)` |
| `HasParamNameContaining(…)` and the other variants     | `HasParamName().Containing(…)` and so on                     |
| `HasInnerException()` / `WithInnerException()`         | `HasInner()` / `WithInner()`                                 |
| `Contains(…).Exactly()` (the parameterless match type) | removed, it restated the default                             |

`HasMessage().Containing(x)` is a literal substring match; use `HasMessage("*x*").AsWildcard()` for a wildcard.

`DoesNotExecuteWithin` read like the negation of `ExecutesWithin`, but both required the delegate to complete without
throwing, so neither was the complement of the other. `ExecutesIn().AtLeast(d)` says the same thing without that trap;
it includes a duration of exactly `d`, where `DoesNotExecuteWithin(d)` required strictly more. If you measured a
delegate that is expected to throw, add
[`AllowingExceptions()`](/docs/expectations/delegates#allowing-exceptions) to let the duration decide alone.

The element type checks `Are<T>()`, `Are(type)`, `AreExactly<T>()` and `AreExactly(type)` no longer offer `Using(…)`
and `Equivalent(…)`. A type check does not compare values, so neither option ever had an effect; remove such a call.
`ComplyWith(…)` on the elements of an `IEnumerable` or `IAsyncEnumerable` drops the same two options: the nested
expectations bring their own, so an option set on the outer result never reached them.
`IsExactly(type)` and `IsNotExactly(type)` are generic over the subject like `Is(type)` and `IsNot(type)`, so the
expectation chain and the awaited result keep the subject type instead of widening it to `object`.
`ContainsKeys(…).WhoseValues` applied the `All()` quantifier implicitly, which left no way to check the values as a
whole. It is now an ordinary collection subject, so `IsEqualTo(…)`, `Contains(…)`, `HasCount(…)` and the other
quantifiers such as `None()` are available as well.

## Failure messages

Failure messages were reviewed as a whole. Options with several spellings now render the same way everywhere (for
example a tolerance always reads `± x`), negated expectations name what they found instead of `but it did`, and many
grammar slips were fixed. A `Whose(…)` nested inside a collection expectation such as `All().ComplyWith(…)` or
`HasItemThat(…)` now names the member it inspects, instead of reporting only the expectation on it. The same
expectations also keep the expectation they continue from, so `HasSingle().Which.Whose(…)` reads
`has a single item whose … for all items` instead of starting at the dangling connector. Where a connector already
introduced the subject, as in `has item that …` or `contains key 2 whose value …`, the member no longer starts a
second relative clause but reads `has Value which is equal to 5`, and it agrees with a plural connector
(`whose values have Length which …`). `IgnoringCase()` is now also named in the expectation when the value is matched
as a regex or wildcard pattern, where it took effect but stayed invisible. A quantified collection expectation refers
back to its own verb, so `All().ComplyWith(it => it.StartsWith("a"))` reports `but only 1 of 3 did` instead of
`but only 1 of 3 were`, a negated quantifier names its complement (`for no items` instead of
`for not at least one item`), and `HasCount` names its subject (`but it had only 3 items` instead of
`but found only 3`). Every expectation text also mirrors the method it comes from, so `IsEqualTo` reads
`is equal to …` for `Guid`, `enum` and `char?` as well, `IsOneOf` on an object reads `is one of […]` instead of the
ambiguous `is equal to one of […]`, and `HasItem` and `Contains` name how they match (`has item matching _ => true`,
`has item equal to 3`, `contains an item equal to 3`). Tests that assert on the exact text of a failure message may
need an update.

## Timeouts on negative event expectations

`Within(…)` used to be ignored on event expectations with an upper bound, such as `DidNotTrigger(…)` or
`Triggered(…).Never()`, so an event raised later inside the window went unseen. In v3 such an expectation waits out
the full timeout. This is the one change that can make a passing test fail without touching its code.

## Execution time as timeout

An execution time expectation used to await the delegate to completion, so a delegate that never returns hung the
test run instead of failing at the bound. The upper bound of `ExecutesWithin(d)`, `Throws().Within(d)`,
`ExecutesIn().AtMost(d)`, `ExecutesIn().Between(a).And(b)` and `ExecutesIn(x).Within(t)` is now applied as timeout,
so a delegate accepting a `CancellationToken` is cancelled once it elapsed and the expectation fails with
"was canceled after …". `ExecutesIn().AtLeast(d)` has no upper bound and stays untimed. The task of an asynchronous
delegate is abandoned at that point even if it ignores the token, and so is a `Task<T>` subject under `WithTimeout`
or `WithCancellation`; only a synchronous delegate still runs to completion. A cancellation fails the
expectation even with `AllowingExceptions()`, because it aborts the execution instead of timing it. See
[Delegates](/docs/expectations/delegates#execution-time).

## `Task` and `ValueTask` subjects

`Expect.That` awaited a `Task<T>` and used its result as the subject, but a non-generic `Task` or `ValueTask` became
the subject itself, so `Expect.That(DoAsync()).IsNotNull()` passed without ever observing a failed operation. Both
now bind to a delegate subject that awaits the task, which makes `DoesNotThrow()`, `Throws<TException>()` and the
execution time expectations available. Every expectation on the task object is a compile error afterwards; where you
really mean the object, name the type explicitly with `Expect.That<Task>(subject)`. See
[Delegates](/docs/expectations/delegates).

## `DateTime` kinds

A `DateTime` with `DateTimeKind.Utc` and one with `DateTimeKind.Local` describe different instants for the same
ticks. `IsEqualTo` already failed for such a pair; now the ordering expectations (`IsAfter`, `IsBefore`,
`IsOnOrAfter`, `IsOnOrBefore`, `IsBetween`) fail as well, in their negated form too, `IsOneOf` ignores an expected
value with the other kind, and `IsInAscendingOrder` / `IsInDescendingOrder` fail for a `DateTime` collection that
mixes both kinds unless you specify a comparer. Comparing a `DateTime` as a value honours the kind as well, so a
collection expectation such as `IsEqualTo` or `Contains`, and `IsEquivalentTo` for a `DateTime` member, no longer
match two values that differ only in their kind. `DateTimeKind.Unspecified` is compatible with both kinds. See
[DateTime / DateTimeOffset](/docs/expectations/common-types/datetime-offset#kind).

## Dictionary subjects

`ContainsKey`, `ContainsKeys`, `ContainsValue`, `ContainsValues`, `Keys`, `Values` and their negated forms were
declared once for `IDictionary<TKey, TValue>` and once for `IReadOnlyDictionary<TKey, TValue>` in two different
classes, so a subject that implements both interfaces, such as `SortedDictionary<TKey, TValue>` or
`ImmutableDictionary<TKey, TValue>`, did not compile. They are now declared together on `ThatDictionary` and such a
subject resolves to the `IDictionary<TKey, TValue>` overload. No call needs an edit, but the class
`aweXpect.ThatReadOnlyDictionary` is gone, so name `aweXpect.ThatDictionary` where you called one of these
expectations as a static method.

## Equivalency

`IsEquivalentTo` fails when it finds no member to compare, unless all members were excluded explicitly. Only public
members are registered at compile time, so asking for internal or private members falls back to reflection, which is
unavailable under trimming.

`IgnoringCollectionOrder()` no longer requires the elements to be comparable, so it now works for the collections it
exists for, such as a collection of DTOs: each expected element is matched against an element that is equivalent to
it. Every element can be matched only once, so `[1, 1, 2]` is still not equivalent to `[1, 2, 2]`, and a failure
reports only the elements that were left over, each against the leftover element it differs from the least.

A **set** and a **dictionary** are no longer compared by the order in which they enumerate: a set is matched element
by element like a collection whose order is ignored, and a dictionary is compared by key. This applies to `ISet<T>`
and `IReadOnlySet<T>`, and to `IDictionary<TKey, TValue>` and `IReadOnlyDictionary<TKey, TValue>`. v2 only
recognized the non-generic `IDictionary`, so a `HashSet<T>` or a type that only implements
`IReadOnlyDictionary<TKey, TValue>` failed when both sides held the same content in a different order. Every other
collection still compares by position.
`IsEquivalentTo` stops at 100 nested objects on a single path and fails naming that path instead of recursing until
the stack overflows, so a graph that is legitimately deeper needs the limit raised: see
[Limiting the recursion depth](/docs/expectations/equivalency#limiting-the-recursion-depth).

A default set with `Customize.aweXpect.Equivalency()` also applies to an expectation that passes an options callback
of its own. The options handed to such a callback dropped the included fields and properties, the comparison type and
every `For<T>` registration of that default, so a global customization silently had no effect on
`IsEquivalentTo(expected, o => …)`. A registration in the callback replaces one for the same type in the default.

`For<T>()` applies to a member whose runtime type derives from `T` as well, and the most derived registration wins.
It used to require the runtime type to match exactly, which no instance of an abstract type ever does, and which made
`For<Type>()` unreachable because the runtime type of a `Type` is the internal `RuntimeType`.

## Extensions and aweXpect.Core

Besides the initialization changes above, v3 renames several result and option types so that their names follow what
they do, names the receiver parameter of every expectation `subject`, and moves a few types into more fitting
namespaces. The new `[GuaranteesNotNull]` attribute marks an expectation that a `null` subject can never satisfy, and
the [null rule](/docs/expectations/write-extension#constraints) that an extension has to follow is documented.

`DidNotSignal()` returns a `DidNotSignalResult`. Its previous name `SignalTimeoutResult`, which only ever existed in
the v3 pre-releases, read like a timeout failure although it is the result of an absent signal.

`EnumerableQuantifier.AppendResult` takes the `it` of the expectation, so that a result about the items themselves can
name the subject that had them. A custom quantifier has to add the parameter.

## New expectations

- **Dictionaries** navigate to their `Keys` and `Values` with the full collection vocabulary (needs C# 14).
- **Collections** gain a positional `DoesNotHaveItem(x).AtIndex(n)`, and uniqueness becomes a quantifier:
  `AtLeast(1).AreNotUnique()`.
- **Events** gain a positional `DidNotTrigger(eventName)`.
- **Delegates** gain `DoesNotSatisfy(…).Within(…)` and more message and `HResult` continuations.
- **Version** gains comparisons and its components. See [Version](/docs/expectations/common-types/version).
- **Guid** gains `IsOneOf`, and **Char** gains character class checks such as `IsADigit` and `IsUpperCased`.

## Analyzer

- `aweXpect0001` follows the expectation instead of scanning the enclosing statement, so it no longer breaks the
  build for an expectation that is assigned to a local, returned from a member or evaluated with
  `GetAwaiter().GetResult()`. It now also covers `Expect.ThatAll` and `Expect.ThatAny`, and no longer accepts an
  expectation because another branch of the same statement verifies one.
- `aweXpect0003` flags a `Has…` exception expectation directly after `Throws`, and offers a code fix. See
  [Delegates](/docs/expectations/delegates#with-after-throws-has-on-the-exception).
- `aweXpect0004` reports an expectation for an ordinary subject that is applied to a delegate subject, where it
  checked the delegate instead of what it does, and offers a code fix. Because it is an error, an expectation such as
  `Expect.That(() => sut.Count()).IsEqualTo(1)` that used to compile now has to be written as
  `Expect.That(() => sut.Count()).DoesNotThrow().WhoseResult.IsEqualTo(1)`. See
  [Delegates](/docs/expectations/delegates#not-throw).
- `aweXpect0005` warns about an expectation inside an `async` lambda that is converted to a void-returning delegate,
  such as `list.ForEach(async x => await Expect.That(x).IsTrue())`, because the lambda returns before the expectation
  is evaluated and its failure is thrown after the test has completed.
- `aweXpect2001` warns when a type named in `[assembly: GenerateMetadata]` yields no registration.
- The nullability suppressor reads `[GuaranteesNotNull]`, so it suppresses the warning after far more expectations.
