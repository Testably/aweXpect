---
title: What's new in v3
sidebar_position: 3
sidebar_label: From aweXpect 2.x
---

# What's new in v3

:::warning[Pre-release]

aweXpect v3 is currently available as a pre-release only. The API described on this page can still change before the
final v3.0.0 release.

:::

aweXpect v3 makes the library trimmable and Native AOT compatible, applies one rule to `null` subjects and spells the
same comparison the same way on every result type. This page covers what you need to know to upgrade. The complete
list of changes, pull request by pull request, is in the [GitHub releases](https://github.com/Testably/aweXpect/releases).

To upgrade:

1. Update the packages and fix the compile errors. Most of them are [renames](#renamed-expectations).
2. Run your tests. Some expectations are stricter now and fail where they used to pass; the
   [behaviour changes](#behaviour-changes) explain why and how to adapt.
3. If you wrote your own expectations, also read [Extensions](#extensions).

Failure messages were reviewed as a whole, so tests that assert on the exact text of a failure message may need an
update.

## Renamed expectations

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
| `ExecutesWithin(d)`                                    | `ExecutesIn().AtMost(d)`                                     |
| `DoesNotExecuteWithin(d)`                              | `ExecutesIn().AtLeast(d)`                                    |
| `DoesNotThrow().AndWhoseResult`                        | `DoesNotThrow().WhoseResult`                                 |
| `AreAllUnique()` on a collection                       | `All().AreUnique()`                                          |
| `AreAllUnique()` on a dictionary                       | `Values.All().AreUnique()` (needs C# 14)                     |
| `ContainsKeys(…).WhoseValues.ComplyWith(…)`            | `ContainsKeys(…).WhoseValues.All().ComplyWith(…)`            |
| `HasItem(…).AtIndex(n).FromEnd()`                      | `HasItem(…).AtIndexFromEnd(n)`                               |
| `Contains(…).AsPrefix()` / `AsSuffix()` on a string    | `StartsWith(…)` / `EndsWith(…)`                              |
| `HasMessageContaining(…)` / `WithMessageContaining(…)` | `HasMessage().Containing(…)` / `WithMessage().Containing(…)` |
| `DoesNotHaveMessage(…)` / `WithoutMessage(…)`          | `HasMessage().NotEqualTo(…)` / `WithMessage().NotEqualTo(…)` |
| `HasParamNameContaining(…)` and the other variants     | `HasParamName().Containing(…)` and so on                     |
| `HasInnerException()` / `WithInnerException()`         | `HasInner()` / `WithInner()`                                 |
| `Contains(…).Exactly()` (the parameterless match type) | removed, it restated the default                             |

The rule behind the renames: a continuation on a **value** compares (`EqualTo`, `GreaterThan`, `Between`, …) and a
continuation on an **occurrence count** counts (`Exactly`, `AtLeast`, `AtMost`, `Never`, …).

`HasMessage().Containing(x)` is a literal substring match; use `HasMessage("*x*").AsWildcard()` for a wildcard. With a
language version below C# 14, check the values of a dictionary as `Expect.That(dictionary.Values).All().AreUnique()`.

## Behaviour changes

### `null` subjects

An expectation that **inspects** the subject fails for a `null` subject, in its negated form as well, because there is
nothing to inspect. An expectation that **compares** the subject against a value you supply treats `null` as an
ordinary value, so `IsEqualTo(null)` succeeds for a `null` subject. As a result, negated expectations such as
`IsNotEmpty()`, `DoesNotContain(…)` or `IsNot<T>()` now fail for a `null` subject where they used to pass. See
[Null subjects](../03-how-it-works/04-null-subjects.md).

In return, awaiting an expectation that a `null` subject cannot satisfy hands out the subject as not nullable, e.g.
`int value = await Expect.That(nullableInt).IsGreaterThan(0);`, and the analyzer suppresses nullability warnings
after it, so you may be able to remove `!` operators.

### Invalid arguments throw

Arguments that made an expectation meaningless now throw when the expectation is built: an empty or `null` value to
search for (`Contains("")`, `ContainsKeys()`), a reversed range (`IsNotBetween(3).And(1)` used to pass for every
subject), a negative count, duration or tolerance.

### Options

Options can be chained in any order, e.g. `IsEqualTo("A*").IgnoringCase().AsWildcard()`, but each option only once.
Options that would replace each other throw an `InvalidOperationException` instead of silently keeping one of them,
e.g. `AtLeast(2).AtMost(5)` (write `Between(2).And(5)`), two match types, or `IgnoringCase()` together with
`Using(comparer)` (pass a case-insensitive comparer instead).

### Exceptions from your code

An exception thrown by your code while an expectation is evaluated, e.g. in a predicate, a member selector, a comparer
or a property getter, fails the expectation (and its negation) with the exception as inner exception. In v2 some of
these exceptions escaped the expectation, so a test that expected such an exception now gets a failed expectation.

### String patterns

A wildcard pattern has to match the complete subject; v2 accepted a match of a single line. A regex pattern is matched
like `Regex.IsMatch(subject, pattern)` without `RegexOptions.Multiline`, so `^` and `$` anchor the complete value.
Where you relied on line anchors, use `AsRegex(RegexOptions.Multiline)` or the inline `(?m)`. See
[Match types](../04-values/03-string.md#match-types).

### `Task` and `ValueTask` subjects

A non-generic `Task` or `ValueTask` used to become the subject itself, so `Expect.That(DoAsync()).IsNotNull()` passed
without observing a failed operation. Now it is awaited like a delegate, with `DoesNotThrow()`, `Throws<TException>()`
and the execution time expectations. Expectations on the task object itself are reported by the analyzer; name the
type explicitly with `Expect.That<Task>(subject)` where you really mean the object. See
[Tasks](../06-behaviour/02-tasks.md).

### Execution time is a timeout

The upper bound of `Throws().Within(…)`, `ExecutesIn().AtMost(…)`, `ExecutesIn().Between(…)` and
`ExecutesIn(…).Within(…)` is applied as a timeout: a delegate that never returns fails the expectation instead of
hanging the test run, and its `CancellationToken` is cancelled. See
[Execution time](../06-behaviour/01-delegates.md#execution-time).

### Events

`Within(…)` on an expectation that limits the number of events, such as `DidNotTrigger(…)` or `Triggered(…).Never()`,
was ignored; it now waits out the full time to catch a late event. A count after `DidNotTriggerPropertyChanged()` is
negated, so `DidNotTriggerPropertyChanged().AtLeast(2.Times())` expects fewer than two events. See
[Events](../06-behaviour/03-events.md#triggering).

### `DateTime` kinds

A `DateTime` with `DateTimeKind.Utc` and one with `DateTimeKind.Local` are no longer considered equal or ordered
against each other: comparing them fails in `IsEqualTo`, `IsAfter`, `IsBefore`, `IsBetween`, the collection
expectations and `IsEquivalentTo`. `DateTimeKind.Unspecified` is compatible with both. See
[Kind](../04-values/10-datetime-offset.md#kind).

### `IsContainedIn`

`IsContainedIn(expected)` without `InAnyOrder()` requires the items to appear in `expected` as an uninterrupted run,
like `Contains` does. Append `IgnoringInterspersedItems()` to allow other items in between. See
[Superset](../05-collections/01-equality.md#superset).

### Equivalency

`IsEquivalentTo` became stricter and more predictable. See [Equivalency](../04-values/13-equivalency.md).

- A type that is compared by its members ignores its own `Equals`. To let `Equals` decide, compare the type
  [by value](../04-values/13-equivalency.md#comparing-by-value-or-by-members).
- Sets are compared regardless of order and dictionaries by key, also for the generic interfaces such as `ISet<T>` and
  `IReadOnlyDictionary<TKey, TValue>`.
- A collection type that declares members of its own, e.g. a `PagedResult<T>` with a `TotalCount`, compares these
  members in addition to its items.
- `IgnoringCollectionOrder()` no longer requires the items to be comparable.
- Only public members are compared; `IncludeMembers.Private` is gone.
- `For<T>()` also applies to types derived from `T`, and returns a copy of the options instead of changing them, so
  use its return value.
- A default set with `Customize.aweXpect.Equivalency()` also applies when an expectation passes its own options.

### Customization

A value set with `Customize.aweXpect` stays in the async flow that set it, so it no longer leaks into tests running in
parallel. It is also no longer visible to the caller of an `async` helper method that set it. Set defaults for all
tests on `Customize.aweXpect.Global`. Each value is set on its own: the group-wide `Get()` and `Update(…)` are gone, so
replace e.g. `Settings().Update(s => s with { DefaultCheckInterval = … })` with
`Settings().DefaultCheckInterval.Set(…)`. See [Configuration](../03-how-it-works/07-configuration.md).

## Trimming and Native AOT

`aweXpect` and `aweXpect.Core` are trimmable and AOT compatible for `net8.0` and later. Test projects need no changes:
a source generator registers the members and events that equivalency, failure messages and event recording need.
In a project with `PublishTrimmed` or `PublishAot`, a type without a registration fails with an error that tells you
to add `[assembly: GenerateMetadata(typeof(MyType))]`. See [Native AOT](../03-how-it-works/08-native-aot.md).

## New expectations

- **Dictionaries** navigate to their `Keys` and `Values` with the full collection vocabulary (needs C# 14).
- **Collections** gain a positional `DoesNotHaveItem(x).AtIndex(n)`, and uniqueness becomes a quantifier:
  `AtLeast(1).AreNotUnique()`.
- **Events** gain a positional `DidNotTrigger(eventName)`.
- **Objects** gain `DoesNotSatisfy(…).Within(…)`.
- **Delegates** gain more message and `HResult` continuations, and `Throws(…).WithoutInner()`.
- **Version** gains comparisons and its components. See [Version](../04-values/07-version.md).
- **Guid** gains `IsOneOf`, and **Char** gains character class checks such as `IsADigit` and `IsUpperCased`.

## Analyzers

- `aweXpect0001` follows the expectation itself, so it no longer reports an expectation that is assigned to a local or
  returned, and it now covers `Expect.ThatAll` and `Expect.ThatAny`.
- New rules flag an exception expectation that should use `With…` after `Throws` (`aweXpect0003`), an expectation
  for a value that is applied to a delegate (`aweXpect0004`, an error: write
  `Expect.That(() => sut.Count()).DoesNotThrow().WhoseResult.IsEqualTo(1)`), and an expectation inside an `async`
  lambda that is converted to a void-returning delegate (`aweXpect0005`).

See [Analyzers](../08-analyzers.md).

## Extensions

If you wrote your own expectations on `aweXpect.Core`, the compiler points out most of the changes. The important ones:

- Replace `IAweXpectInitializer` with a `[ModuleInitializer]` and register a test framework adapter explicitly. See
  [Initialization](../11-extending/08-initialization.md).
- Options are extension methods in the `aweXpect` namespace, and the result classes with a `TSelf` type parameter are
  gone. A result of your own derives from `AndOrResult<TType, TThat, TSelf>` and implements `IOptionsProvider<TOptions>`.
  See [Options](../11-extending/03-options-and-match-types.md#options).
- A constraint adds its contexts in `ConstraintResult.AppendContexts(ResultContextCollector)`, which replaces
  `ExpectationBuilder.AddContext(…)` and `UpdateContexts(…)`. See
  [Contexts](../11-extending/06-message-conventions.md#contexts).
- `IAsyncConstraint<T>.IsMetBy` returns a `ValueTask<ConstraintResult>` instead of a `Task<ConstraintResult>`.
- A result that fails the expectation and its negation alike sets the new `Outcome.FailureBothWays`. See
  [Failing both ways](../11-extending/02-constraints-and-results.md#failing-both-ways).
- Mark an expectation that a `null` subject can never satisfy with `[GuaranteesNotNull]`, and follow the
  [null rule](../11-extending/02-constraints-and-results.md#null-subjects).
- The element interfaces nested in `ThatEnumerable` and `ThatAsyncEnumerable` moved to the top level, e.g.
  `ThatEnumerable.IElements<TItem>` is now `IEnumerableElements<TItem>`.
- A custom `IObjectMatchType` implements `AppendContexts(…)` and `AreConsideredEqualWithExplanation(…)`, which takes
  over `GetExtendedFailure`.
- A customization group stores each value on its own. See
  [Customization values](../11-extending/07-customization-values.md#add-a-customization-group).
