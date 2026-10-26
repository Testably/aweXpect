---
title: What's new in v4
sidebar_position: 4
sidebar_label: From aweXpect 3.x
---

# What's new in v4

aweXpect v4 releases all packages together with one version and gives them one vocabulary: the extension packages
follow the same naming, option and nesting rules as aweXpect, and the types that extension authors build on are
consistent across Core and the packages. This page covers what you need to know to upgrade. The complete list of
changes, pull request by pull request, is in the [GitHub releases](https://github.com/Testably/aweXpect/releases).

To upgrade:

1. Update all aweXpect packages to the same 4.x version and fix the compile errors. Most of them are
   [renames](#renamed-expectations) or changes in an [extension package](#extension-packages).
2. Run your tests. Some failure messages changed and a few expectations are stricter now; the
   [behaviour changes](#behaviour-changes) explain them.
3. If you wrote your own expectations, also read [Extensions](#extensions).

Most changes only affect code that names a result type or passes an argument by name; chained expectations keep
compiling.

## One version for all packages

`aweXpect.Json`, `aweXpect.Web`, `aweXpect.Xml`, `aweXpect.Reflection` and `aweXpect.Snapshots` (with
`aweXpect.Snapshots.DiffEngine` and `aweXpect.Snapshots.PublicApi`) are now released together with `aweXpect` and
`aweXpect.Core`, with one version. Use the same version for all of them; a breaking change in any package waits for
the next major version.

- The extension packages jump from their own versions to 4.0.0.
- `aweXpect.Xml` and the `aweXpect.Snapshots` packages are still in beta and are published as `4.x.y-beta.1`.
- The extension packages are now signed with the key of aweXpect, so their public key token changes. This only
  matters for `InternalsVisibleTo` entries or binding redirects that name it.

## Renamed expectations

None of the renames has an `[Obsolete]` forwarder; each is a compile error that is fixed once:

| v3                                                | v4                                                           |
|---------------------------------------------------|--------------------------------------------------------------|
| `IsEquivalentTo(…, o => o.Ignoring(predicate))`   | `IsEquivalentTo(…, o => o.IgnoringMembers(predicate))`       |
| `IsValidJson().Which(j => …)`                     | `IsValidJson(j => …)`                                        |
| `IsValidXml().Which(root => …)`                   | `IsValidXml(root => …)`                                      |
| `HasStatusCode().DifferentTo(…)`                  | `HasStatusCode().NotEqualTo(…)`                              |
| `WithJsonOptions(…)` in the Json options          | `WithDocumentOptions(…)`                                     |
| `o => o with { … }` on `JsonOptions`              | the option methods, e.g. `o => o.IgnoringArrayOrder()`       |
| `Ignoring(predicate)` in the Json and Xml options | `IgnoringMembers(predicate)`                                 |
| `IsValidJson(o => o with { … })`                  | `IsValidJson(o => o.WithDocumentOptions(d => d with { … }))` |

`IsValidJson(null)` without a parameter name is now ambiguous; write `IsValidJson(options: null)`.

Parameters that name a looked-up item are called `name` (`HasHeader(name)`, `DoesNotHaveHeader(name)`), and compared
values are called `expected` or `unexpected`. This only affects arguments passed by name.

## Behaviour changes

### Failure messages

Failure messages were aligned across the packages, so tests that assert on the exact text of a failure message may
need an update. Among others:

- `Containing(…)` on a string property such as `HasMessage()` names the quantifier, e.g.
  `has message containing "foo" at least once`.
- Conflicting options are reported with one wording, e.g. `Using cannot be combined with AsWildcard.`
- In a plural context the noun of a generated kind check stays singular, e.g. `whose Items are a boolean for all
  items`, and a negated JSON kind check names the actual kind.
- The JSON and XML options are listed in the failure of the comparison itself, not in the failure of an unrelated
  expectation on the same subject.
- A missing JSON property and a missing or unexpected XML node name the value like a missing array element, e.g.
  `$.title had missing "Let It Be"` instead of `$.title was missing`.
- The status code expectations read like the operators of the other property results, e.g.
  `DoesNotHaveStatusCode(404)` and `HasStatusCode().NotEqualTo(404)` read `does not have status code equal to 404
  NotFound`, and `HasStatusCode().Success()` reads `has status code in the success range (2xx)`.

### Not nullable results

Expectations that a `null` subject can never satisfy return the subject as not nullable when awaited in all
packages, as they already did in aweXpect. This covers the Xml, Json and Snapshots expectations, the status code
expectations of aweXpect.Web and the collection expectations of aweXpect.Reflection. You may be able to remove `!`
operators after `await`.

### `Containing` on string properties

`HasMessage().Containing(…)`, `WithMessage().Containing(…)`, `HasParamName().Containing(…)` and the `Uri` property
checks of aweXpect.Web offer the quantifiers and match types of `Contains`, e.g.
`HasMessage().Containing("retry").AtLeast(2)`.

### Collections

- `AreEqualTo` on a struct enumerable such as `ImmutableArray<T>` takes an expected value of the item type and a
  typed comparer. An expected value of another type no longer compiles.
- `AreEquivalentTo` after a quantifier no longer offers `Using(…)`, which always threw; pass the equivalency options
  to `AreEquivalentTo` itself.
- The items of a collection of strings keep the nullability of the collection, like other items.

### Options

- A repeated option replaces the earlier one instead of throwing, e.g. `IgnoringCase().IgnoringCase(false)` compares
  case-sensitively and `AtLeast(2).AtLeast(3)` requires three. Options that set the same thing in different ways,
  e.g. `AsPrefix().AsSuffix()` or `AtLeast(2).AtMost(5)`, still throw.
- An option set to `false` is the same as not set, so `IgnoringIndentation(false).AsBlock()` no longer throws.
- A match type that does not support several options already set names all of them, e.g.
  `IgnoringIndentation and IgnoringNewlineStyle cannot be combined with AsBlock.`
- `WithCancellation(…)`, `WithTimeSystem(…)` and `Allocates(…)`, which apply to the whole chain of expectations, still
  throw when they are specified more than once.

### Customization

- The collection-valued settings, such as `ExcludedAssemblyPrefixes`, `ExcludedAttributeTypes` and the content
  processors of aweXpect.Web, are `IReadOnlyList<T>` instead of arrays.
- The excluded assembly prefixes moved to `Customize.aweXpect.Settings().ExcludedAssemblyPrefixes`. A prefix ending
  with a dot, such as `MyCompany.`, now matches in Core as well.

### Time system

`WithTimeSystem(…)` and the `ITimeSystem` it takes are marked experimental with the diagnostic `aweXpect5001`.
Suppress it in test projects that use a virtual time system.

## New expectations

- **Dates** gain `HasDayOfWeek(…)` on `DateTime`, `DateTimeOffset` and `DateOnly`.
- **Dictionaries** with string values offer the string match types, e.g. `ContainsValue("prefix*").AsWildcard()`.
- **Exceptions** keep their type after `HasInner(Type)` and `DoesNotHaveInner(Type)`, so `.And.HasParamName(…)`
  follows.
- **Delegates** keep their result type after `Because`, `WithTimeout` and `WithCancellation`, so
  `.Because(…).WhoseResult` compiles.
- **Nested subjects** such as `Throws().Which`, `WhoseResult` or `HasSingle().Which` offer `Is<TType>()` and the
  other type checks.

## Extension packages

### aweXpect.Json

- The nested expectations of `IsObject` and `IsArray` (`With`, `Without`, `At`, `AString`, `ANumber`,
  `WithElements`, …) are removed. Verify a value with `HasProperty`, `DoesNotHaveProperty`, `HasValueAt` or
  `HasElementAt`, and its .NET value with `IsString`, `IsNumber`, `IsNumber<TNumber>` or `IsBoolean`, either against an
  expected value, e.g. `IsString("Let It Be")`, or with all expectations of aweXpect. Counts and shapes are covered by
  `IsObject().WhoseProperties`,
  `IsArray().WhoseElements` and `Matches`.
- `IsString()` and `IsNumber()` offer the operators of a property on their result, so that the value is compared in the
  same sentence, e.g. `IsString().StartingWith("Let").IgnoringCase()` or `IsNumber().EqualTo(4.5).Within(0.1)`. They
  return `IsStringResult` and `IsNumberResult` from `aweXpect.Json.Results` instead of an `AndOrResult`, which only
  affects code that names the result type.
- `IsArray().WhoseElements` and `IsObject().WhoseProperties` on a `JsonNode` provide the nodes, as `JsonNode?` and
  `KeyValuePair<string, JsonNode?>` items, e.g. for `Contains(node)`, instead of `JsonElement` and `JsonProperty` items.
  `IsArrayResult` and `IsObjectResult` take the item type as third type parameter, which only affects code that names
  the result type.
- `Matches` and `IsValidJsonMatching` report additional array elements. Add
  `o => o.IgnoringAdditionalArrayElements()` to allow them.
- An expected `string` in `Matches`, `MatchesExactly`, `IsValidJsonMatching` and `IsValidJsonMatchingExactly` is JSON
  text, like in `IsJsonSerializedAs`, and invalid JSON throws an `ArgumentException`. A string that is a valid JSON
  scalar, e.g. `"42"`, `"true"` or `"null"`, silently changes its meaning. Pass `"\"Let It Be\""` or
  `(object)"Let It Be"` for a JSON string.
- `JsonOptions` is immutable like `XmlOptions` and configured only through its methods.

### aweXpect.Reflection

- **Inherited by default.** Checks consider inherited attributes, operators, members and relations, and
  `.Directly()` restricts them to what is declared on the item itself or to a direct relation. `bool inherit`,
  `MemberScope`, the `forceDirect` flag and the `…Directly…` methods are removed, e.g. `InheritsFrom<Base>(true)` is
  now `InheritsFrom<Base>().Directly()`. Operator and member checks such as `ContainsMethods` were declared-only
  before; append `.Directly()` to keep that. Listing members (`Types().Methods()`) stays declared-only;
  `.IncludingInherited()` adds the inherited ones and `.IncludingObjectMembers()` also those of `System.Object`.
- `AtIndex(i).FromEnd()` is now `AtIndexFromEnd(i)`, as in aweXpect.
- The namespace filters are `WhichAreWithinNamespace` and `WhichAreNotWithinNamespace`.
- The name filters use the string options of aweXpect, so they chain in any order and offer every option, e.g.
  `WithName("x").IgnoringCase().AsPrefix()`.
- Parameters can be filtered by a predicate with `Matching(p => …)`.
- The filter options (`TypeFilterOptions`, `AttributeFilterOptions<TMember>`, `ParameterFilterOptions`,
  `GenericArgumentsFilterOptions` and `GenericArgumentFilterOptions`) are internal, and the results and filters no
  longer offer them through `IOptionsProvider<…>`.
- The expectations on all items, such as `AreSealed()`, use the quantifier wording of aweXpect
  (`all are sealed, but only 2 of 5 were`), work with quantifiers (`AtLeast(2).AreSealed()`), and a `null` item fails
  them and their negation.
- The version components (`HasVersion().WithMajor`) offer all comparisons of an `int` property, including
  `Between(…).And(…)`.
- `Customize.aweXpect.Reflection()` and its settings need `using aweXpect.Reflection;`.
- `AccessModifiers` moved from `aweXpect.Reflection.Collections` to `aweXpect.Reflection`; replace the `using` where
  you name it.

### aweXpect.Snapshots (from 0.x)

- Rules are written with `.For`: select, filter, then act, e.g. `.For.Members().WithName("Id").Ignore()` or
  `.For.Guids().Mask()`. They replace `.Ignoring` and `.Masking`, and `SnapshotRule.For` with the `Rules` setting
  replaces `SnapshotIgnoring`, `SnapshotMasking` and their settings.
- `.Accepting(SnapshotAcceptance.All)`, `New` or `None` replaces `.AcceptingChanges(bool)`; `AWEXPECT_ACCEPT_SNAPSHOTS`
  takes `none`, `new` or `all`.
- Snapshot files carry the marker `.awexpect-snapshot.` instead of `.snapshot.`, and pending files match
  `*.awexpect-snapshot.pending.*`. Rename existing files and update your `.gitignore`.
- Names from `.Named(…)` are escaped in file names, so a name with `.` or `%` has to be accepted again.
- Numbered placeholders are wrapped in braces, e.g. `{Guid_1}`, so snapshots with masked values have to be accepted
  again.
- `byte[]`, `Stream` and `FileInfo` subjects are snapshotted as binary files.
- `MatchesPublicApiSnapshot` selects the API with `Within` and `Ignoring` instead of `ExcludingAttribute` and
  `ExcludingForwardedTypes`, and needs no extra `using`.
- `MatchesPublicApiSnapshot` has one overload per subject with an optional `options` parameter instead of one with and
  one without options. Calls compile unchanged, but have to be compiled again.
- `PublicApiOptions` is immutable: each option returns new options, so a call whose result is discarded has no
  effect.

### aweXpect.Web

- `HasHeader(…).WithValue(…)` and `HasQueryParameter(…).WithValue(…)` return a `StringEqualityTypeResult` and offer
  the match types, e.g. `WithValue("application/*").AsWildcard()`, like the type, title, detail and instance of
  `HasProblemDetailsContent`. This only affects code that names the result type.
- `HasStatusCode(…)`, `HasStatusCode().EqualTo(…)` and `HasStatusCode().NotEqualTo(…)` take an `HttpStatusCode`
  instead of an `HttpStatusCode?`. A response always has a status code, so `null` never matched.
- `HasStatusCode()` returns the enum property result of aweXpect.Core, `PropertyResult.Enum<…, HttpStatusCode>`,
  instead of `StatusCodeResult`, which was removed. `Success()`, `Redirection()`, `ClientError()`, `ServerError()` and
  `Error()` are extension methods in `HttpStatusCodePropertyResultExtensions`. Calls compile unchanged, but code that
  names `StatusCodeResult` has to use the new type.

### Namespaces

Results of the extension packages are in `aweXpect.<Package>.Results`, their option types (`JsonOptions`,
`XmlOptions`, `PublicApiOptions`) in `aweXpect.<Package>.Options` and their other public types in
`aweXpect.<Package>`. Add the matching `using` where you name such a type.

## Extensions

If you wrote your own expectations on `aweXpect.Core`, the compiler points out most of the changes. The important ones:

- Match types have one lifecycle: `StringMatchType` and `ObjectMatchType<TElement>` create a matcher per evaluation
  (`StringMatcher`, `ObjectMatcher<TElement>`) that returns a `MatchResult`. They replace `IStringMatchType` and
  `IObjectMatchType`. See [Custom match types](../11-extending/06-comparisons.md#custom-match-types).
- The operators of property results are extension methods on one interface per property type, such as
  `IIntPropertyResult<TResult>` or `IEnumPropertyResult<TEnum, TResult>`, so a result of your own gets them by
  implementing the interface. `IDoublePropertyResult<TResult, TToleranceResult>` and
  `IDecimalPropertyResult<TResult, TToleranceResult>` with `PropertyResult.Double` and `PropertyResult.Decimal` are new,
  and their `EqualTo` offers a tolerance, e.g. `EqualTo(4.5).Within(0.1)`. The `string` operators are split over `IStringEqualityPropertyResult<TResult>`,
  `IStringAffixPropertyResult<TResult>` and `IStringOccurrencePropertyResult<TResult>`, which
  `PropertyResult.String<TType, TThat, TValue>` implements instead of offering them as instance methods. Calls such as
  `HasMessage().EqualTo(…)` compile unchanged, but have to be compiled again.
- The expectations on quantified items extend the Core interfaces `IEnumerableItems<TItem>`,
  `IAsyncEnumerableItems<TItem>`, `IStructEnumerableItems<TCollection, TItem>` and
  `INonGenericEnumerableItems<TCollection>`, which replace `IEnumerableElements<TItem>` and its siblings.
- The options of an expectation are immutable values in one `OptionsHolder<TOptions>`, a reference to the current value.
  `IOptionsProvider<TOptions>.Options` returns the holder: create it in your expectation, e.g.
  `new OptionsHolder<Quantifier>(Quantifier.Default)`, pass it to your constraint and your result, and read
  `holder.Value` when the constraint evaluates. A constraint that only reads the options can take an
  `IReadOnlyOptionsHolder<TOptions>`. See [Options](../11-extending/05-options.md).
- `Quantifier`, `CollectionMatchOptions`, `CollectionIndexOptions`, `CollectionOrderOptions<TElement>`,
  `NumberTolerance<TNumber>`, `TimeTolerance`, `SignalerOptions`, `RepeatedCheckOptions` and `ExecutionTimeOptions`
  are `readonly struct`s, and `StringEqualityOptions` is a sealed immutable class. Start from their `Default`, e.g.
  `StringEqualityOptions.Default`, instead of a parameterless constructor, and for a date from `TimeTolerance.Days`.
  A method that sets an option returns a new value, so use the value it returns: a discarded call on a struct, e.g.
  `options.AtLeast(2);`, compiles and changes nothing. A repeated option replaces the earlier one, and methods that
  mirror a user-facing option take its name, e.g. `Within` instead of `SetTolerance`. `Quantifier.Never()` is an
  option method instead of a static factory, e.g. `Quantifier.Default.Never()`.
- The option methods are also extension methods on the holder in `aweXpect.Options`, which replace the value in the
  holder, e.g. `result.Options.AtLeast(2)`. Import `aweXpect.Options` where you call them: without it, the compiler
  finds only the user-facing option of the same name and reports `CS0311`.
- `ObjectEqualityOptions<TSubject>` and `ObjectEqualityWithToleranceOptions<TSubject, TTolerance>` are removed. Object
  comparisons keep their `ObjectMatchType<TElement>` in an `OptionsHolder<ObjectMatchType<TElement>>`, also with a
  tolerance, for which `ObjectMatchType<TElement>.WithTolerance(…)` creates an
  `ObjectToleranceMatchType<TElement, TTolerance>`. `ObjectEqualityOptionsExtensions` is renamed to
  `ObjectMatchTypeExtensions`.
- An `ObjectMatchType<TElement>` passes the name of its option to its base constructor, and `SetMatchType(matchType)`
  reads it, so that the same option again replaces the match type and another one throws. A match type without a name
  is rejected.
- `IOptionsEquality<TSubject>` is renamed to `IEqualityEvaluator<TElement>`. The new `IEqualityOptions<TElement>`
  returns the evaluator for one evaluation with `ForEvaluation(…)`, and `ObjectMatcher<TElement>` is such an evaluator.
- `DayTolerance` is removed; `TimeTolerance.Days` rejects a tolerance that is not a whole number of days.
  `IRelativeToleranceOptions<T>` is removed as well: `Percent()` calls the new `SetRelativeTolerance(…)` of
  `IWithinToleranceResult<TResult, TTolerance>`, which a result of your own implements: it makes the tolerance
  relative and returns the result to continue with.
- The string options carry no parameter name. The members that can reject the expected value take the name of the
  parameter instead: `ValidateExpected(expected, parameterName)`, `MatchWithExplanation(actual, expected,
  parameterName)`, `CountOccurrences(…, parameterName)` and `AreConsideredEqual(actual, expected, parameterName)` on
  the `StringEqualityOptions`, and `ValidateExpected(expected, parameterName)` of a `StringMatchType`.
- `CollectionMatchOptions.SetEquivalenceRelation` is removed; pass the relation to the constructor.
  `ExecutesInResult` and `ExecutesInToleranceResult` take the `ExpectationBuilder` as first constructor argument.
- `ExpectationBuilder.Timeout` only returns the timeouts added with `WithTimeout(…)`. The limits of `ExecutesIn()` and
  of `Within(…)` after `Throws…()` are kept apart, so that a later limit replaces an earlier one, and still apply when
  the expectation is evaluated.
- The predicates of a `Signaler<TParameter>` moved from `SignalerOptions<TParameter>` to
  `SignalerPredicateOptions<TParameter>`, where `WithPredicate` is `AddPredicate`, like on the `TriggerEventFilter`.
- The signaler and event recording expectations extend the nullable `IThat<Signaler?>`, `IThat<Signaler<TParameter>?>`
  and `IThat<IEventRecording<TSubject>?>`, and their results continue on it, so your own expectations on these subjects extend it as well.
- `ConstraintResult` creates a result that fails both ways with `FailureBothWays` instead of `Fail`.
- A `ConstraintResult` negates itself in `protected override void NegateInPlace()` instead of
  `public override ConstraintResult Negate()`, so a negation can no longer return a new result that is then ignored.
  Negate a result with `.Invert()`, or with the new `.InvertIf(isNegated)`.
- `ExpectationBuilder.WithCancellation` and `WithTimeout` return the builder, and the async `ForWhich` is
  `ForAsyncWhich`. `MemberAccessor.FromFunc` and `FromFuncAsMemberAccessor` are `FromFuncWithName` and
  `FromFuncWithCallerExpression`.
- Nested subjects are `IThatSubject<T>`, and nested expectations take an `Action<IThatSubject<T>>`.
- Results no longer implement `IOptionsProvider<ExpectationBuilder>`; use `EquivalencyExpectations.TryGetBuilder` for
  `It.Is…` expectations.
- The result constructors of aweXpect are internal; the results of `aweXpect.Core` stay public as extension points.
  `ObjectEqualityWithToleranceResult` and `StringCollectionMatchResult` moved to Core.
- The quantified collection constraints moved to `aweXpect.Core.Extending`, and the occurrence count of `Contains`
  is available there as `AddStringOccurrenceConstraint`.
- `ReflectionFallback` moved from `aweXpect.Core` to `aweXpect.Core.Extending`, and its `NotSupported(what, remedy)`
  is public, so that an extension throws the same error as the built-in expectations when reflection is switched off.
  aweXpect.Json and aweXpect.Reflection use it too, so their wording of this error changed slightly. The runtime switch
  `aweXpect.ReflectionFallback.IsSupported` and the `AweXpectReflectionFallback` property keep their names.
- The type parameters of results follow one scheme: `TType` for the subject, `TThat` for the subject wrapper, `TSelf`
  for the result itself, `TResult` for the result a call continues with, `TCollection` and `TItem` for a collection
  and its items and `TElement` for the value an option compares, e.g. `CollectionOrderResult<TCollection, TThat, TElement>`. Code that names a result changes where the
  order or the number of type parameters changed: `PropertyResult.Int<TType, TThat, TValue>`, its siblings and
  `EnumValueResult<TType, TValue>` start with the subject type, `HasItemResult`, `ObjectHasItemResult`,
  `ObjectHasItemWithToleranceResult`, `StringHasItemResult` and `StringBlockHasItemResult` take a `TThat` after the
  collection, and `ContainsKeyResult` and `ContainsKeysResult` have no `TKey`.
- `[GenerateCollectionExpectation]` also generates the expectation directly on a collection with `CollectionOverload`
  and `NegatedName`.
- `ITestFrameworkAdapter` has the new members `TryGetCurrentTest`, `TryGetCancellationToken` and `TryAddAttachment`,
  so an adapter of your own has to implement them; each returns `false` when the test framework does not support it.
  See [the running test](../11-extending/11-initialization.md#the-running-test).
