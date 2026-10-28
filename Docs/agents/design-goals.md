# Design goals

These goals apply to every Testably repository. They guide agents and contributors when a change has more than one reasonable solution. The goals of the packages at the end refine them.

Correctness is a hard requirement. Every other goal is a strong default: follow it, and when a change has to break one, say so in the pull request and explain why.

## Decision test

Before proposing a change, ask: does it keep the library correct, and does it make it faster, more consistent, easier to extend or more pleasant to use? If the answer is no, reconsider the change.

## Goals

When goals conflict, the earlier one wins.

1. **Correct, with clear failures (must).** Behaviour is exact and covered by tests. A failure states what was expected and what was found, and tests pin the complete message.
2. **Stable and compatible.** Follow semantic versioning: breaking changes only in a major version, on purpose and documented. Public API changes show up in the API snapshots and are reviewed. Keep dependencies minimal and their version floors low, and support the declared target frameworks.
3. **AOT and trimming friendly.** No unguarded reflection: prefer source generators, otherwise put reflection behind a feature guard with trimming annotations. This covers member reflection (`GetProperties`, `GetMethods`, `GetCustomAttributes`, reading or invoking a member) and `MakeGenericType` or `MakeGenericMethod`. Queries on the shape of a type that the code already holds need no guard: `IsGenericType`, `GetGenericTypeDefinition` and `GetGenericArguments` are trimming-safe. `GetInterfaces` is not exact, as the trimmer or the AOT compiler can remove the implementation of an interface that the program never uses, so call it unguarded only where a missed interface degrades to the behaviour for a type without it, like a dictionary that is formatted as a sequence of pairs, and say so in its suppression, as `FindGenericInterface` in `aweXpect.Core` does; a new suppression needs the maintainer's approval. When the interface is what an expectation checks, guard the lookup; when a missed interface would silently change a comparison, root the interfaces, like the generated registrations do for equivalency. A change that touches reflection runs the AOT smoke tests. `aweXpect.Reflection` refines this goal, see its package goals.
4. **A consistent, discoverable and pleasant API.** Extension methods exist only on the types they apply to, so IntelliSense shows what makes sense. Naming, option chaining and null handling follow the existing patterns (see the repository-specific rules). Every public member has XML documentation. Prefer minimal ceremony for the user: sensible defaults, no boilerplate, failure messages that read well.
5. **Fast, especially on the happy path.** Succeeding calls are the common case. Avoid allocations in hot paths, and back every performance claim or change with a benchmark before and after.
6. **Extensible.** Third parties build on the same extension points the library uses itself. New features use the public extension surface instead of internals, and the core stays small and stable.
7. **Errors at compile time rather than at run time.** Prefer analyzers, source generators and types that make misuse a compiler error over runtime checks.
8. **Documented and pleasant to read.** A public feature is not done until it is documented on the docs site, with examples that are tested where the repository can test them. The docs are rendered with Docusaurus: use its features where they help the reader, such as admonitions (`:::note`, `:::tip`, `:::warning`) for remarks, tabs for alternatives (for example synchronous and asynchronous usage, or one tab per test framework) and collapsible `<details>` for long output, and follow the style of the existing pages. Examples follow the music theme of the aweXpect documentation (albums, tracks, artists, playlists) instead of placeholder names like `Foo` or `Bar`.

## Code

- **Modern C#.** Use current language features and APIs where the target frameworks allow them, with polyfills for netstandard2.0 and net48 instead of avoiding a feature. Check that the oldest target framework still compiles.
- **Async.**
  - Use `ValueTask` for operations on the happy path that often complete synchronously, like the evaluation of a constraint. An API that only runs while a failure is built, like `ResultContext.GetContent` or the content processors of `aweXpect.Web`, may return `Task`, as it does not run when the expectation succeeds.
  - Pass the `CancellationToken` through every async path.
  - Do not use `ConfigureAwait(false)` in library code or in generated code. Expectations run user code after their own awaits (predicates, mappings, async delegates and enumerables, custom comparers and formatters), and this code must run on the synchronization context of the caller, for example on the UI thread of a WPF or WinForms test. A synchronous API boundary that blocks detaches from the context of the caller instead, as `Synchronously` does. The analyzers and code fixers run in Roslyn and not in a test, so they keep `ConfigureAwait(false)`.
  - Do not block on async code (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`), except at a deliberate synchronous API boundary, and complete synchronously there where possible.
  - Do not use `Task.Run` to make code look asynchronous; use it only where the real counterpart does work in the background.
- **Avoid.**
  - Catching exceptions without handling or rethrowing them.
  - LINQ, closures and boxing in hot paths; use loops and cached delegates there.
  - String concatenation in loops; use a `StringBuilder`.

## Before you commit

Run the checks that match the change:

- **Any code change:** the tests of the affected projects, on the oldest and the newest target framework.
- **Public API:** the API snapshot tests; review and accept intentional changes, never commit pending snapshots.
- **Reflection, trimming or AOT:** the AOT smoke tests.
- **Hot path or a performance claim:** a benchmark before and after.
- **New public feature:** documentation and its examples.
- **Failure messages:** tests that pin the complete message.
- **Static analysis:** no new Sonar or analyzer findings. Avoid the common ones: high cognitive complexity, duplicated string literals, unused members and parameters, methods that could be static, long parameter lists. Do not suppress a finding without asking the maintainer first.

## Reviews

Verify every review finding against the code before changing anything. If you disagree, push back once with concrete reasoning; if the reviewer or the maintainer confirms the finding, implement it.

## Goals of the packages

### aweXpect and aweXpect.Core

1. **Readable both ways.** A negatable expectation has its natural negation next to it (same file, same constraint), and a negated expectation never passes on a null subject or an undecided outcome.
2. **One meaning per option word.** `Ignoring…` relaxes a check, `As…` changes the match type, `Using` supplies a comparer, `Within` sets a tolerance or a timeout, `Including…` and `Limiting…` widen or bound the scope of a comparison instead of relaxing it; options chain in any order.
3. **Messages follow the message conventions.** The expectation reads in the present tense, the result starts with "it" in the past tense, values go through the formatter, and tests pin the complete message. A property result may start with the property instead of "it", like "but message did not contain …" or "but message did throw …", as long as all results of one property expectation, like the forms of `HasMessage`, use the same style.
4. **Subjects keep their type.** Reference subjects are nullable, scalar value types get a nullable twin (collection structs like `ImmutableArray<T>` do not; a default `ImmutableArray<T>` already counts as a `null` collection), methods on base types are generic over the subject so chaining keeps the concrete type; property checks offer a direct overload and a property result, collection quantifiers read as plural sentences; every parameter or property that hands out a subject for further expectations (nested `Action<…>` expectations, `Which`, `Whose…`) uses `IThatSubject<T>`, so that `Is<TType>()` and the other type checks are available everywhere. The item expectations of collections (`Contains`, `HasItem`, …) extend `IThat<IEnumerable<TItem>?>` instead, as C# cannot infer `TItem` from a constraint like `where TCollection : IEnumerable<TItem>`, while an expectation that needs no item type, like `IsEmpty`, stays generic over the collection type. A collection item uses the inferred `TItem`, so that its nullability flows from the collection type like in a `foreach`; `T?` only where aweXpect itself can yield `null`, e.g. a member of a `null` parent.
5. **Uniform argument handling.** Arguments are validated eagerly, expectations are marked `[GuaranteesNotNull]`, and caller expressions are captured where they make the message readable.
6. **Trimming-safe by construction.** Member reflection only through the type metadata registry, and member metadata annotations wherever a value reaches equivalency.
7. **Predictable layout.** One method family per file in its `That<Type>` partial class, results and options in their own namespaces, generated families never written by hand. The match types of Core are an accepted exception: the abstract bases that extensions derive from (`ObjectMatchType<TElement>`, `StringMatchType` and their matchers) are in `aweXpect.Core`, while the ready-made ones stay next to their feature, `ObjectToleranceMatchType<TElement, TTolerance>` in `aweXpect.Options` and `EquivalencyMatchType<TElement>` in `aweXpect.Equivalency`, as moving them would break every extension for little gain.
8. **The extension surface is API too.** Results, options, `ExpectationBuilder` and the other types extension authors call or name follow the same rules as the user-facing methods: one name per concept, the option rules below, and the same type-parameter names and order across results. An inconsistency there is fixed in the next major version like any other API break, not kept because users never call it directly. A result starts with `<TType, TThat, …>`, or `<TCollection, TThat, …>` on a collection, and one without a subject type, like `IIntPropertyResult<TResult>`, starts with `TResult` or `TSelf`; other type parameters follow, like `TTolerance` in `IWithinToleranceResult<TResult, TTolerance>`. A type parameter has the same meaning everywhere:

   | Name                   | Meaning                                                                                              |
   |------------------------|------------------------------------------------------------------------------------------------------|
   | `TSelf`                | the result itself, which an option returns for chaining                                              |
   | `TResult`              | the result a call continues with, e.g. after `Between(…)` or `AtLeast()`                             |
   | `TType`, `TThat`       | the type of the subject, and the `IThat<…>` the result continues on                                  |
   | `TValue`               | a value other than the subject, e.g. the value of the source before it was narrowed to `TType`; in the constraint layer (`IValueConstraint<TValue>`, `AddConstraint<TValue>`) the value a constraint receives, which is not always the subject, e.g. a member value |
   | `TCollection`, `TItem` | the type of a collection subject, and the type of its items                                          |
   | `TElement`             | the value an equality or order option compares, e.g. in `ObjectMatchType<TElement>`, unless it is `TItem` |
9. **Result constructors in `aweXpect` are internal.** As with the plumbing constructors of the extension packages (their rule 5), only the expectations of `aweXpect` can build its results meaningfully, and every public constructor is permanent API. The results of `aweXpect.Core` stay public as extension points; a result of `aweXpect` that an extension needs is promoted to Core when there is a real need.
10. **Quantified items are Core interfaces.** The expectations on the items that `All()`, `AtLeast(…)` and the other quantifiers select are extension methods on the Core interfaces `IEnumerableItems<TItem>`, `IAsyncEnumerableItems<TItem>`, `IStructEnumerableItems<TCollection, TItem>` or `INonGenericEnumerableItems<TCollection>`, so that aweXpect, the generator and extension packages add them the same way; the string-specific ones are overloads on `IEnumerableItems<string?>`. These interfaces are covariant in `TItem`, so that an expectation on the items of a base type or of a nullable type applies to the items of a derived or not nullable type. A member whose type argument cannot be inferred, like `Are<TType>()`, names the item type in its result and is therefore a member of the invariant `IQuantifiedEnumerableItems<TItem>` or `IQuantifiedAsyncEnumerableItems<TItem>` that the quantifiers return; the classes behind them are internal. The concept is called "items" everywhere, with `TItem` for the item and `TCollection` for the collection.

The options of Core are immutable values that belong to a single expectation, unlike the package options configured with a `Func<Options, Options>` (rule 4 of the extension packages): the expectation holds one `OptionsHolder<…>`, a plain reference to the current value, which it shares with its constraint and its result, and the result offers it through `IOptionsProvider<…>`; the constraint reads the value when it evaluates, never when it is built. When only the constraint and the result share the holder, the constraint is the `IOptionsHolderSource<…>` of the result instead and creates the holder only when an option is set, comparing with the options it started with until then; it is added with `AddConstraint(state, constraintBuilder, out constraint)`. Each options type starts from its `Default`, and the small ones are `readonly struct`s. A method that sets a Core option returns a new value, and the extension method of the same name on the holder in `aweXpect.Options` replaces the value in the holder and returns the holder, so that calls chain. A repeated option replaces the earlier one and an option set to `false` equals not set, while options that set the same thing in different ways throw, e.g. `AtMost cannot be combined with AtLeast.`; a strategy hook that competes with other options takes the `optionName` of the custom option that uses it, which the exception names, while a hook without a competing option, like `SetComparer`, takes none. Strategy hooks are named `Set<Thing>`, like `SetMatchType`, `SetMatch` or `SetComparer`. A hook that combines with the earlier calls instead of replacing them, like the predicates of an event or signal filter, is named `Add<Thing>`, like `AddPredicate`. A method that mirrors a user-facing option has the name of its extension method, like `Within` on `TimeTolerance` or `SignalerOptions`, or `Never` on `Quantifier`.

### Extension packages

These goals hold for every extension package:

1. **Feels like core aweXpect.** Naming, negation, result types, argument checks and messages follow the goals above, and the layout mirrors `aweXpect`.
   - Nested expectations are an `Action<IThatSubject<…>> expectations` overload of the plain verb, never a `Func` whose return value is ignored; a suffix like `HasItemThat` only disambiguates from existing lambda overloads.
   - When `null` fails, the expectation is marked `[GuaranteesNotNull]` and its result has the non-null subject type, also for a nullable value-type subject (like `IsTrue` on `bool?`), and keeps the `IThat<…>` of the subject for chaining.
   - Variable arguments are a `params T[]`, never a params collection like `params IEnumerable<T>`, as consumers on older language versions (C# 7.3 on net48, C# 12 on net8.0) cannot pass single values to it; an overload for existing collections takes `IEnumerable<T>` without `params`.
   - A string name that is looked up in a document or message (a header, query parameter, XML element or attribute, JSON property) is a parameter `name`; compared values and typed keys of a collection are `expected` or `unexpected`.
2. **Core is enough.** An extension builds only on the public API of `aweXpect.Core`, like a third-party extension would. If it needs more, the gap is closed in Core, in the same pull request. A type test or other workaround against Core types is such a gap.
3. **Unanswerable means failed both ways.** A `null` subject, a missing element or an error while reading the subject fails the expectation and its negation instead of throwing.
4. **Options are immutable and visible.** A package has one sealed options type (one that cannot be derived from outside, if it needs a derived type) with internal state, a copy constructor and option methods that each return a copy, passed as `Func<XmlOptions, XmlOptions>? options = null` like `XmlOptions`; no public mutable state. Package option types live in `aweXpect.<Package>.Options`, their extension classes in the namespace `aweXpect` itself, not in `aweXpect.<Package>`, so that `o => o.IgnoringArrayOrder()` needs no extra `using`, as a call site finds extension methods only through its own namespaces and `using` directives; the discarded-option analyzer aweXpect0009 covers these namespaces. This rule is about these package options only: the result-scoped options of Core (`StringEqualityOptions`, `CollectionMatchOptions`, `Quantifier` and the like) are immutable values as well, but belong to a single expectation and are reached through the `OptionsHolder<…>` of `IOptionsProvider<…>` on a result, whose fluent methods replace the value in the holder. A result that can be stored and continued, like a filter of `aweXpect.Reflection`, implements `ICopyOnOption<…>`, so that an option returns a new result instead of changing the holder. The options in effect are listed in the failure. `IgnoringMember` in Json and Xml takes paths and `IgnoringMembers` a predicate on the path, like equivalency does.
5. **Plumbing constructors stay internal.** A result or filter class whose constructor takes plumbing (internal-purpose options, selector functions, the inner collection it decorates) or relies on state stored elsewhere gets an internal constructor, as nobody outside the package can build it meaningfully and every public constructor is permanent API; the results of `aweXpect.Core` stay public as extension points.
6. **Entry points in `aweXpect`, everything else in `aweXpect.<Package>`.** The `That<Subject>` classes and entry extensions such as `AsJson()` need no extra `using`; results go into `aweXpect.<Package>.Results`, all other public types into `aweXpect.<Package>` or below, so that they cannot clash with Core or another package.

#### aweXpect.Json

1. **Compare JSON semantically.** Numbers by value, member order irrelevant; whatever a default ignores is visible in the failure.
2. **Serialization fails loudly without reflection.** Serializing caller values respects the reflection switch and throws a clear error when it is off; prefer reflection-free paths.

#### aweXpect.Web

1. **Matching follows the RFCs.** Header names are case-insensitive, URI paths case-sensitive, IPv4-mapped IPv6 addresses handled explicitly.
2. **HTTP failures show the request and response** in one consistent format, never formatted inline.
3. **Content is read asynchronously without consuming the stream**, never blocking.
4. **Clear option words.** `With…` narrows, `Whose…` nests expectations, `Ignoring…` only modifies a comparison.
5. **JSON handling stays reflection-free.**

#### aweXpect.Xml

1. **Parsing is always secure.** No DTDs and no external resolution.
2. **Invalid expected XML is rejected when the expectation is written**, so a negation can still fail.
3. **Equivalence follows XML semantics.** Prefixes, attribute order, comments and insignificant whitespace are ignored; `xml:space` and CDATA are honoured.
4. **Element and document subjects stay in parity.**

#### aweXpect.Reflection

This package exists to inspect types through reflection, so it replaces goal 3 (AOT and trimming friendly) with **trimming friendly by failing loudly**: under trimming an expectation answers from preserved metadata or throws a clear error instead of avoiding reflection, and never silently checks less.

1. **Three layers, one vocabulary.** Sources, filters and expectations read consistently, and singular and plural forms stay in parity.
2. **Negations mean what they say.** Every positive has its negation where it makes sense, and a plural negation means "none are", never an inverted "not all".
3. **Filters stay lazy, immutable and chainable**, and options never break the chain.
4. **Option words follow core.** `Ignoring…` for string options, `Excluding…` to narrow a scope. `Within` also names a namespace scope, here and in `aweXpect.Snapshots.PublicApi`, besides the tolerance or timeout of core goal 2. `aweXpect.Snapshots.PublicApi` groups its options in selectors and names what it leaves out of the written API with `Ignoring`, like `o.Ignoring.Type<T>()` or `o.Ignoring.Attribute<T>()`, instead of `Excluding…`, as one selector covers everything left out, from types and namespaces to attributes and compiler-generated members.
5. **Messages read naturally** for singular and plural subjects.

#### aweXpect.Snapshots

1. **Stable identity.** Snapshot ids and file names are the same on every machine, culture and run; changing the on-disk format is a breaking change.
2. **Writing is explicit.** A passing or negated expectation never writes a snapshot, accepting is a deliberate choice, and an accepted change still fails the run. `Accepting(...)` is always the last option of an expectation.
3. **A small, shared rule grammar.** Selection, filter, one action, the same for per-expectation and global rules.
4. **Messages name the API that fixes the problem.**
5. **AOT for the core packages.** `aweXpect.Snapshots.PublicApi` opts out, as Roslyn cannot be trimmed, but must not pull reflection into `aweXpect.Snapshots`.
