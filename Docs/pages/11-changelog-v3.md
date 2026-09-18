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
is spelled across different result types. Most of the breaking changes below are renames that the compiler will point
out; the behavioural changes are collected in their own sections so that you can check your test suite for them.

## Trimming and Native AOT

Both `aweXpect` and `aweXpect.Core` declare themselves trimmable and AOT compatible for `net8.0` and later. A test
project that publishes with `PublishTrimmed` or `PublishAot` no longer receives trim warnings from aweXpect, and the
smoke tests in CI run the expectations as a Native AOT binary.

Under the JIT, aweXpect reached three things through reflection: the members that an equivalency comparison walks,
the members that a failure message renders for an object, and the events that a recording subscribes to. Trimming
removes whatever is only reached reflectively, so all three would silently do less than they claim. v3 replaces
reflection with a registry that a source generator fills at compile time:

- **Equivalency** registers the public members of every type that reaches `IsEquivalentTo`, `IsNotEquivalentTo`,
  `AreEquivalentTo` or `Equivalent()`, transitively. See
  [Equivalency: Trimming and Native AOT](/docs/expectations/equivalency#trimming-and-native-aot).
- **Failure messages** render a registered object from its registration, so the message keeps listing its members
  after trimming. Boxed `KeyValuePair` entries and non-generic dictionaries render as `[key] = value` without
  reflection.
- **Event recording** registers the public events of every subject passed to `Record()` together with a handler
  factory, which also lifts the previous limit of four handler parameters. See
  [Events: Trimming and Native AOT](/docs/expectations/events#trimming-and-native-aot).
- **Initialization** no longer scans the loaded assemblies. The test framework adapter and extension initializers
  register themselves from a module initializer.

Reflection stays available as a fallback under the JIT for types the generator cannot see. When you publish with
trimming or Native AOT enabled, the fallback is switched off and such a type fails with an error that names it and
asks you to register it with `[assembly: GenerateMetadata(typeof(MyType))]`. The `AweXpectReflectionFallback` MSBuild
property forces the fallback either way, and the public `ReflectionFallback.IsSupported` switch lets an extension
guard its own reflection the same way.

Nothing changes for a project that is not trimmed: the registrations are generated regardless, and a type without one
is reflected over as before. The registry only holds public instance members, so a registered object no longer lists
static members, indexers or write-only properties in a failure message.

### What you have to do

- **Nothing**, if your test project only uses the built-in expectations under the JIT.
- **Extension authors** replace `IAweXpectInitializer` with a `[ModuleInitializer]`, mark extension methods whose
  argument reaches equivalency or a recording with `[RequiresMemberMetadata]` or `[RequiresEventMetadata]`, and
  register a hand-written `ITestFrameworkAdapter` explicitly. See [Extensions](#extensions-and-awexpectcore) below and
  [Write your own extension](/docs/expectations/write-extension#initialization).
- **Equivalency options** that inspected a `MemberInfo` move to `IgnoringFields` and `IgnoringProperties`, because a
  registration cannot supply one. See [Equivalency](#equivalency) below.

## Null subjects

v2 answered the question "what does a `null` subject do?" differently from expectation to expectation, and often
differently between an expectation and its negated form: `IsNotEmpty()` failed for a `null` collection but passed for
a `null` string, and `DoesNotComplyWith(it => it.HasItem(1))` passed for a `null` list.

v3 follows one rule. An expectation that **inspects** the subject fails for a `null` subject, in its negated form and
through `DoesNotComplyWith` as well, because there is nothing to inspect. An expectation that **compares** the subject
against a value you supply keeps treating `null` as an ordinary value on both sides, so `IsEqualTo(null)`,
`IsSameAs(null)` and `IsOneOf(null, "a")` succeed for a `null` subject.

The following expectations now **fail** for a `null` subject where they succeeded before:

| Area        | Expectations                                                                                                                   |
|-------------|--------------------------------------------------------------------------------------------------------------------------------|
| Strings     | `IsNotEmpty()`, `DoesNotStartWith(…)`, `DoesNotEndWith(…)`                                                                     |
| Collections | `DoesNotContain(…)`, `IsNotContainedIn(…)`, every negated inspection via `DoesNotComplyWith`                                    |
| Numbers     | `IsNotBetween(…)`, `IsNotNaN()`, `IsNotFinite()`, `IsNotInfinite()`, and the ordering comparisons and `IsPositive()` / `IsNegative()` on a nullable number under `DoesNotComplyWith` |
| Enums       | `DoesNotHaveFlag(…)`, `HasValue().NotEqualTo(…)`, `HasFlag(null)`                                                              |
| Types       | `IsNot<T>()`, `IsNotExactly<T>()`, `IsNot(Type)`, `IsNotExactly(Type)`, `IsNotEquatableTo(…)`, `IsNotParsableInto<T>()`         |
| Properties  | every `Has…().NotEqualTo(…)`, such as `HasLength()`, `HasLineCount()`, `HasDay()`, `HasKind()`, `HasPosition()`                |
| Booleans    | `Implies(…)` reports `it was <null>` instead of `it did not`                                                                    |

One change goes the other way: `IsSameAs(null)` and `IsNotSameAs(other)` now **succeed** for a `null` subject, because
identity is a comparison and `ReferenceEquals(null, null)` is `true`. Previously `IsSameAs(null)` failed with
`refers to <null>, but it was <null>`.

Every expectation that a `null` subject cannot satisfy carries a `[GuaranteesNotNull]` attribute, and the analyzer
reads it to suppress a CS8602 on the subject after the expectation ran. In v2 only three expectations were recognised
by name, so you may be able to remove `!` operators from your tests.

## Argument validation

Passing an empty value to search for made an expectation meaningless: every string contains `""`, every collection
starts with an empty sequence, and `ContainsKeys()` without arguments asserted nothing, so such a test could never
fail. Passing `null` produced a failure that had nothing to do with the subject. Both now throw at the call site:
`ArgumentNullException` for `null`, `ArgumentException` for an empty value.

- `Contains`, `StartsWith`, `EndsWith` and their negated forms, on strings and on collections, including the `params`
  overloads.
- `ContainsKeys`, `ContainsValues`, `DoesNotContainKeys`, `DoesNotContainValues`.
- `IsContainedIn` and `IsNotContainedIn` for a `null` collection. An empty collection is still accepted, because only an
  empty subject is contained in it.
- `HasMessage().Containing(…)` and `HasParamName().Containing(…)` and their `With…` twins. `EqualTo(null)` keeps
  accepting `null`, because `HasParamName().EqualTo(null)` is a real expectation.

The parameters are declared non-nullable, so a caller only hits the exception where a warning was already
suppressed.

## Vocabulary

The same comparison was spelled differently depending on which result type it happened to land in: `>=` was
`AtLeast` on `HasCount()` but `GreaterThanOrEqualTo` on a number. v3 draws one line. A continuation on a **value**
compares (`EqualTo`, `NotEqualTo`, `GreaterThan`, `GreaterThanOrEqualTo`, `LessThan`, `LessThanOrEqualTo`,
`Between`) and a continuation on an **occurrence count** counts (`Exactly`, `AtLeast`, `AtMost`, `MoreThan`,
`LessThan`, `Between`, `Never`).

Every scalar `Has…` expectation now offers both shapes that `HasCount` already had: `HasLength(9)` as the shorthand
and `HasLength().EqualTo(9)` as the explicit form with the full comparison vocabulary. This adds the shorthand to the
date and time components, `HasKind`, `HasOffset`, the `Version` components, `HasLength`, `HasLineCount`,
`HasPosition` and `HasBufferSize`, and adds the continuation to `HasHResult`, `HasValue` and `WithHResult`.

None of the renames has an `[Obsolete]` forwarder; each is a compile error that is fixed once.

### Renames and removals

| v2                                                                 | v3                                                              |
|--------------------------------------------------------------------|-----------------------------------------------------------------|
| `ThrowsException()`                                                | `Throws()`                                                      |
| `For(x => x.Member, m => m.IsEqualTo(…))`                          | `Whose(x => x.Member, m => m.IsEqualTo(…))`                     |
| `HasCount().MoreThan(n)`                                           | `HasCount().GreaterThan(n)`                                     |
| `HasCount().AtLeast(n)`                                            | `HasCount().GreaterThanOrEqualTo(n)`                            |
| `HasCount().AtMost(n)`                                             | `HasCount().LessThanOrEqualTo(n)`                               |
| `DoesNotHaveCount(n)`                                              | `HasCount().NotEqualTo(n)`                                      |
| `DoesNotHaveValue(n)` on an enum                                   | `HasValue().NotEqualTo(n)`                                      |
| `ExecutesIn().Approximately(expected, tolerance)`                  | `ExecutesIn(expected).Within(tolerance)`                        |
| `AreAllUnique()` on a collection                                   | `All().AreUnique()`                                             |
| `AreAllUnique()` on a dictionary                                   | `Values.All().AreUnique()`                                      |
| `HasMessageContaining(…)` / `WithMessageContaining(…)`             | `HasMessage().Containing(…)` / `WithMessage().Containing(…)`    |
| `DoesNotHaveMessage(…)` / `WithoutMessage(…)`                      | `HasMessage().NotEqualTo(…)` / `WithMessage().NotEqualTo(…)`    |
| `HasParamNameContaining(…)` and the other `ParamName` variants     | `HasParamName().Containing(…)` and so on                        |
| `Contains(…).Exactly()` (the parameterless match type)             | removed, it restated the default                                |

Two of these change behaviour beyond the name:

- `HasMessage().Containing(x)` is a literal substring match. The undocumented wildcard support of
  `HasMessageContaining` is gone; write `HasMessage("*x*").AsWildcard()` instead.
- `All().AreUnique()` reports duplicates through the quantifier family. A failure reads
  `is unique for all items, but only 2 of 4 were` instead of `only has unique items, but it contained 1 duplicate`,
  and `null` elements of a non-generic `IEnumerable` now compare as equal to each other, as they always did on the
  generic overloads.

### Failure messages

The rendering was unified where the same option had several spellings. Tests that assert on the exact failure text may
need an update.

- A tolerance renders as `± x` everywhere. `ExecutesIn` previously rendered `approximately 0:05 ± 0:01`, and date and
  time comparisons rendered `within 1:00`, which reads like a deadline.
- `HasHResult(5)` renders `has HResult equal to 5` instead of `has HResult 5`, `HasValue(1)` renders
  `has value equal to 1`, and `WithHResult(5)` renders `with HResult equal to 5`, because all three are now the
  shorthand for the continuation.
- A struct property nested under `HasInnerException` reads `whose HResult is equal to 42` instead of
  `whose has HResult equal to 42`.
- Collection equality reads like the other collection relations: `IsEqualTo(expected)` renders
  `is equal to collection expected in order` instead of `matches collection expected in order`, and `IsNotEqualTo`
  renders `is not equal to collection …` instead of `does not match collection …`.
- A member selected with `Whose(x => x.Message.Length, …)` renders without the leading dot:
  `whose Message.Length is …` and `but Message.Length was …` instead of `whose .Message.Length is …`.
- `DoesNotHaveItem(1).AtIndex(0)` names the item it found: `but it had item 1 at index 0` instead of `but it did`.
- A `Never()` quantifier appends its `within` window when one was given.
- Every equivalency member difference renders as a `Property X differed:` block with `Found:` and `Expected:` lines.
  A `null` on one side previously read `Property Value was <null> instead of "Foo"`, and an `It.Is<T>()` member read
  `Property IntValue was 1` or, on a type mismatch, `Property StringValue was string`. An `It.Is<T>()` member now
  shows its expectation as `Expected` and, on a type mismatch, the found type in parentheses:
  `Found: "abc" (string)`.
- An unexpected exception renders in one shape: its type in the `but` clause and its message indented below a colon.
  A `Task<T>` subject that throws now reads `but it did throw a NotSupportedException:` followed by the message,
  instead of the bare type name plus a separate `Exception:` section with the full `ToString()`. The exception is
  still forwarded as `InnerException` of the assertion exception, so its stack trace remains available there.
- `HasInner` and `WithInner` name the relation when the inner exception has the wrong type:
  `but it was an inner ArgumentException:` instead of `but it was an ArgumentException:`.
- The message of an unexpected exception is indented along with the rest of the result inside `Expect.ThatAll`.

## Timeouts on negative event expectations

`Within(…)` was ignored on every event expectation with an upper bound: `DidNotTrigger`, `DidNotTriggerPropertyChanged`,
`Never()`, `AtMost`, `LessThan`, `Exactly` and `Between`. The expectation returned immediately with the current
count, so an event fired later inside the window went unseen and `DidNotTrigger("X").Within(500.Milliseconds())`
passed after a few milliseconds. In v3 such an expectation waits out the full timeout and returns early only on the
event that breaks it. The same applies to a `Signaled()` count with an upper bound.

This is the one behavioural change that can make a passing test fail without touching its code: a negative event
expectation with `Within(…)` now observes the window it asks for.

## Equivalency

- `IsEquivalentTo` passed unconditionally when it found no member to compare. It now fails, unless every member was
  excluded explicitly through `IncludeMembers.None`.
- `MemberToIgnore.IgnoreMember` no longer receives a `MemberInfo`, because a registration cannot supply one. The
  `Ignoring` overload that exposed it is replaced by `IgnoringFields` and `IgnoringProperties`.
- `EquivalencyExtensions` moved to the `aweXpect` namespace, so `Contains(x).Equivalent()` needs no extra `using`.
  The `Equivalent` parameter is named `options`.
- Asking a registered type for `IncludeMembers.Internal` or `IncludeMembers.Private` reflects over the whole type,
  because only public members are registered. Under trimming this throws.
- Indexers are skipped, a `new`-hidden member is taken from the most derived type instead of throwing, a property on
  the actual side is read only through a public getter, and an exception thrown by a getter surfaces unwrapped.

## Extensions and aweXpect.Core

- `IAweXpectInitializer` is removed. Run the initialization from a `[ModuleInitializer]` as described in
  [Write your own extension](/docs/expectations/write-extension#initialization).
- A hand-written `ITestFrameworkAdapter` is no longer discovered by scanning the loaded assemblies on `net8.0` and
  later. Register it with `TestFrameworkRegistry.Register`. The generated adapters register themselves.
- The `aweXpect.Frameworks` project is renamed to `aweXpect.Generators` and hosts both generators.
- The result types of `Contains` and `IsContainedIn` were named after the pre-rename infinitives `Contain` and
  `BeContainedIn`. They are now named after what they do, such as `ProperCollectionMatchResult`, and the dictionary
  results `ContainsValueResult` and `ContainsValuesResult` are `ContainsKeyResult` and `ContainsKeysResult`.
  `EnumerableQuantifier` and `CollectionCountResult` moved out of the root namespace into `aweXpect.Options` and
  `aweXpect.Results`. A test only names these types when it stores a result in an explicitly typed variable.
- The receiver parameter of every expectation is named `subject` instead of `source`. This only affects the static
  call form with a named argument, such as `ThatString.IsEqualTo(subject: value, …)`.
- `AndOrWhichResult<,>` and `AndOrWhichResult<,,>` are removed. No built-in expectation returned them, so only an
  extension that constructed them directly is affected; `Whose` covers the member navigation they offered.
- `PropertyResult.Int`, `Long`, `TimeSpan` and `DateTimeKind` gain the `<TValue, TType, TThat>` shape that
  `PropertyResult.String` already had, with an `ExpectationGrammars` parameter, so one continuation renders
  `has HResult equal to 42` on a subject and `with HResult equal to 42` after `Throws()`. The existing `<TItem>`
  classes stay as thin subclasses, so `Has…` expectations built on them are unchanged.
- `PropertyResult.String` returns `StringEqualityTypeResult`, so `AsWildcard()` and friends are reachable, and gains
  `StartingWith`, `EndingWith` and their negations.
- The three `ConstraintResult` base classes document how they treat a `null` actual, and the
  [null rule](/docs/expectations/write-extension#constraints) that an extension has to follow is stated on the
  extension page.
- `ConstraintResult.WithValue<T>` takes the name of the subject like `WithNotNullValue<T>` and `WithEqualToValue<T>`:
  its constructor is `WithValue<T>(string it, ExpectationGrammars grammars)`, and the name is available as the
  inherited `It` property. A derived class passes its `it` to the base and uses `It` in its own result texts.
- The `[GuaranteesNotNull]` attribute in `aweXpect.Core` marks an expectation that a `null` subject can never satisfy,
  and the source generator emits it for expectations derived from `ConstraintResult.WithNotNullValue`.
- The option types follow the result vocabulary: `StringEqualityOptions.UsingComparer(comparer)` is renamed to
  `Using(comparer)` like `ObjectEqualityOptions.Using`, `StringEqualityOptions.Exactly()` is removed because exact
  matching is the default, and `TimeSpanEqualityOptions.Approximately(expected, tolerance)` is internal; construct an
  `ExecutesInToleranceResult` to get the tolerance continuation `ExecutesIn(expected).Within(tolerance)`.

## New expectations

- **Dictionaries** navigate to their keys and values: `That(dictionary).Keys.Contains(42)` and
  `That(dictionary).Values.All().AreUnique()` continue with the full collection vocabulary. These are C# 14 extension
  properties and need a C# 14 compiler.
- **Collections** gain a positional `DoesNotHaveItem(x).AtIndex(n)` and `DoesNotHaveItemThat(…)`, mirroring every
  `HasItem` overload. A collection too short to have an item at that index satisfies the expectation.
- **Uniqueness** is a quantifier: `AtLeast(1).AreNotUnique()`, `None().AreUnique()` and `AtLeast(4).AreUnique()`
  express what `AreAllUnique` could not.
- **Events** gain a positional `DidNotTrigger(eventName)`, the twin of `Triggered(eventName)`.
- **Delegates** gain `DoesNotSatisfy(…).Within(…)`, `Throws().WithHResult()` with the comparison vocabulary, and
  `WithMessage().StartingWith(…)` and `EndingWith(…)`.
- **Version** gains comparisons (`IsGreaterThan`, `IsBetween`, …) and the components `HasMajor`, `HasMinor`,
  `HasBuild` and `HasRevision`. See [Version](/docs/expectations/common-types/version).
- **Guid** gains `IsOneOf` and `IsNotOneOf`.
- **Char** gains `IsADigit`, `IsAnAsciiDigit`, `IsAnAsciiHexDigit`, `IsUpperCased`, `IsLowerCased` and `IsControl`
  with their negations.

## Analyzer

- `aweXpect0003` flags a `Has…` exception expectation bound directly after `Throws`, which compiles but reads
  "throws a CustomException has Message …". The code fix renames it to the `With…` twin or inserts `.Which`. See
  [Delegates](/docs/expectations/delegates#with-after-throws-has-on-the-exception).
- `aweXpect2001` warns when a type named in `[assembly: GenerateMetadata]` yields no registration.
- The nullability suppressor reads `[GuaranteesNotNull]` instead of a fixed list of three names, so it suppresses the
  warning after far more expectations than before.

## Fixes

- `And` and `Or` inside a `HasInner`, `WithInner` or `Whose` continuation silently dropped the later expectation,
  so a failing `And` passed and a passing `Or` failed.
- An expectation typed at the narrowed exception under `HasInner<T>(…)`, such as `Satisfies` or `Is<T>`, never
  matched and threw "does not support".
- `HasValue(0)` on a `null` enum subject succeeded, because `null` converted to `0`.
- `IsEquatableTo` threw a `NullReferenceException` for a `null` subject instead of failing.
- A number failure message dropped the difference when the subject was `MinValue`.
- The numeric conversion of `IsEqualTo` used `dynamic`, which is unavailable under Native AOT and swallowed its failure
  into a wrong result. It converts through generic math now, and `nint` and `nuint` take part.
- A boxed `KeyValuePair` rendered as `[foo, 42]` instead of `["foo"] = 42`.
- The reflective event recorder kept a handler for a static event subscribed after the subject was collected, so the
  event kept recording across tests. A handler that returns a value or takes a parameter by reference now fails with
  the event name and the reason instead of a bare `ArgumentException`.
- `IsNotBetween(1).And(3)` passed for a `null` number while `IsNotBetween(null).And(null)` failed.
- `IsEqualTo` and `IsNotEqualTo` on an `ImmutableArray<T>` offered a `Properly()` modifier that silently turned the
  equality into a proper subset check. They now return the same result as every other overload.
