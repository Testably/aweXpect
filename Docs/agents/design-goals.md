# Design goals

These goals apply to every Testably repository. They guide agents and contributors when a change has more than one reasonable solution. The goals of the packages at the end refine them.

Correctness is a hard requirement. Every other goal is a strong default: follow it, and when a change has to break one, say so in the pull request and explain why.

## Decision test

Before proposing a change, ask: does it keep the library correct, and does it make it faster, more consistent, easier to extend or more pleasant to use? If the answer is no, reconsider the change.

## Goals

When goals conflict, the earlier one wins.

1. **Correct, with clear failures (must).** Behaviour is exact and covered by tests. A failure states what was expected and what was found, and tests pin the complete message.
2. **Stable and compatible.** Follow semantic versioning: breaking changes only in a major version, on purpose and documented. Public API changes show up in the API snapshots and are reviewed. Keep dependencies minimal and their version floors low, and support the declared target frameworks.
3. **AOT and trimming friendly.** No unguarded reflection: prefer source generators, otherwise put reflection behind a feature guard with trimming annotations. A change that touches reflection runs the AOT smoke tests. `aweXpect.Reflection` refines this goal, see its package goals.
4. **A consistent, discoverable and pleasant API.** Extension methods exist only on the types they apply to, so IntelliSense shows what makes sense. Naming, option chaining and null handling follow the existing patterns (see the repository-specific rules). Every public member has XML documentation. Prefer minimal ceremony for the user: sensible defaults, no boilerplate, failure messages that read well.
5. **Fast, especially on the happy path.** Succeeding calls are the common case. Avoid allocations in hot paths, and back every performance claim or change with a benchmark before and after.
6. **Extensible.** Third parties build on the same extension points the library uses itself. New features use the public extension surface instead of internals, and the core stays small and stable.
7. **Errors at compile time rather than at run time.** Prefer analyzers, source generators and types that make misuse a compiler error over runtime checks.
8. **Documented and pleasant to read.** A public feature is not done until it is documented on the docs site, with examples that are tested where the repository can test them. The docs are rendered with Docusaurus: use its features where they help the reader, such as admonitions (`:::note`, `:::tip`, `:::warning`) for remarks, tabs for alternatives (for example synchronous and asynchronous usage, or one tab per test framework) and collapsible `<details>` for long output, and follow the style of the existing pages. Examples follow the music theme of the aweXpect documentation (albums, tracks, artists, playlists) instead of placeholder names like `Foo` or `Bar`.

## Code

- **Modern C#.** Use current language features and APIs where the target frameworks allow them, with polyfills for netstandard2.0 and net48 instead of avoiding a feature. Check that the oldest target framework still compiles.
- **Async.**
  - Use `ValueTask` for operations that often complete synchronously.
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
2. **One meaning per option word.** `Ignoring…` relaxes a check, `As…` changes the match type, `Using` supplies a comparer, `Within` sets a tolerance or a timeout; options chain in any order.
3. **Messages follow the message conventions.** The expectation reads in the present tense, the result starts with "it" in the past tense, values go through the formatter, and tests pin the complete message.
4. **Subjects keep their type.** Reference subjects are nullable, value types get a nullable twin, methods on base types are generic over the subject so chaining keeps the concrete type; property checks offer a direct overload and a property result, collection quantifiers read as plural sentences.
5. **Uniform argument handling.** Arguments are validated eagerly, expectations are marked `[GuaranteesNotNull]`, and caller expressions are captured where they make the message readable.
6. **Trimming-safe by construction.** Reflection only through the type metadata registry, and member metadata annotations wherever a value reaches equivalency.
7. **Predictable layout.** One method family per file in its `That<Type>` partial class, results and options in their own namespaces, generated families never written by hand.

### Extension packages

These goals hold for every extension package:

1. **Feels like core aweXpect.** Naming, negation, result types, argument checks and messages follow the goals above, and the layout mirrors `aweXpect`.
   - Nested expectations are an `Action<…> expectations` overload of the plain verb, never a `Func` whose return value is ignored; a suffix like `HasItemThat` only disambiguates from existing lambda overloads.
   - When `null` fails, the expectation is marked `[GuaranteesNotNull]` and its result has the non-null subject type, also for a nullable value-type subject (like `IsTrue` on `bool?`), and keeps the `IThat<…>` of the subject for chaining.
   - A public `params IEnumerable<T>` always has a `params T[]` overload, as consumers on older language versions (C# 7.3 on net48, C# 12 on net8.0) cannot pass single values otherwise.
   - A string name that is looked up in a document or message (a header, query parameter, XML element or attribute, JSON property) is a parameter `name`; compared values and typed keys of a collection are `expected` or `unexpected`.
2. **Core is enough.** An extension builds only on the public API of `aweXpect.Core`, like a third-party extension would. If it needs more, the gap is closed in Core, in the same pull request. A type test or other workaround against Core types, such as testing for `IOptionsProvider<ExpectationBuilder>`, is such a gap.
3. **Unanswerable means failed both ways.** A `null` subject, a missing element or an error while reading the subject fails the expectation and its negation instead of throwing.
4. **Options are immutable and visible.** A package has one sealed options type (one that cannot be derived from outside, if it needs a derived type) with internal state, a copy constructor and option methods that each return a copy, passed as `Func<XmlOptions, XmlOptions>? options = null` like `XmlOptions`; no public mutable state. The options in effect are listed in the failure. `IgnoringMember` in Json and Xml takes paths, like equivalency does.

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
4. **Option words follow core.** `Ignoring…` for string options, `Excluding…` to narrow a scope. `Within` also names a namespace scope, here and in `aweXpect.Snapshots.PublicApi`, besides the tolerance or timeout of core goal 2.
5. **Messages read naturally** for singular and plural subjects.

#### aweXpect.Snapshots

1. **Stable identity.** Snapshot ids and file names are the same on every machine, culture and run; changing the on-disk format is a breaking change.
2. **Writing is explicit.** A passing or negated expectation never writes a snapshot, accepting is a deliberate choice, and an accepted change still fails the run. `Accepting(...)` is always the last option of an expectation.
3. **A small, shared rule grammar.** Selection, filter, one action, the same for per-expectation and global rules.
4. **Messages name the API that fixes the problem.**
5. **AOT for the core packages.** `aweXpect.Snapshots.PublicApi` opts out, as Roslyn cannot be trimmed, but must not pull reflection into `aweXpect.Snapshots`.
