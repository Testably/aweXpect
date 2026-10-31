# Documentation

Rules for the user docs in `Docs/pages` and `Docs/extensions`. Design goal 8 in [design goals](design-goals.md)
(Docusaurus features, music theme) and the editing rules in `AGENTS.md` (CRLF, no em-dashes) apply as well.

## Audience

- `Docs/pages` (except `11-extending`) and `Docs/extensions/<Package>`: people who write tests.
- `Docs/pages/11-extending`: authors of extension packages.
- `Source/<Package>/README.md` is the NuGet README of an extension package and repeats its
  `Docs/extensions/<Package>/00-index.md` in plain Markdown, with absolute links to the docs; keep both in sync.

Write for the task of the reader: what a feature does, how to use it and what the failure looks like. Internals belong
in XML docs and `Docs/agents`, not in the user docs.

## Show, hide or remove

| Content                                                                                  | Where                                 |
|------------------------------------------------------------------------------------------|---------------------------------------|
| What an expectation or option does, a sample, its options, a failure message that helps  | visible                               |
| A warning that prevents a silent mistake, e.g. an expectation that checks the wrong thing | visible (`:::warning`)               |
| A rule an extension author has to implement                                              | visible                               |
| Internal mechanics, rare target framework quirks, precise corner cases                   | `<details>` at the end of the section |
| Misuse of arguments (`null`, empty or negative values that throw)                        | remove                                |
| A restriction the compiler or an analyzer already enforces                               | remove                                |
| The general option rules (any order, a repeated option replaces the earlier one, `false` behaves as not set, conflicting options throw) | only on Concepts > Options |

- One `<details>` block per section with a descriptive `<summary>`, blank lines after the summary and before
  `</details>`, never nested and never in the middle of a paragraph.
- Don't hide the content of a page that exists only for that content: a page of its own shows it.
- Put a fact where readers look for it, e.g. how `NaN` compares in the `NaN` section, not under the ordering.
- State a fact once and link to it: the supported test frameworks are on Setup > Test frameworks, Native AOT is on
  its own page, the option rules are on Concepts > Options.

## Reference pages

- Skeleton: the heading, one sentence "Describes the possible expectations for …", an overview table
  `Expectation | Negated | Summary` with a link per row, then one section per expectation, starting with
  "You can verify that …" and a sample with positive and negated calls.
- Every expectation of the page has a row in the overview table. Leave the Negated cell empty when there is no
  negation; for a quantifier write e.g. "use `None()`".
- Where expectations overlap, e.g. `Contains(predicate)`, `Any().Satisfy(…)` and `HasItem(…)`, say which one to use
  and why.
- An extension package index has: intro, Installation (`dotnet add package`, the needed `using`, and that `aweXpect`
  is referenced as well), At a glance, a failure message, the overview tables, a Documentation page list and, when the
  package has settings, a Customization table. Its settings page is called "Customization".

## Samples

- Music domain (albums, tracks, artists, playlists), TUnit for test code (`[Test]`, `[Arguments(…)]`), except where
  a test framework is the subject of the sample.
- Occurrence counts as plain numbers: `AtLeast(2)`, `Between(1).And(4)`; `2.Times()` is optional.
- Durations may use `aweXpect.Chronology` (`2.Seconds()`) with the comment `// from the aweXpect.Chronology package`.
- `csharp` blocks in `Docs/pages` are compiled (see [testing](testing.md#documentation-tests)); mark a block that
  cannot compile `csharp no-compile`. Blocks in `Docs/extensions` are not compiled.
- When a test project already has the sample as tested code, e.g. in an `Examples` folder, show that code instead of a
  version of your own, and change both together.
- Check every statement and sample against `Source/` and the tests, also failure messages and default values. Don't
  write them from memory.

## Links and URLs

- The URL of a page is its path without the numeric prefixes, e.g. `04-values/02-number.md` is
  `/aweXpect/values/number`. A sub-folder with a `_category_.json` is a sidebar group.
- Never move the Analyzers page or rename its headings: the help links of the analyzers and generators point to
  `https://docs.testably.org/aweXpect/analyzers#<rule>` and to anchors like `#expectation-generator`.
- Prefer moves that keep the URL. When a page moves or a heading changes, update every link to it: relative links,
  site links (`/aweXpect/…`), absolute links (`https://docs.testably.org/aweXpect/…`) in `Docs`, `README.md` and
  `Source`. Add a redirect for the old URL to `docusaurus.config.ts` in `Testably/Testably.Site`, deployed together
  with the move, as its build fails on a redirect to a missing page.
- `Docs/extensions` links core pages with absolute URLs, as its pages are also published on NuGet.
- The site compiles the pages as MDX: no custom heading ids (`{#id}`) and no other unescaped `{` or `<` in prose. To
  keep an anchor, keep the heading text.

## Checks

- The slow tests of `aweXpect.Docs.Tests` after every change to `Docs/**`.
- After moving pages or headings, build `Testably.Site` with the changed docs (copy them the way its `Pages` target
  does) and fix the broken links and anchors it reports.
