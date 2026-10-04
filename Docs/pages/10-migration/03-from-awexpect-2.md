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

aweXpect v3 is the first release that can be published with trimming and Native AOT. Along the way it settles two
things that had grown inconsistent over the v2 releases: how a `null` subject is treated, and how the same comparison
is spelled across different result types. Most breaking changes are renames that the compiler points out; the
behavioural changes are summarized below so that you know what to look for in your test suite.

This page gives the overall picture. The complete list of changes, pull request by pull request, is in the
[GitHub releases](https://github.com/Testably/aweXpect/releases).

## Trimming and Native AOT

Both `aweXpect` and `aweXpect.Core` are trimmable and AOT compatible for `net8.0` and later. Equivalency, the
rendering of objects in failure messages and event recording no longer rely on reflection: a source generator
registers the members and events they need at compile time. Reflection stays available as a fallback, except in a
project that enables trimming or Native AOT (`PublishTrimmed` or `PublishAot`), also when it runs under the JIT: there,
a type without a registration fails with an error that tells you to add `[assembly: GenerateMetadata(typeof(MyType))]`.

- **Test projects** that use the built-in expectations need no changes.
- **Extension authors** replace `IAweXpectInitializer` with a `[ModuleInitializer]` and register a hand-written
  `ITestFrameworkAdapter` explicitly. See [Initialization](../11-extending/05-initialization.md).
- **Equivalency options** that inspected a `MemberInfo` move to `IgnoringFields` and `IgnoringProperties`.

More details are in the [Equivalency](../03-how-it-works/08-native-aot.md#equivalency) and
[Events](../03-how-it-works/08-native-aot.md#events) sections of Native AOT and trimming.

## Null subjects

v3 follows one rule: an expectation that **inspects** the subject fails for a `null` subject, in its negated form as
well, because there is nothing to inspect. An expectation that **compares** the subject against a value you supply
treats `null` as an ordinary value, so `IsEqualTo(null)` and `IsSameAs(null)` succeed for a `null` subject.

As a result, many negated expectations such as `IsNotEmpty()`, `DoesNotContain(…)` or `IsNot<T>()` now fail for a
`null` subject where they used to pass. In return, the analyzer knows every expectation that a `null` subject cannot
satisfy, so you may be able to remove `!` operators after such an expectation.

Awaiting such an expectation hands out the subject as not nullable, e.g.
`int value = await Expect.That(nullableInt).IsGreaterThan(0);`. For this, the ordering and range expectations on a
nullable number (`IsGreaterThan`, `IsGreaterThanOrEqualTo`, `IsLessThan`, `IsLessThanOrEqualTo`, `IsBetween` and their
negations) return a `NumberToleranceResult<TNumber, IThat<TNumber?>>` instead of a
`NullableNumberToleranceResult<TNumber, IThat<TNumber?>>`, so code that spells out this result type has to be adapted.
In turn, a comparison that a `null` subject can satisfy hands the subject out as nullable, so
`IEnumerable<int> values = await Expect.That(items).IsNotEqualTo(other);` now warns that `values` can be `null`.

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
- a negative duration in the `ExecutesIn()` family, and in `Throws().Within(…)`, which already rejected one before,
- a negative or `NaN` tolerance in `IsEqualTo(…).Within(…)` on a collection, and a negative
  `DefaultTimeComparisonTolerance`.

The negated forms are worth a second look: a reversed range such as `IsNotBetween(3).And(1)` used to pass for every
subject, and a negative count such as `HasCount().NotEqualTo(-1)` used to hold for every collection. Both now throw.
A reversed range on a `TimeOnly` is unaffected, because there it describes a range across midnight.

`Between(…).And(…)` on an occurrence count, as in `Contains("a").Between(4).And(3)`, already threw for a reversed
range; it now throws an `ArgumentOutOfRangeException` instead of a plain `ArgumentException`.

## Exceptions from your code

An exception thrown by your code while an expectation is evaluated fails the expectation, and its negation alike, with
the exception as inner exception. This covers predicates such as in `All().Satisfy(…)`, `Contains(…)` or `HasItem(…)`,
member selectors, comparers, a throwing `Equals`, a throwing property getter in equivalency and the enumeration of a
collection subject. v2 threw some of these exceptions directly or wrapped them in an
`InvalidOperationException("Error evaluating … constraint with value …")`, so a test that expected such an exception
now gets a failed expectation instead. Exceptions of aweXpect itself, such as the argument validation above, are
still thrown.

## Conflicting string options

`IgnoringCase()` and `Using(comparer)` could be combined although only one of them ever took effect, and a comparer
set together with `AsRegex()` or `AsWildcard()` was ignored altogether, in both cases without a trace in the failure
message. Such a combination now throws an `InvalidOperationException` at the call that creates it, in either order.
Pass a case-insensitive comparer instead of combining it with `IgnoringCase()`, and express the casing of a pattern
with `IgnoringCase()` alone.

## Options in any order, each option once

Options can be chained in any order, e.g. `IsEqualTo("A*").IgnoringCase().AsWildcard()`. Every option can be specified
only once, and options that would replace each other, e.g. `AtLeast(2).AtMost(5)` or two match types, throw an
`InvalidOperationException` at the call instead of silently keeping the later one; write a range as
`Between(2).And(5)`. `HasItem(…).AtIndex(1).FromEnd()` becomes `HasItem(…).AtIndexFromEnd(1)`.

The option methods are extension methods in the `aweXpect` namespace, and the result classes with a `TSelf` type
parameter, e.g. `CountResult<TType, TThat, TSelf>`, are gone. A result of your own derives from
`AndOrResult<TType, TThat, TSelf>` and implements `IOptionsProvider<TOptions>` for the options it offers, see
[Options](../11-extending/02-constraints-and-results.md#options).

## String patterns

A wildcard pattern has to match the complete subject. v2 anchored it to a single line, so
`Expect.That("xyz\nabc").IsEqualTo("abc").AsWildcard()` passed although the first line is not covered by the pattern.
A regex pattern is matched with the default options, like `Regex.IsMatch(subject, pattern)`, instead of with
`RegexOptions.Multiline`, so `^` and `$` anchor the complete value instead of any single line. Where you relied on the
line anchors, pass the option explicitly with `AsRegex(RegexOptions.Multiline)` or use the inline `(?m)`. Counting
occurrences with `Contains(…)` still finds a pattern anywhere in the subject. See
[Match types](../04-values/03-string.md#match-types).

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
| `ExecutesWithin(d)`                                    | `ExecutesIn().AtMost(d)`                                     |
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

`Values` on a dictionary subject is an extension property and needs C# 14. With an older language version, pass the
values themselves, as in `Expect.That(dictionary.Values).All().AreUnique()`.

`ExecutesIn().AtMost(d)` behaves like `ExecutesWithin(d)` did and reads `executes in at most …` in the failure message.
`DoesNotExecuteWithin` read like the negation of `ExecutesWithin`, but both required the delegate to complete without
throwing, so neither was the complement of the other. `ExecutesIn().AtLeast(d)` says the same thing without that trap;
it includes a duration of exactly `d`, where `DoesNotExecuteWithin(d)` required strictly more. If you measured a
delegate that is expected to throw, add
[`AllowingExceptions()`](../06-behaviour/01-delegates.md#allowing-exceptions) to let the duration decide alone.

The element type checks `Are<T>()`, `Are(type)`, `AreExactly<T>()` and `AreExactly(type)` no longer offer `Using(…)`
and `Equivalent(…)`. A type check does not compare values, so neither option ever had an effect; remove such a call.
`ComplyWith(…)` on the elements of an `IEnumerable` or `IAsyncEnumerable` drops the same two options: the nested
expectations bring their own, so an option set on the outer result never reached them.
`IsExactly(type)` and `IsNotExactly(type)` are generic over the subject like `Is(type)` and `IsNot(type)`, so the
expectation chain and the awaited result keep the subject type instead of widening it to `object`.
`ContainsKeys(…).WhoseValues` applied the `All()` quantifier implicitly, which left no way to check the values as a
whole. It is now an ordinary collection subject, so `IsEqualTo(…)`, `Contains(…)`, `HasCount(…)` and the other
quantifiers such as `None()` are available as well.
The `With…` expectations after `Throws()` accept the same arguments as their `Has…` twins: `WithMessage(expected)`
takes a `string?` and `WithHResult(expected)` an `int?`, so `WithMessage(null)` requires the message to be `null` like
`HasMessage(null)`. `WithRecursiveInnerExceptions(…)` returns the thrown exception as non-nullable like the other
`With…` expectations.

## Failure messages

Failure messages were reviewed as a whole. Options with several spellings now render the same way everywhere (for
example a tolerance always reads `± x`), negated expectations name what they found instead of `but it did`, and many
grammar slips were fixed. A `Whose(…)` nested inside a collection expectation such as `All().ComplyWith(…)` or
`HasItemThat(…)` now names the member it inspects, instead of reporting only the expectation on it. The same
expectations also keep the expectation they continue from, so `HasSingle().Which.Whose(…)` reads
`has a single item whose … for all items` instead of starting at the dangling connector. Where a connector already
introduced the subject, as in `has an item that …` or `contains key 2 whose value …`, the member no longer starts a
second relative clause but reads `has Value that is equal to 5`, and it agrees with a plural connector
(`whose values all have Length that …`). `IgnoringCase()` is now also named in the expectation when the value is matched
as a regex or wildcard pattern, where it took effect but stayed invisible. A quantified collection expectation refers
back to its own verb, so `All().ComplyWith(it => it.StartsWith("a"))` reports `but only 1 of 3 did` instead of
`but only 1 of 3 were`, a negated quantifier names its complement (`for no items` instead of
`for not at least one item`), and `HasCount` names its subject (`but it had only 3 items` instead of
`but found only 3`). Every expectation text also mirrors the method it comes from, so `IsEqualTo` reads
`is equal to …` for `Guid`, `enum` and `char?` as well, `IsOneOf` on an object reads `is one of […]` instead of the
ambiguous `is equal to one of […]`, and `HasItem` and `Contains` name how they match (`has an item matching _ => true`,
`has an item equal to 3`, `contains an item equal to 3`). Tests that assert on the exact text of a failure message may
need an update.

The contexts below a failure message (e.g. `Collection:`, `Expected:` or `Actual:`) belong to the part of the
expectation they describe: they appear exactly when that part explains the failure, also after a negation, and no
longer for a part that succeeded, e.g. the other operand of `And` or `Equivalency options` of a succeeding
`IsEquivalentTo`. A context of a member is labelled with it, e.g. `Collection (Items):` or `Actual (S1):`, so that two
members with the same kind of context each show their own, and a failed expectation on the items of a collection
(`All().ComplyWith(…)`) also shows the contexts of the item that explains the failure, e.g. `Expected (item [1]):`.
Contexts with the same title and member are shown once, or numbered when their content differs.

An object that contains itself through a collection, such as a tree node that lists itself among its children, and a
collection that contains itself are rendered as `{ *recursive* }` or `[ *recursive* ]` where they repeat, instead of
overflowing the stack and aborting the test run. An instance that appears twice without containing itself, e.g. in two
members of the same object, is written out both times instead of being marked as recursive the second time. A formatter
registered with `ValueFormatter.Register` is asked for every value, also where the declared type picks a dedicated
overload, so a formatter for `DateTime` now applies to the subject and the expected value of `IsEqualTo` as well, not
only to the items of a collection or the members of an object. Nested objects, collections and tuples are written up to
20 levels deep and up to 1000 of them per value, so a long chain is cut off with `{ … }` instead of overflowing the
stack. A collection inside an object is indented below its member, a combination of `[Flags]` values reads `A | B`
instead of `A, B`, which looked like two collection items, and combining marks in strings that are not normalized, like
the accent of a decomposed `é`, and unpaired surrogates are escaped as `\uXXXX`.

## Negative event expectations

`Within(…)` used to be ignored on event expectations with an upper bound, such as `DidNotTrigger(…)` or
`Triggered(…).Never()`, so an event raised later inside the window went unseen. In v3 such an expectation waits out
the full timeout.

A count after `DidNotTriggerPropertyChanged()` or `DidNotTriggerPropertyChangedFor(…)` used to replace the implicit
"never" without negating anything, so `DidNotTriggerPropertyChanged().AtLeast(2.Times())` passed for three events. The
count is now negated and describes the unwanted occurrence: `AtLeast(2.Times())` expects fewer than two events and
`Once()` anything but exactly one. Without a count the expectation still means "never". See
[Events](../06-behaviour/03-events.md#triggering).

## Execution time as timeout

An execution time expectation used to await the delegate to completion, so a delegate that never returns hung the
test run instead of failing at the bound. The upper bound of `Throws().Within(d)`, `ExecutesIn().AtMost(d)`,
`ExecutesIn().Between(a).And(b)` and `ExecutesIn(x).Within(t)` is now applied as timeout,
so a delegate accepting a `CancellationToken` is cancelled once it elapsed and the expectation fails with
"did not finish within …". `ExecutesIn().AtLeast(d)` has no upper bound and stays untimed. The task of an asynchronous
delegate is abandoned at that point even if it ignores the token, and so is a `Task<T>` subject under `WithTimeout`
or `WithCancellation`; only a synchronous delegate still runs to completion. A timeout fails the
expectation even with `AllowingExceptions()`, and a canceled `WithCancellation` token leaves it inconclusive, because
both abort the execution instead of timing it. See
[Delegates](../06-behaviour/01-delegates.md#execution-time).

## `Task` and `ValueTask` subjects

`Expect.That` awaited a `Task<T>` and used its result as the subject, but a non-generic `Task` or `ValueTask` became
the subject itself, so `Expect.That(DoAsync()).IsNotNull()` passed without ever observing a failed operation. Both
now bind to a delegate subject that awaits the task, which makes `DoesNotThrow()`, `Throws<TException>()` and the
execution time expectations available. The analyzer rule `aweXpect0004` reports every expectation on the task object
as an error afterwards; where you really mean the object, name the type explicitly with `Expect.That<Task>(subject)`.
See [Tasks](../06-behaviour/02-tasks.md).

## `DateTime` kinds

A `DateTime` with `DateTimeKind.Utc` and one with `DateTimeKind.Local` describe different instants for the same
ticks. `IsEqualTo` already failed for such a pair; now the ordering expectations (`IsAfter`, `IsBefore`,
`IsOnOrAfter`, `IsOnOrBefore`, `IsBetween`) fail as well, in their negated form too, `IsOneOf` ignores an expected
value with the other kind, and `IsInAscendingOrder` / `IsInDescendingOrder` fail for a `DateTime` collection that
mixes both kinds unless you specify a comparer. Comparing a `DateTime` as a value honours the kind as well, so a
collection expectation such as `IsEqualTo` or `Contains`, and `IsEquivalentTo` for a `DateTime` member, no longer
match two values that differ only in their kind. `DateTimeKind.Unspecified` is compatible with both kinds. See
[DateTime / DateTimeOffset](../04-values/10-datetime-offset.md#kind).

## Contained collections

`Expect.That(values).IsContainedIn(expected)` without `InAnyOrder()` requires the items to appear in `expected` as an
uninterrupted run, like `Expect.That(expected).Contains(values)` already did. v2 accepted items with other items in
between, so `[1, 3]` was contained in `[1, 2, 3]`; append `IgnoringInterspersedItems()` to keep that meaning. See
[Superset](../05-collections/01-equality.md#superset).

## Dictionary subjects

`ContainsKey`, `ContainsKeys`, `ContainsValue`, `ContainsValues`, `Keys`, `Values` and their negated forms were
declared once for `IDictionary<TKey, TValue>` and once for `IReadOnlyDictionary<TKey, TValue>` in two different
classes, so a subject that implements both interfaces, such as `SortedDictionary<TKey, TValue>` or
`ImmutableDictionary<TKey, TValue>`, did not compile. They are now declared together on `ThatDictionary` and such a
subject resolves to the `IDictionary<TKey, TValue>` overload. No call needs an edit, but the class
`aweXpect.ThatReadOnlyDictionary` is gone, so name `aweXpect.ThatDictionary` where you called one of these
expectations as a static method.

## Equivalency

A type that is compared by its members ignores its own `Equals`. v2 asked `Equals` first and took `true` as success,
so a type whose `Equals` called more instances equal than its members do passed although its members differed. Such a
comparison now fails; to let `Equals` decide, compare the type
[by value](../04-values/13-equivalency.md#comparing-by-value-or-by-members).

`IsEquivalentTo` throws an `InvalidOperationException` when it finds no member to compare, unless all members were
excluded explicitly. Only public members are registered at compile time, so asking for internal members falls back to
reflection, which is unavailable under trimming.

`IncludeMembers.Private` is gone: protected and private members are implementation details and are never compared.
To compare a type whose state is private, compare it
[by value](../04-values/13-equivalency.md#comparing-by-value-or-by-members) so that its `Equals` decides.

The numbers `BigInteger`, `Complex`, `Half`, `NFloat`, `Int128` and `UInt128` are compared by value. Their members
could not tell two values apart (`3` and `5` share `IsZero`, `IsEven` and `Sign`), and a type without public members,
such as `Int128`, threw. A `StringBuilder` is compared by the text it contains, also against a `string`, instead of by
its `Capacity` and `Length`.

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
[Limiting the recursion depth](../04-values/13-equivalency.md#limiting-the-recursion-depth).

A default set with `Customize.aweXpect.Equivalency()` also applies to an expectation that passes an options callback
of its own. The options handed to such a callback dropped the included fields and properties, the comparison type and
every `For<T>` registration of that default, so a global customization silently had no effect on
`IsEquivalentTo(expected, o => …)`. A registration in the callback replaces one for the same type in the default.

`For<T>()` applies to a member whose runtime type derives from `T` as well, and the most derived registration wins.
It used to require the runtime type to match exactly, which no instance of an abstract type ever does, and which made
`For<Type>()` unreachable because the runtime type of a `Type` is the internal `RuntimeType`.

`For<T>()` returns a copy instead of changing the options it is called on, so a call inside `.Equivalent(o => …)` no
longer writes into the customized default and from there into every later check; use its return value. Its callback
is applied to the final options, so an option set after `For<T>()` (such as `IgnoringCollectionOrder()` or a later
`IgnoringMember`) now applies to `T` as well. When the subject and the expectation have different types, a registration
for the type of the expectation wins, because the compared members come from it. The public `CustomOptions` dictionary
is gone; `GetOptionsFor(type)` returns the options that apply to a type.

A type that implements the non-generic `IEqualityComparer` is compared by its members like any other type. When
either side at the top level implemented it, v2 let its `Equals(x, y)` decide the whole comparison and ignored every
option. To let a type decide with its own `Equals`, compare it
[by value](../04-values/13-equivalency.md#comparing-by-value-or-by-members); to check a member against a custom
criterion, use [`It.Is<T>()`](../04-values/13-equivalency.md#per-property-expectations-with-itist).

## Customization

A value set with `Customize.aweXpect` stays in the async flow that set it and the flows started from there. Once a
parent flow had set any customization, for example in an assembly-level setup, all tests shared one store, so a value
set in one test leaked into the tests running in parallel with it. As a consequence, a value set inside an awaited
`async` helper method is no longer visible to its caller after the `await`; set it in the calling method or in a
synchronous helper. Disposing the lifetime of a single value such as `MaximumStringLength` restores only that value,
also when lifetimes are disposed out of order, and disposing a lifetime a second time has no effect. See
[Configuration](../03-how-it-works/07-configuration.md#lifetimes-and-async-flows).

Each customization value is stored on its own. The whole-group `Get()` and `Update(…)` of `Formatting()`,
`Settings()`, `Equivalency()` and `Reflection()` are gone, together with the `ICustomizationValueUpdater<T>` interface
and the `FormattingCustomizationValue`, `SettingsCustomizationValue`, `EquivalencyCustomizationValue` and
`ReflectionCustomizationValue` records: an update skipped the validation of the values, and a value set in a test
hid global changes to the other values of its group. Set each value on its own instead, e.g. replace
`Settings().Update(s => s with { DefaultCheckInterval = … })` with `Settings().DefaultCheckInterval.Set(…)`, with one
`using` per value. A group of an extension stores each value under its own key, see
[customization values](../11-extending/04-customization-values.md#add-a-customization-group).

Whether a value set in an assembly-level setup reached the tests depended on the test framework and on whether the
setup was asynchronous. Set such defaults on the new `Customize.aweXpect.Global`, e.g.
`Customize.aweXpect.Global.Formatting().MaximumStringLength.Set(500)`, which applies them to all async flows; a value
set in a test still takes precedence. See [Global defaults](../03-how-it-works/07-configuration.md#global-defaults).

## Extensions and aweXpect.Core

Besides the initialization changes above, v3 renames several result and option types so that their names follow what
they do, names the receiver parameter of every expectation `subject`, and moves a few types into more fitting
namespaces. The new `[GuaranteesNotNull]` attribute marks an expectation that a `null` subject can never satisfy, also
in an extension, to [suppress nullability warnings](../11-extending/02-constraints-and-results.md#nullability-warnings)
after it, and the [null rule](../11-extending/02-constraints-and-results.md#null-subjects) that an extension has to follow is
documented.

`DidNotSignal()` returns a `DidNotSignalResult`. Its previous name `SignalTimeoutResult`, which only ever existed in
the v3 pre-releases, read like a timeout failure although it is the result of an absent signal.

`RepeatedCheckOptions.Interval` is a plain `TimeSpan`. The `ICheckInterval` interface, its implementation
`FixedCheckInterval` and the constant `RepeatedCheckOptions.DefaultInterval` are gone: nothing accepted a custom
interval, and the default comes from `Customize.aweXpect.Settings().DefaultCheckInterval`. `Satisfies(…)` and
`CompliesWith(…)` with `Within(…)` now shorten the last wait to the timeout like `Eventually()`, so they check a last
time at the timeout and never after it. As there, a `WithTimeout(…)` that is not shorter than `Within(…)` reports the
result of that last check instead of "did not finish within …"; the effective timeout, which includes
`TestCancellation.FromTimeout`, decides.

`IEvaluationContext` has a `Cancellation` property with the token, the effective timeout and whether the evaluation
timed out or was canceled by the caller; an own implementation returns `EvaluationCancellation.None` outside of an
evaluation. `RepeatedCheckOptions.CheckRepeatedly` takes the `IEvaluationContext` instead of the `ExpectationBuilder`
and the token, and returns the `Outcome`: `Undecided` for a cancellation, which the constraint reports as undecided
instead of letting an `OperationCanceledException` escape.

An extension that references only `aweXpect.Core` can now add expectations on collection items and repeated checks:
`EnumerableQuantifier`, `QuantifiedCollectionConstraint<TValue, TItem>`, `RepeatedCheckOptions`,
`RepeatedCheckResult<TType, TThat>`, `ObjectCountResult<…>` and `CollectionCountResult<TReturn>` moved from `aweXpect` to
`aweXpect.Core` and keep their namespaces, so only code compiled against an earlier pre-release has to be rebuilt.
The interfaces of the quantified elements are no longer nested in `ThatEnumerable` and `ThatAsyncEnumerable`:

| Before                                                            | Now                                             |
|-------------------------------------------------------------------|-------------------------------------------------|
| `ThatEnumerable.IElements<TItem>`                                 | `IEnumerableElements<TItem>`                    |
| `ThatEnumerable.IElements`                                        | `IEnumerableStringElements`                     |
| `ThatEnumerable.IElementsForEnumerable<TEnumerable>`              | `INonGenericEnumerableElements<TEnumerable>`    |
| `ThatEnumerable.IElementsForStructEnumerable<TEnumerable, TItem>` | `IStructEnumerableElements<TEnumerable, TItem>` |
| `ThatEnumerable.IElementsForStructEnumerable<TEnumerable>`        | `IStructEnumerableStringElements<TEnumerable>`  |
| `ThatAsyncEnumerable.IElements<TItem>`                            | `IAsyncEnumerableElements<TItem>`               |
| `ThatAsyncEnumerable.IElements`                                   | `IAsyncEnumerableStringElements`                |

An expectation on collection items extends the interface, e.g. `this IEnumerableElements<Track> elements`, and reads
its `Quantifier` and `Subject` without a cast.

`EnumerableQuantifier` can no longer be derived from outside aweXpect.Core, because its negated and nested texts rely on
members that are not public; use the built-in quantifiers. Its `AppendResult` takes the `it` of the expectation, so that
a result about the items themselves can name the subject that had them. `Exactly` takes an `int?`, and a `null`
count matches no collection.

`ExpectationBuilder` and `EquivalencyExpectationBuilder` can no longer be derived from, as they rely on members
that are not public.
`ThatBoolSubject`, `ThatDelegateThrows<TException>` and `It.IsEquivalent<T>` no longer have a public
`ExpectationBuilder` property; reach the builder through `IExpectThat<T>`, as for every other subject.
The constructor of `ThatDelegate.WithValue<T>` also takes the delegate, which `Eventually()` invokes on every attempt.

A constraint adds its contexts in the new `ConstraintResult.AppendContexts(ResultContextCollector)`, which is only
called while the failure message is created and only for the parts of the result that explain the failure. It replaces
`ExpectationBuilder.AddContext(…)`, `ExpectationBuilder.UpdateContexts(…)` and the `ResultContexts` list, which are
gone, as is the setter of `ResultContext.Title`. Move the context from `IsMetBy` into `AppendContexts` and keep the
values it needs in the constraint, see [contexts](../11-extending/03-message-conventions.md#contexts). A context that
was added when the expectation was built belongs to the constraint it describes. A result that wraps other results
adds their contexts with `contexts.Visit(…)`. As the builder is no longer needed for contexts,
`QuantifiedCollectionConstraint<TValue, TItem>` and `QuantifiedCollectionConstraintBase<TValue, TItem>` no longer take an
`ExpectationBuilder`, and `ManualExpectationBuilder<TValue>` is sealed and no longer takes an inner builder to forward
contexts to. `ExpectationBuilder.ForWhich(…)` takes a `contextMember` that labels the contexts of the member.

`Outcome` has the new value `FailureBothWays` for a result that fails the expectation and its negation alike, e.g.
for a `null` subject or when code of the caller threw. Code that checks for a failed expectation checks for
`Outcome.Failure` and `Outcome.FailureBothWays`. A result sets `Outcome.FailureBothWays` instead of overriding the
`Outcome`, which `ConstraintResult.WithValue<T>` seals, see
[failing both ways](../11-extending/02-constraints-and-results.md#failing-both-ways).
`WithNotNullValue<T>` and `WithEqualToValue<T>` now derive from `WithValue<T>`, so an extension compiled against an
earlier version has to be rebuilt.

`IAsyncConstraint<T>.IsMetBy` and `IAsyncContextConstraint<T>.IsMetBy` return a `ValueTask<ConstraintResult>` instead
of a `Task<ConstraintResult>`, so that a constraint that completes synchronously allocates no task: change the return
type of an own implementation, and return `new ValueTask<ConstraintResult>(result)` instead of `Task.FromResult(result)`.
`ManualExpectationBuilder<TValue>.IsMetBy` and `EquivalencyExpectationBuilder.IsMetBy` also return a
`ValueTask<ConstraintResult>`: await it only once, or call `AsTask()` to keep it.
Awaiting an expectation result uses a `ValueTaskAwaiter<T>`, which needs no change to `await`, but code compiled
against an earlier version has to be rebuilt.

The unused enum `aweXpect.Core.Helpers.MemberVisibilities` is gone. `aweXpect.Equivalency.IncludeMembers` selects the
members that an equivalency comparison includes.

A custom `IObjectMatchType` must implement the new `AppendContexts(ResultContextCollector)`, which adds the contexts
that explain a failed comparison, e.g. the equivalency options. Leave its body empty when the match type adds no
context.
`IObjectMatchType.AreConsideredEqual` only decides, so that the items of a collection are compared without writing a
failure text. A custom match type implements the new `AreConsideredEqualWithExplanation`, which returns an
`IObjectMatchResult`, and moves its `GetExtendedFailure` there. A caller of `ObjectEqualityOptions<T>.GetExtendedFailure`
compares with `AreConsideredEqualWithExplanation` instead and calls `GetExtendedFailure` on its result.

## New expectations

- **Dictionaries** navigate to their `Keys` and `Values` with the full collection vocabulary (needs C# 14).
- **Collections** gain a positional `DoesNotHaveItem(x).AtIndex(n)`, and uniqueness becomes a quantifier:
  `AtLeast(1).AreNotUnique()`.
- **Events** gain a positional `DidNotTrigger(eventName)`.
- **Objects** gain `DoesNotSatisfy(…).Within(…)`.
- **Delegates** gain more message and `HResult` continuations, and `Throws(…).WithoutInner()` as the twin of
  `DoesNotHaveInner()`.
- **Version** gains comparisons and its components. See [Version](../04-values/07-version.md).
- **Guid** gains `IsOneOf`, and **Char** gains character class checks such as `IsADigit` and `IsUpperCased`.

## Analyzer

- `aweXpect0001` follows the expectation instead of scanning the enclosing statement, so it no longer breaks the
  build for an expectation that is assigned to a local, returned from a member or evaluated with
  `GetAwaiter().GetResult()`. It now also covers `Expect.ThatAll` and `Expect.ThatAny`, and no longer accepts an
  expectation because another branch of the same statement verifies one.
- `aweXpect0003` flags a `Has…` or `DoesNotHave…` exception expectation directly after `Throws`, and offers a code
  fix. See [Delegates](../06-behaviour/01-delegates.md#with-after-throws-has-on-the-exception).
- `aweXpect0004` reports an expectation for an ordinary subject that is applied to a delegate subject, where it
  checked the delegate instead of what it does, and offers a code fix. Because it is an error, an expectation such as
  `Expect.That(() => sut.Count()).IsEqualTo(1)` that used to compile now has to be written as
  `Expect.That(() => sut.Count()).DoesNotThrow().WhoseResult.IsEqualTo(1)`. See
  [Delegates](../06-behaviour/01-delegates.md#no-exception).
- `aweXpect0005` warns about an expectation inside an `async` lambda that is converted to a void-returning delegate,
  such as `list.ForEach(async x => await Expect.That(x).IsTrue())`, because the lambda returns before the expectation
  is evaluated and its failure is thrown after the test has completed.
- `aweXpect2001` warns when a type named in `[assembly: GenerateMetadata]` yields no registration.
- The nullability suppressor reads `[GuaranteesNotNull]`, so it suppresses the warning after far more expectations.
