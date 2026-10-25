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

| v3                                                       | v4                                                         |
|----------------------------------------------------------|------------------------------------------------------------|
| `IsEquivalentTo(…, o => o.Ignoring(predicate))`          | `IsEquivalentTo(…, o => o.IgnoringMembers(predicate))`     |
| `IsValidJson().Which(j => …)`                            | `IsValidJson(j => …)`                                      |
| `IsValidXml().Which(root => …)`                          | `IsValidXml(root => …)`                                    |
| `HasStatusCode().DifferentTo(…)`                         | `HasStatusCode().NotEqualTo(…)`                            |
| `WithJsonOptions(…)` in the Json options                 | `WithDocumentOptions(…)`                                   |
| `o => o with { … }` on `JsonOptions`                     | the option methods, e.g. `o => o.IgnoringArrayOrder()`     |

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
- `HasStatusCode().NotEqualTo(404)` reads `does not have status code 404 NotFound`.

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

### Customization

- The collection-valued settings, such as `ExcludedAssemblyPrefixes`, `ExcludedAttributeTypes` and the content
  processors of aweXpect.Web, are `IReadOnlyList<T>` instead of arrays.
- The excluded assembly prefixes moved to `Customize.aweXpect.Settings().ExcludedAssemblyPrefixes`. A prefix ending
  with a dot, such as `MyCompany.`, now matches in Core as well.

### Time system

`WithTimeSystem(…)` is marked experimental with the diagnostic `aweXpect5001`. Suppress it in test projects that use
a virtual time system.

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

- `Matches`, `IsValidJsonMatching` and the nested expectations of `IsObject` and `IsArray` report additional array
  elements. Add `o => o.IgnoringAdditionalArrayElements()` to allow them.
- `IsObject()` offers `WhoseProperties`, like `WhoseElements` on arrays.
- `JsonOptions` is immutable like `XmlOptions` and configured only through its methods.
- The nested expectations of `IsObject` and `IsArray` take an `Action`: a block lambda must drop its `return`
  statement.

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
- The expectations on all items, such as `AreSealed()`, use the quantifier wording of aweXpect
  (`all are sealed, but only 2 of 5 were`), work with quantifiers (`AtLeast(2).AreSealed()`), and a `null` item fails
  them and their negation.
- The version components (`HasVersion().WithMajor`) offer all comparisons of an `int` property, including
  `Between(…).And(…)`.
- `Customize.aweXpect.Reflection()` and its settings need `using aweXpect.Reflection;`.

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

### aweXpect.Web

- `HasHeader(…).WithValue(…)` and `HasQueryParameter(…).WithValue(…)` return a `StringEqualityTypeResult` and offer
  the match types, e.g. `WithValue("application/*").AsWildcard()`, like the type, title, detail and instance of
  `HasProblemDetailsContent`. This only affects code that names the result type.

### Namespaces

Results of the extension packages are in `aweXpect.<Package>.Results` and their other public types in
`aweXpect.<Package>`. Add the matching `using` where you name such a type.

## Extensions

If you wrote your own expectations on `aweXpect.Core`, the compiler points out most of the changes. The important ones:

- Match types have one lifecycle: `StringMatchType` and `ObjectMatchType<TSubject>` create a matcher per evaluation
  (`StringMatcher`, `ObjectMatcher<TSubject>`) that returns a `MatchResult`. They replace `IStringMatchType` and
  `IObjectMatchType`. See [Options](../11-extending/03-options-and-match-types.md).
- The operators of property results are extension methods on one interface per property type, such as
  `IIntPropertyResult<TResult>` or `IEnumPropertyResult<TEnum, TResult>`, so a result of your own gets them by
  implementing the interface.
- The expectations on quantified items extend the Core interfaces `IEnumerableItems<TItem>`,
  `IAsyncEnumerableItems<TItem>`, `IStructEnumerableItems<TCollection, TItem>` and
  `INonGenericEnumerableItems<TCollection>`, which replace `IEnumerableElements<TItem>` and its siblings.
- Every method that sets a Core option returns the options object, each option can be set only once, and methods
  that mirror a user-facing option take its name, e.g. `Within` instead of `SetTolerance`.
- `ConstraintResult` creates a result that fails both ways with `FailureBothWays` instead of `Fail`.
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
- The type parameters of results follow one scheme: `TType` for the subject, `TThat` for the subject wrapper, and
  `TResult` for the type returned after an option, e.g. `CollectionOrderResult<TType, TThat, TMember>`.
- `[GenerateCollectionExpectation]` also generates the expectation directly on a collection with `CollectionOverload`
  and `NegatedName`.
