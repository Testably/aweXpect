---
description: What changed in aweXpect v4 and how to upgrade from aweXpect 3.x.
sidebar_label: From aweXpect 3.x
---

# What's new in v4

aweXpect v4 releases all packages together with one version and gives them one vocabulary: the extension packages follow the same naming, option and nesting rules as aweXpect, and the types that extension authors build on are consistent across Core and the packages. This page covers what you need to know to upgrade. The complete list of changes, pull request by pull request, is in the [GitHub releases](https://github.com/Testably/aweXpect/releases).

To upgrade:

1. Update all aweXpect packages to the same 4.x version and fix the compile errors. Most of them are [renames](#renamed-expectations) or changes in an [extension package](#extension-packages).
2. Review the [changes the compiler does not catch](#changes-the-compiler-does-not-catch).
3. Run your tests. Some failure messages changed and a few expectations are stricter now; the [behavior changes](#behavior-changes) explain them.
4. If you wrote your own expectations, also read [Extensions](#extensions).

Most changes only affect code that names a result type or passes an argument by name; chained expectations keep compiling.

:::warning[Compile everything against v4]
Many return types changed, so a binary compiled against v3 is not supported. This includes extension packages of other authors: use a version that is built for v4.
:::

:::tip[Upgrading from aweXpect 2.x]
Work through [What's new in v3](./migration-to-v3.md) first, then through this page. Some rules changed again in v4, e.g. a repeated option threw in v3 and replaces the earlier one in v4.
:::

## What to read

| If you use…                                                               | Read                                                                                                                 |
|---------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------|
| aweXpect                                                                  | [Renamed expectations](#renamed-expectations) and [Behavior changes](#behavior-changes)                             |
| aweXpect.Json, aweXpect.Reflection, aweXpect.Web or aweXpect.Xml          | also its section under [Extension packages](#extension-packages) and [Namespaces](#namespaces)                       |
| aweXpect.Snapshots                                                        | also [aweXpect.Snapshots](#awexpectsnapshots-from-0x): existing snapshot files have to be renamed                    |
| expectations of your own on `aweXpect.Core`                               | also [Extensions](#extensions)                                                                                       |

## Changes the compiler does not catch

These changes compile unchanged, but an existing test may now check something different than before. Check whether your tests rely on them:

- **aweXpect.Json: an expected string is JSON text.** In `Matches`, `MatchesExactly`, `IsValidJsonMatching` and `IsValidJsonMatchingExactly`, `"42"` now expects the number 42 and `"true"` the boolean. Write `"\"42\""` or `(object)"42"` for a JSON string. See [aweXpect.Json](#awexpectjson).
- **aweXpect.Reflection: checks include inherited items.** Checks such as `ContainsMethods(…)` also see what a base type declares, so they pass when a member moves to the base class. Append `.Directly()` where only the type itself counts. See [aweXpect.Reflection](#awexpectreflection).
- **aweXpect.Web: host, scheme and media type ignore case.** `HasHost`, `HasScheme`, `HasContentType` and `DoesNotHaveContentType` no longer notice a different casing. Append `.IgnoringCase(false)` to keep a case-sensitive check. See [aweXpect.Web](#awexpectweb).

## One version for all packages

`aweXpect.Json`, `aweXpect.Web`, `aweXpect.Xml`, `aweXpect.Reflection` and `aweXpect.Snapshots` (with `aweXpect.Snapshots.DiffEngine` and `aweXpect.Snapshots.PublicApi`) are now released together with `aweXpect` and `aweXpect.Core`, with one version. Use the same version for all of them; a breaking change in any package waits for the next major version.

- The extension packages jump from their own versions to 4.0.0.
- `aweXpect.Xml` and the `aweXpect.Snapshots` packages are still in beta and are published as `4.x.y-beta.1`.
- The extension packages are now signed with the key of aweXpect, so their public key token changes. This only matters for `InternalsVisibleTo` entries or binding redirects that name it.

## Renamed expectations

None of the renames has an `[Obsolete]` forwarder; each is a compile error that is fixed once:

| v3                                                | v4                                                           |
|---------------------------------------------------|--------------------------------------------------------------|
| `IsEquivalentTo(…, o => o.Ignoring(predicate))`   | `IsEquivalentTo(…, o => o.IgnoringMembers(predicate))`       |
| `IsEquivalentTo(…, o => o.Ignoring(type => …))`   | `IsEquivalentTo(…, o => o.IgnoringMembers((_, type) => …))`  |
| `IsValidJson().Which(j => …)`                     | `IsValidJson(j => …)`                                        |
| `IsValidXml().Which(root => …)`                   | `IsValidXml(root => …)`                                      |
| `HasStatusCode().DifferentTo(…)`                  | `HasStatusCode().NotEqualTo(…)`                              |
| `WithJsonOptions(…)` in the Json options          | `WithDocumentOptions(…)`                                     |
| `o => o with { … }` on `JsonOptions`              | the option methods, e.g. `o => o.IgnoringArrayOrder()`       |
| `Ignoring(predicate)` in the Json and Xml options | `IgnoringMembers(predicate)`                                 |
| `IsValidJson(o => o with { … })`                  | `IsValidJson(o => o.WithDocumentOptions(d => d with { … }))` |
| `IsValidJson(null)`                               | `IsValidJson(options: null)`, as the call is now ambiguous   |

The renames of aweXpect.Reflection and aweXpect.Snapshots are listed in their [sections](#extension-packages).

Parameters that name a looked-up item are called `name` (`HasHeader(name)`, `DoesNotHaveHeader(name)`), and compared values are called `expected` or `unexpected`. This only affects arguments passed by name.

## Behavior changes

### Failure messages

Failure messages were aligned across the packages, so tests that assert on the exact text of a failure message may need an update.

<details>
<summary>Examples of changed failure messages</summary>

- `Containing(…)` on a string property such as `HasMessage()` names the quantifier, e.g. `has message containing "foo" at least once`.
- Conflicting options are reported with one wording, e.g. `Using cannot be combined with AsWildcard.`
- In a plural context the noun of a generated kind check stays singular, e.g. `whose Items are a boolean for all items`, and a negated JSON kind check names the actual kind.
- The JSON and XML options are listed in the failure of the comparison itself, not in the failure of an unrelated expectation on the same subject.
- A missing JSON property and a missing or unexpected XML node name the value like a missing array element, e.g. `$.title had missing "Let It Be"` instead of `$.title was missing`.
- The status code expectations read like the operators of the other property results, e.g. `DoesNotHaveStatusCode(404)` and `HasStatusCode().NotEqualTo(404)` read `does not have status code equal to 404 NotFound`, and `HasStatusCode().Success()` reads `has status code in the success range (2xx)`.
- A failing snapshot ends with a `Snapshot options` context that lists the rules, the comparer and the acceptance in effect.
- The item lists of aweXpect.Reflection and the difference lists of aweXpect.Json and aweXpect.Xml end with `(… and n more)` after `MaximumNumberOfCollectionItems` items, like the collections of aweXpect.

</details>

### Options

The option rules are the same for all packages now, see [Options](../02-how-it-works/05-options.md):

- A repeated option replaces the earlier one instead of throwing, e.g. `IgnoringCase().IgnoringCase(false)` compares case-sensitively and `AtLeast(2).AtLeast(3)` requires three. Options that set the same thing in different ways, e.g. `AsPrefix().AsSuffix()` or `AtLeast(2).AtMost(5)`, still throw.
- An option set to `false` is the same as not set, so `IgnoringIndentation(false).AsBlock()` no longer throws.
- A match type that does not support several options already set names all of them, e.g. `IgnoringIndentation and IgnoringNewlineStyle cannot be combined with AsBlock.`
- `WithCancellation(…)`, `WithTimeProvider(…)` and `Allocates(…)`, which apply to the whole chain of expectations, still throw when they are specified more than once.

### Collections

- `AreEqualTo` on a struct enumerable such as `ImmutableArray<T>` takes an expected value of the item type and a typed comparer. An expected value of another type no longer compiles.
- `AreEquivalentTo` after a quantifier no longer offers `Using(…)`, which always threw; pass the equivalency options to `AreEquivalentTo` itself.
- The items of a collection of strings keep the nullability of the collection, like other items.
- The item expectations of `Contains`, `IsEqualTo`, `IsContainedIn` and their negations receive the items as `IThatSubject<TItem>` instead of `IThatSubject<TItem?>`, like `HasItemThat` and `ComplyWith`. An explicitly typed lambda with a nullable parameter on a collection of non-nullable items now warns with CS8622.

### Customization

- The collection-valued settings, such as `ExcludedAssemblyPrefixes`, `ExcludedAttributeTypes` and the content processors of aweXpect.Web, are `IReadOnlyList<T>` instead of arrays.
- The excluded assembly prefixes moved to `Customize.aweXpect.Settings().ExcludedAssemblyPrefixes`. A prefix ending with a dot, such as `MyCompany.`, now matches in Core as well.
- The assemblies of TUnit are excluded by default, like those of the other test frameworks, so e.g. `DependsOnlyOn` of aweXpect.Reflection no longer reports them.

### Time provider

`WithTimeSystem(…)` and `ITimeSystem` from `aweXpect.Core.Internal` are replaced by `WithTimeProvider(…)` in the namespace `aweXpect.Core`, which takes a `System.TimeProvider`, e.g. the `FakeTimeProvider` of `Microsoft.Extensions.TimeProvider.Testing`. On .NET Framework, `TimeProvider` comes from the `Microsoft.Bcl.TimeProvider` package, a new dependency of `aweXpect.Core` for `netstandard2.0`. See [on a virtual clock](../02-how-it-works/06-time-and-cancellation.md#on-a-virtual-clock).

## Extension packages

Each section first lists what the compiler reports, then the behavior changes. New expectations are listed under [New in v4](#new-in-v4).

### aweXpect.Json

**Fix compile errors**

- The nested expectations of `IsObject` and `IsArray` (`With`, `Without`, `At`, `AString`, `ANumber`, `WithElements`, …) are removed. Verify a value with `HasProperty`, `DoesNotHaveProperty`, `HasValueAt` or `HasElementAt`, and its .NET value with `IsString`, `IsNumber`, `IsNumber<TNumber>` or `IsBoolean`, either against an expected value, e.g. `IsString("Let It Be")`, or with all expectations of aweXpect. Counts and shapes are covered by `IsObject().WhoseProperties`, `IsArray().WhoseElements` and `Matches`.
- `JsonOptions` is immutable like `XmlOptions` and configured only through its methods.
- `IsJsonSerializable<T>()` on an `object` subject is removed; the type is that of the subject, and an overload takes a `JsonTypeInfo<T>` for trimming and Native AOT.
- `IsArray().WhoseElements` and `IsObject().WhoseProperties` on a `JsonNode` provide the nodes, as `JsonNode?` and `KeyValuePair<string, JsonNode?>` items, e.g. for `Contains(node)`, instead of `JsonElement` and `JsonProperty` items. `IsArrayResult` and `IsObjectResult` take the item type as third type parameter, which only affects code that names the result type.
- `IsString()` and `IsNumber()` return `IsStringResult` and `IsNumberResult` from `aweXpect.Json.Results` instead of an `AndOrResult`, which only affects code that names the result type.

**Check behavior**

- An expected `string` in `Matches`, `MatchesExactly`, `IsValidJsonMatching` and `IsValidJsonMatchingExactly` is JSON text, like in `IsJsonSerializedAs`, and invalid JSON throws an `ArgumentException`. A string that is a valid JSON scalar, e.g. `"42"`, `"true"` or `"null"`, silently changes its meaning. Pass `"\"Let It Be\""` or `(object)"Let It Be"` for a JSON string.
- `Matches` and `IsValidJsonMatching` report additional array elements. Add `o => o.IgnoringAdditionalArrayElements()` to allow them.
- `IgnoringMember(name)` matches the property name case-sensitively, like the comparison itself, so `IgnoringMember("id")` no longer ignores an `ID` property.
- `IsValidJson(j => …)`, `IsValidJsonMatching(…)` and `IsValidJsonMatchingExactly(…)` fail both ways when the subject is not valid JSON, so their negation no longer passes for invalid JSON. `IsValidJson()` without expectations keeps its negation.
- An invalid or `null` expected JSON in `IsJsonSerializedAs(…)` throws when the expectation is written, also when it is never awaited.
- `IsJsonSerializable()` uses `Customize.aweXpect.Json().DefaultJsonSerializerOptions` when no options are passed and keeps the subject type.
- A JSON number is passed to `It.Is<T>()` as the numeric type `T` when it fits, so `It.Is<long>()` or `It.Is<decimal>()` match a number like `5`.

### aweXpect.Reflection

Checks consider inherited attributes, operators, members and relations by default, and `.Directly()` restricts them to what is declared on the item itself or to a direct relation:

```diff
- await That(typeof(LiveAlbum)).InheritsFrom<Album>(true);
+ await That(typeof(LiveAlbum)).InheritsFrom<Album>().Directly();
```

**Fix compile errors**

| v3                                                         | v4                                                           |
|------------------------------------------------------------|--------------------------------------------------------------|
| `bool inherit`, `MemberScope`, `forceDirect` and the `…Directly…` methods | `.Directly()`, e.g. `InheritsFrom<Base>(true)` is now `InheritsFrom<Base>().Directly()` |
| `AtIndex(i).FromEnd()`                                     | `AtIndexFromEnd(i)`, as in aweXpect                          |
| `OfType` / `OfExactType` on properties, fields and events  | `WhichAreOfType` / `WhichAreOfExactType`                     |
| `WithoutParameters()`                                      | `WithNoParameters()`, like the expectation `HasNoParameters()` |
| `Types(modifier)`, also `Classes`, `Enums`, `Interfaces`, `Records`, `RecordStructs` and `Structs` | `Types().WhichAre(modifier)`          |
| `TypeFilters.GenericTypes.GenericTypesWithArgument` and `…WithNamedArgument` | declared in `TypeFilters`, like their counterparts in `MethodFilters` |
| `Customize.aweXpect.Reflection().DependencyResolver().Set(…)` | `Customize.aweXpect.Reflection().DependencyResolver.Set(…)`, with `using aweXpect.Reflection;` |
| `AccessModifiers` in `aweXpect.Reflection.Collections`     | `AccessModifiers` in `aweXpect.Reflection`                   |

- The namespace filters are `WhichAreWithinNamespace` and `WhichAreNotWithinNamespace`. `OrOfType` and `OrOfExactType` keep their names.
- The filter options (`TypeFilterOptions`, `AttributeFilterOptions<TMember>`, `ParameterFilterOptions`, `GenericArgumentsFilterOptions` and `GenericArgumentFilterOptions`) are internal, and the results and filters no longer offer them through `IOptionsProvider<…>`.
- `IFilter<TEntity>.Applies` receives the `CancellationToken` of the enumeration; add the parameter to your own filters and pass it on to asynchronous work.

**Check behavior**

- Operator and member checks such as `ContainsMethods` were declared-only before; append `.Directly()` to keep that. Listing members (`Types().Methods()`) stays declared-only; `.IncludingInherited()` adds the inherited ones and `.IncludingObjectMembers()` also those of `System.Object`.
- `AtIndex(i)` on parameters and generic arguments only matches at index `i`; before, an item at a lower index also satisfied it.
- The expectations on all items that check a condition on each item, such as `AreSealed()`, `InheritFrom<T>()`, `Implement<T>()`, `HaveOperator(…)` or `ContainMethods(…)`, use the quantifier wording of aweXpect (`all are sealed, but only 2 of 5 were`, and `none inherit from Base` for a negation), work with quantifiers (`AtLeast(2).InheritFrom<Base>().Directly()`), and a `null` item fails them and their negation.

### aweXpect.Snapshots (from 0.x)

:::warning[Update your existing snapshots]
1. Rename the snapshot files: they carry the marker `.awexpect-snapshot.` instead of `.snapshot.`.
2. Update your `.gitignore`: pending files match `*.awexpect-snapshot.pending.*`.
3. Accept again the snapshots with masked values, as numbered placeholders are wrapped in braces, e.g. `{Guid_1}`, and those with a name from `.Named(…)` that contains `.` or `%`, as the name is escaped in the file name.
4. If you set `AWEXPECT_ACCEPT_SNAPSHOTS`, use `none`, `new` or `all`.
:::

**Fix compile errors**

| v3                                                         | v4                                                                           |
|------------------------------------------------------------|------------------------------------------------------------------------------|
| `.Ignoring…` and `.Masking…`                               | rules with `.For`: select, filter, then act, e.g. `.For.Members().WithName("Id").Ignore()` or `.For.Guids().Mask()` |
| `SnapshotIgnoring`, `SnapshotMasking` and their settings   | `SnapshotRule.For` with the `Rules` setting                                  |
| `.AcceptingChanges(bool)`                                  | `.Accepting(SnapshotAcceptance.All)`, `New` or `None`                        |
| `.ComparedWith(comparer)`                                  | `.Using(comparer)`, like the other comparer options                          |
| `ExcludingAttribute` and `ExcludingForwardedTypes` of `MatchesPublicApiSnapshot` | `Within` and `Ignoring`, without an extra `using`      |
| `SnapshotResult<TType, TThat>`                             | `StringSnapshotResult` or `ObjectSnapshotResult`, depending on the subject   |
| `DiffEngineReporter` in `aweXpect.Snapshots`               | `DiffEngineReporter` in `aweXpect.Snapshots.DiffEngine`                      |

- The result type depends on the subject, so the rules offer only what applies, e.g. `.For.Lines()` only on a string snapshot.
- `MatchesPublicApiSnapshot` has one overload per subject with an optional `options` parameter instead of one with and one without options. Calls compile unchanged, but have to be compiled again.
- `PublicApiOptions` is immutable: each option returns new options, so a call whose result is discarded has no effect. The analyzer warning [aweXpect0009](../08-analyzers.md#awexpect0009) reports it.

**Check behavior**

- `byte[]`, `Stream` and `FileInfo` subjects are snapshotted as binary files.
- Snapshot ids of one test that differ only in case, e.g. `.Named("Live")` and `.Named("live")`, fail on every operating system, also on Linux, where they passed before.
- Snapshot files are never compiled, also with `.WithExtension("cs")`, and the IDE nests them under their test file; `<AweXpectSnapshotsNestFiles>false</AweXpectSnapshotsNestFiles>` turns the nesting off.

### aweXpect.Web

**Fix compile errors**

- `HasStatusCode(…)`, `HasStatusCode().EqualTo(…)` and `HasStatusCode().NotEqualTo(…)` take an `HttpStatusCode` instead of an `HttpStatusCode?`. A response always has a status code, so `null` never matched.
- `HasStatusCode()` returns the enum property result of aweXpect.Core, `PropertyResult.Enum<…, HttpStatusCode>`, instead of `StatusCodeResult`, which was removed. `Success()`, `Redirection()`, `ClientError()`, `ServerError()` and `Error()` are extension methods in `HttpStatusCodePropertyResultExtensions`. Calls compile unchanged, but code that names `StatusCodeResult` has to use the new type.
- `HasHeader(…).WithValue(…)` and `HasQueryParameter(…).WithValue(…)` return a `StringEqualityTypeResult`. This only affects code that names the result type.
- `HasProblemDetailsContent()` without a type no longer offers string options or match types, and `HasProblemDetailsContent(…)` requires a non-null type.
- The expected values of `HasProblemDetailsContent(…)` and its `WithTitle`, `WithStatus`, `WithDetail` and `WithInstance` are named `expected` instead of `type`, `title`, `status`, `detail` and `instance`. This only affects calls with named arguments.
- The built-in content processors `JsonContentProcessor`, `StringContentProcessor` and `BinaryContentProcessor` are sealed. A content processor of your own implements `IContentProcessor` and can delegate to a built-in one.

**Check behavior**

- `HasHost` and `HasScheme` compare ignoring case by default, also without an expected value, as RFC 3986 defines the host and the scheme as case-insensitive. Add `.IgnoringCase(false)` before `.Using(comparer)` or a match type of your own that cannot ignore the casing.
- `HasContentType` and `DoesNotHaveContentType` compare the media type ignoring case by default, as RFC 9110 defines it case-insensitively. Add `.IgnoringCase(false)` to compare it case-sensitively, which `.Using(comparer)` needs as well.
- `HasHeader` and `DoesNotHaveHeader` also read the headers of the content, so `HasHeader("Content-Type")` now succeeds and `DoesNotHaveHeader("Content-Type")` fails when the content has this header, the same for `Content-Length`, `Content-Encoding` or `Content-Disposition`. Use `HasContentType` to verify the media type.
- `HasHeader(…).WhoseValue(…)` and `HasQueryParameter(…).WhoseValue(…)` fail, also when negated, if the header or query parameter does not have exactly one value, like `WithValue`. Before, the expectations on the value received `null`, so that e.g. `WhoseValue(value => value.IsNull())` succeeded for several values.
- A request without content has an empty string content, like a response, so `HasContent("")` and `HasContent(c => c.IsEmpty())` succeed and `DoesNotHaveContent("")` fails, as HTTP does not distinguish a missing body from an empty one.
- `HasPath` keeps an encoded slash (`%2F`) in the path, so `/artists/AC%2FDC` no longer has the path `/artists/AC/DC`, but `/artists/AC%2FDC`.
- A problem details content without a `type` member has the type `"about:blank"` instead of failing.
- `IsLoopback` and `IsNotLoopback` of an `IPAddress` map an IPv4-mapped address to IPv4 first, so `::ffff:127.0.0.2` is a loopback address on every target framework.

### aweXpect.Xml

**Fix compile errors**

- `HasAttribute(name)` and the new `HasValue()` return an `XmlValueResult`. This only affects code that names the result type.
- The nested expectations of `HasElement`, `HasAttribute` and `HasValue` receive a non-nullable `XElement` or `string`.
- `XmlOptions` has no public constructor and is configured only through its methods.

**Check behavior**

- `IsValidXml(root => …)` and `IsValidXmlMatching(…)` fail both ways when the subject is not valid XML, so their negation no longer passes for invalid XML. `IsValidXml()` without expectations keeps its negation.
- `IgnoringAdditionalNodes()` in the XML options lets every comparison match a part of the subject, also `IsEqualTo(…).AsXml()`.

### Namespaces

Results of the extension packages are in `aweXpect.<Package>.Results`, their option types (`JsonOptions`, `XmlOptions`, `PublicApiOptions`) in `aweXpect.<Package>.Options` and their other public types in `aweXpect.<Package>`, e.g. `aweXpect.Snapshots.DiffEngine` or `aweXpect.Snapshots.PublicApi.Options`. Add the matching `using` where you name such a type. The option methods, e.g. `o => o.IgnoringArrayOrder()`, are in `aweXpect` and need no extra `using`.

The customization classes are called `JsonCustomizationExtensions`, `WebCustomizationExtensions` and `SnapshotsCustomizationExtensions` instead of `…AwexpectCustomizationExtensions`. Calls such as `Customize.aweXpect.Json()` compile unchanged.

## Extensions

If you wrote your own expectations on `aweXpect.Core`, the compiler points out most of the changes.

### Renamed and moved types

| v3                                                                 | v4                                                                         |
|--------------------------------------------------------------------|----------------------------------------------------------------------------|
| `IStringMatchType`, `IObjectMatchType`                             | `StringMatchType`, `ObjectMatchType<TElement>`, see [match types](#match-types) |
| `IEnumerableElements<TItem>` and its siblings                      | `IEnumerableItems<TItem>`, `IAsyncEnumerableItems<TItem>`, `IStructEnumerableItems<TCollection, TItem>`, `INonGenericEnumerableItems<TCollection>` |
| `IOptionsEquality<TSubject>`                                       | `IEqualityEvaluator<TElement>`                                             |
| `ObjectEqualityOptionsExtensions`                                  | `ObjectMatchTypeExtensions`                                                |
| `BetweenResult<TTarget>`, `BetweenResult<TTarget, TType>` and the nested `BetweenResult` of `AllocatesResult` and `ExecutesInResult` | `BetweenResult<TResult, TValue>` |
| `DayTolerance`                                                     | `TimeTolerance.Days`, which rejects a tolerance that is not a whole number of days |
| the predicates of `SignalerOptions<TParameter>`, `WithPredicate`   | `SignalerPredicateOptions<TParameter>`, `AddPredicate`, like on the `TriggerEventFilter` |
| `Fail` for a `ConstraintResult` that fails both ways              | `FailureBothWays`                                                          |
| `public override ConstraintResult Negate()`                        | `protected override void NegateInPlace()`, see [constraint results](#constraints-and-evaluation) |
| the async `ExpectationBuilder.ForWhich`                            | `ForAsyncWhich`                                                            |
| `MemberAccessor.FromFunc`, `FromFuncAsMemberAccessor`              | `FromFuncWithName`, `FromFuncWithCallerExpression`                         |
| `ReflectionFallback` in `aweXpect.Core`                            | `ReflectionFallback` in `aweXpect.Core.Extending`                          |
| `Quantifier.Never()` as static factory                             | the option method, e.g. `Quantifier.Default.Never()`                       |
| `SetTolerance` and other methods that mirror a user-facing option  | the name of the option, e.g. `Within`                                      |

### Match types

- Match types have one lifecycle: `StringMatchType` and `ObjectMatchType<TElement>` create a matcher per evaluation (`StringMatcher`, `ObjectMatcher<TElement>`) that returns a `MatchResult`. See [Custom match types](../11-extending/06-comparisons.md#custom-match-types).
- `ObjectEqualityOptions<TSubject>` and `ObjectEqualityWithToleranceOptions<TSubject, TTolerance>` are removed. Object comparisons keep their `ObjectMatchType<TElement>` in an `OptionsHolder<ObjectMatchType<TElement>>`, also with a tolerance, for which `ObjectMatchType<TElement>.WithTolerance(…)` creates an `ObjectToleranceMatchType<TElement, TTolerance>`.
- An `ObjectMatchType<TElement>` passes the name of its option to its base constructor, and `SetMatchType(matchType)` reads it, so that the same option again replaces the match type and another one throws. A match type without a name is rejected.
- The new `IEqualityOptions<TElement>` returns the evaluator for one evaluation with `ForEvaluation(…)`, and `ObjectMatcher<TElement>` is such an evaluator.
- The string options carry no parameter name. The members that can reject the expected value take the name of the parameter instead: `ValidateExpected(expected, parameterName)`, `MatchWithExplanation(actual, expected, parameterName)`, `CountOccurrences(…, parameterName)` and `AreConsideredEqual(actual, expected, parameterName)` on the `StringEqualityOptions`, and `ValidateExpected(expected, parameterName)` of a `StringMatchType`.

### Options

- The options of an expectation are immutable values in one `OptionsHolder<TOptions>`, a reference to the current value. `IOptionsProvider<TOptions>.Options` returns the holder: create it in your expectation, e.g. `new OptionsHolder<Quantifier>(Quantifier.Default)`, pass it to your constraint and your result, and read `holder.Value` when the constraint evaluates. A constraint that only reads the options can take an `IReadOnlyOptionsHolder<TOptions>`. See [Options](../11-extending/05-options.md).
- `Quantifier`, `CollectionMatchOptions`, `CollectionIndexOptions`, `CollectionOrderOptions<TElement>`, `NumberTolerance<TNumber>`, `TimeTolerance`, `SignalerOptions`, `RepeatedCheckOptions` and `ExecutionTimeOptions` are `readonly struct`s, and `StringEqualityOptions` is a sealed immutable class. Start from their `Default`, e.g. `StringEqualityOptions.Default`, instead of a parameterless constructor, and for a date from `TimeTolerance.Days`.
- A method that sets an option returns a new value, so use the value it returns: a discarded call on a struct, e.g. `options.AtLeast(2);`, changes nothing, and the new analyzer warning [aweXpect0009](../08-analyzers.md#awexpect0009) reports it. Mark immutable options of your own with `[ImmutableOptions]` to get the same warning. A repeated option replaces the earlier one.
- The option methods are also extension methods on the holder in `aweXpect.Options`, which replace the value in the holder, e.g. `result.Options.AtLeast(2)`. Import `aweXpect.Options` where you call them: without it, the compiler finds only the user-facing option of the same name and reports `CS0311`.
- `IRelativeToleranceOptions<T>` is removed: `Percent()` calls the new `SetRelativeTolerance(…)` of `IWithinToleranceResult<TResult, TTolerance>`, which a result of your own implements: it makes the tolerance relative and returns the result to continue with.
- `CollectionMatchOptions.SetEquivalenceRelation` is removed; pass the relation to the constructor. `CollectionMatchOptions.ExpectationItem<TItem>` takes an `Action<IThatSubject<TItem>>`.

### Results

- The operators of property results are extension methods on one interface per property type, such as `IIntPropertyResult<TResult>` or `IEnumPropertyResult<TResult, TEnum>`, so a result of your own gets them by implementing the interface. `IDoublePropertyResult<TResult, TToleranceResult>` and `IDecimalPropertyResult<TResult, TToleranceResult>` with `PropertyResult.Double` and `PropertyResult.Decimal` are new, and their `EqualTo` offers a tolerance, e.g. `EqualTo(4.5).Within(0.1)`. The `string` operators are split over `IStringEqualityPropertyResult<TResult>`, `IStringAffixPropertyResult<TResult>` and `IStringOccurrencePropertyResult<TResult>`, which `PropertyResult.String<TType, TThat, TValue>` implements instead of offering them as instance methods. Calls such as `HasMessage().EqualTo(…)` compile unchanged, but have to be compiled again.
- `PropertyComparison<TProperty>.Satisfying(…)` lets an extension add operators of its own to a property result.
- The type parameters of results follow one scheme: `TType` for the subject, `TThat` for the subject wrapper, `TSelf` for the result itself, `TResult` for the result a call continues with, `TCollection` and `TItem` for a collection and its items and `TElement` for the value an option compares, e.g. `CollectionOrderResult<TCollection, TThat, TElement>`. Code that names a result changes where the order or the number of type parameters changed: `PropertyResult.Int<TType, TThat, TValue>`, its siblings and `EnumValueResult<TType, TValue>` start with the subject type, `HasItemResult`, `ObjectHasItemResult`, `ObjectHasItemWithToleranceResult`, `StringHasItemResult` and `StringBlockHasItemResult` take a `TThat` after the collection, and `ContainsKeyResult` and `ContainsKeysResult` have no `TKey`.
- The result constructors of aweXpect and of the extension packages are internal, as are `ProblemDetailsOptions` and the changeable filter interfaces of aweXpect.Reflection; the results of `aweXpect.Core` stay public as extension points. `ObjectEqualityWithToleranceResult` and `StringCollectionMatchResult` moved to Core, and `ToleranceEqualityResult` and `AndOrResult<TThat>` are removed. A result without a value of your own derives from `ExpectationResultWithoutValue<TSelf>`, so that `Because` returns it.
- `ExecutesInResult` and `ExecutesInToleranceResult` take the `ExpectationBuilder` as first constructor argument.
- Results no longer implement `IOptionsProvider<ExpectationBuilder>`; use `EquivalencyExpectations.TryGetBuilder` for `It.Is…` expectations.
- Nested subjects are `IThatSubject<T>`, and nested expectations take an `Action<IThatSubject<T>>`.
- The signaler and event recording expectations extend the nullable `IThat<Signaler?>`, `IThat<Signaler<TParameter>?>` and `IThat<IEventRecording<TSubject>?>`, and their results continue on it, so your own expectations on these subjects extend it as well.

### Constraints and evaluation

- A `ConstraintResult` negates itself in `protected override void NegateInPlace()` instead of `public override ConstraintResult Negate()`, so a negation can no longer return a new result that is then ignored. Negate a result with `.Invert()`, or with the new `.InvertIf(isNegated)`.
- `ExpectationBuilder.WithCancellation` and `WithTimeout` return the builder.
- `ExpectationBuilder.Timeout` only returns the timeouts added with `WithTimeout(…)`. The limits of `ExecutesIn()` and of `Within(…)` after `Throws…()` are kept apart, so that a later limit replaces an earlier one, and still apply when the expectation is evaluated.
- An extension that waits with a timeout of its own cancels with `IEvaluationContext.CancelAfter(…)`, so that the timeout runs on the clock of the evaluation.
- The quantified collection constraints moved to `aweXpect.Core.Extending`, and the occurrence count of `Contains` is available there as `AddStringOccurrenceConstraint`.
- `[GenerateCollectionExpectation]` also generates the expectation directly on a collection with `CollectionOverload` and `NegatedName`, and a predicate parameter marked with `[ExpectationOptions]` adds the string options.
- The `NotSupported(what, remedy)` of `ReflectionFallback` is public, so that an extension throws the same error as the built-in expectations when reflection is switched off. aweXpect.Json and aweXpect.Reflection use it too, so their wording of this error changed slightly. The runtime switch `aweXpect.ReflectionFallback.IsSupported` and the `AweXpectReflectionFallback` property keep their names.

### Test framework adapter

`ITestFrameworkAdapter` has the new members `TryGetCurrentTest`, `TryGetCancellationToken` and `TryAddAttachment`, so an adapter of your own has to implement them; each returns `false` when the test framework does not support it. See [the running test](../11-extending/11-initialization.md#the-running-test).

## New in v4

These additions need no change in your code.

- **Dates** gain `HasDayOfWeek(…)` on `DateTime`, `DateTimeOffset` and `DateOnly`.
- **Dictionaries** with string values offer the string match types, e.g. `ContainsValue("prefix*").AsWildcard()`.
- **Exceptions** keep their type after `HasInner(Type)` and `DoesNotHaveInner(Type)`, so `.And.HasParamName(…)` follows.
- **String properties**: `HasMessage().Containing(…)`, `WithMessage().Containing(…)`, `HasParamName().Containing(…)` and the `Uri` property checks of aweXpect.Web offer the quantifiers and match types of `Contains`, e.g. `HasMessage().Containing("retry").AtLeast(2)`.
- **Delegates** keep their result type after `Because`, `WithTimeout` and `WithCancellation`, so `.Because(…).WhoseResult` compiles.
- **Nested subjects** such as `Throws().Which`, `WhoseResult` or `HasSingle().Which` offer `Is<TType>()` and the other type checks.
- **Combinations** from `Expect.ThatAll` and `Expect.ThatAny` offer `Because`, `WithTimeout` and `WithCancellation`, and a timeout is one deadline for the whole combination.
- **Not nullable results**: expectations that a `null` subject can never satisfy return the subject as not nullable when awaited in all packages, as they already did in aweXpect. This covers the Xml, Json and Snapshots expectations except `MatchesPublicApiSnapshot`, the status code expectations of aweXpect.Web and the collection expectations of aweXpect.Reflection. You may be able to remove `!` operators after `await`.

The extension packages:

- **aweXpect.Json**: `IsString()` and `IsNumber()` offer the operators of a property on their result, so that the value is compared in the same sentence, e.g. `IsString().StartingWith("Let").IgnoringCase()` or `IsNumber().EqualTo(4.5).Within(0.1)`. `IsNotValidJson()` is new.
- **aweXpect.Reflection**: every check has its negation as an expectation and as a filter, e.g. `DoesNotReturn`, `DoesNotHaveParameter` and `WhichDoNotReturn`, and the member-type filters gain `WhichAreNotOfType` and `WhichAreNotOfExactType`. Collections of types offer `DependOn<T>()` and `DoNotDependOn<T>()`, and parameters are filtered by a predicate with `Matching(p => …)`. The name filters use the string options of aweXpect, so they chain in any order and offer every option, e.g. `WithName("x").IgnoringCase().AsPrefix()`. The version components (`HasVersion().WithMajor`) offer all comparisons of an `int` property, including `Between(…).And(…)`.
- **aweXpect.Web**: requests gain `HasContentType(…)` and `DoesNotHaveContentType(…)`, and responses gain `DoesNotHaveStatusCode(…)`. `HasScheme`, `HasHost`, `HasPath` and `HasFragment` on a `Uri` take the expected value directly, and `HasRequestUri(uri => …)` applies the `Uri` expectations to the request URI. `WithValue(…)` of headers and query parameters offers the match types, e.g. `WithValue("application/*").AsWildcard()`, like the type, title, detail and instance of `HasProblemDetailsContent(expected)`.
- **aweXpect.Xml**: `HasAttribute(name)` and the new `HasValue()` compare the value in the same sentence, e.g. `HasAttribute("id").EqualTo("7")` or `HasValue().StartingWith("Let")`. `IsNotValidXml()` is new.
