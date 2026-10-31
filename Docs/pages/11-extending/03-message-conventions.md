---
description: Write the expectation and result texts of a constraint so that its failure message reads like those of the built-in expectations.
---

# Message conventions

A failure message of aweXpect reads like one English sentence. The built-in expectations follow the conventions on
this page, so that an extension that follows them as well reads like part of the library.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;
```

</details>

## Shape of a failure message

aweXpect composes the failure message from the subject, the expectation texts and the result texts of the
constraints, and the reason from `Because(…)`:

```text title="Failure message"
Expected that <subject>
<expectation>[, because <reason>],
but <result>
```

Your constraint only writes its expectation text in `AppendNormalExpectation` and `AppendNegatedExpectation`, and its
result text in `AppendNormalResult` and `AppendNegatedResult` (`AppendExpectation` and `AppendResult` when it derives
from `ConstraintResult` directly); the rest is added around them. For the `IsRadioFriendly` expectation from
[your first expectation](./00-index.md#the-constraint), `await That(track).IsRadioFriendly()` fails for
"Hey Jude" with:

```text title="Failure message"
Expected that track
is radio friendly,
but it was 7:11 long
```

The same texts are reused when the expectation is combined or nested, e.g. inside `Whose`, where `it` is replaced by
the name of the member:

```text title="Failure message"
Expected that single
whose ASide is radio friendly,
but ASide was 7:11 long
```

## Expectation text

- Start with the verb in the present tense, in lower case, and end without punctuation: `is radio friendly`,
  `is equal to "Abbey Road"`, `starts with "Abbey"`, `has flag A`.
- Write the negated text with `not`: `is not radio friendly`, `does not start with "Abbey"`.
- For something that happened during the test, like the built-in `Triggered`/`DidNotTrigger` and
  `Signaled`/`DidNotSignal`, name the method in the past tense and its negation `DidNot…`, and write the text in the
  present perfect: `has recorded the callback at least once`, `has never recorded the PropertyChanged event`.
- Append the options after the expectation, e.g. ` ignoring case`, ` using AlbumComparer` or ` in any order`.
- Describe the expected value, not the check: `is radio friendly` instead of `Duration <= 3:00 returns true`.

### Code of the caller in the expectation text

A predicate or another delegate of the caller has no value to format, so name it by its source code, like the built-in
`Satisfies` does (`satisfies x => x.Length > 3`). Let the compiler fill in the source code with
`[CallerArgumentExpression]` on an optional parameter, which the built-in expectations call `doNotPopulateThisValue`,
and pass it to the constraint for its expectation text:

```csharp
using System.Runtime.CompilerServices;
using aweXpect.Results;

public static AndOrResult<Track, IThat<Track?>> HasTitleMatching(
    this IThat<Track?> subject,
    Func<string, bool> predicate,
    [CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new HasTitleMatchingConstraint(it, grammars, predicate, doNotPopulateThisValue)),
        subject);
```

For `HasTitleMatching(title => title.StartsWith("Let"))`, the expectation text can then read
`has a title matching title => title.StartsWith("Let")`. On `netstandard2.0` and `net48`, declare the attribute in your
own package, see [polyfills for older targets](./00-index.md#polyfills-for-older-targets).

## Result text

- Start with the name of the subject, the `it` parameter of the constraint (exposed as `It` by the helper classes),
  followed by a verb in the past tense: `it was 7:11 long`, `it had 3 items`, `ASide was 7:11 long`. The name is
  `it` or the name of a member, so never write the subject yourself.
- Describe what was found instead, without repeating the expectation. A short elliptical result is normal:
  `it was`, `it did`, `it was not`.
- Format every value with `Formatter.Format`, as described in [formatting](./04-formatting.md).
- Add a detail after a comma: `, which differs at index 12`, `, which differs by 2`.
- Put longer information, such as the items of a collection, in a context below the message instead (see
  [contexts](#contexts)).
- When a name was not found, suggest the one that was most likely meant with `ClosestMatch.AppendHint` from
  `aweXpect.Core.Extending`. It appends ` (did you mean "PlayCount"?)` only when a single candidate is close enough:

  ```csharp
  private static void AppendMissingKey(StringBuilder stringBuilder, string it, string expected, string[] keys)
  {
      stringBuilder.Append(it).Append(" did not contain key ");
      Formatter.Format(stringBuilder, expected);
      ClosestMatch.AppendHint(stringBuilder, expected, keys);
  }
  ```

  `ClosestMatch.Find` returns that candidate, or `null`, for your own wording. When you find the suggestion before
  the message is written, e.g. while the comparison runs, append the same hint for it with
  `ClosestMatch.AppendHint(stringBuilder, suggestion)`, which appends nothing for `null`.

Some results are written for you:

| Situation                   | Result text                                                             | Written by                                                              |
|-----------------------------|-------------------------------------------------------------------------|-------------------------------------------------------------------------|
| a `null` subject            | `it was <null>`                                                         | `ConstraintResult.WithNotNullValue<T>`                                  |
| the evaluation was canceled | `it could not be verified, because the evaluation was already canceled` | `AppendCanceledResult`, the default of `AppendUndecidedResult`          |
| code of the caller threw    | `the predicate did throw an InvalidOperationException`                  | [`UserCode.Invoke`](./02-constraints-and-results.md#code-of-the-caller) |

## Vocabulary

Name the expectation methods like the built-in ones, so that the whole chain reads like a sentence:

| Pattern                                    | Use it for                                                     | Examples                                            |
|--------------------------------------------|----------------------------------------------------------------|-----------------------------------------------------|
| `Is…`, `IsNot…`                            | a state or a comparison of the subject                         | `IsEmpty`, `IsNotEqualTo`, `IsRadioFriendly`        |
| `Has…`                                     | a property of the subject, optionally with a comparison        | `HasLength(3)`, `HasCount().GreaterThan(2)`         |
| `DoesNot…`                                 | the negation of a verb                                         | `DoesNotContain`, `DoesNotStartWith`                |
| past tense, `DidNot…`                      | something that happened during the test                        | `Triggered`, `DidNotTrigger`, `Signaled`            |
| `With…`                                    | a property of the result of the previous expectation           | `Throws<T>().WithMessage(…)`                        |
| `Which`, `Whose`                           | continuing with a new subject, or with a member of the subject | `HasSingle().Which`, `Whose(x => x.Title, …)`       |
| `Ignoring…`, `Using`, `Within`, `In…Order` | options of the previous expectation                            | `IgnoringCase()`, `Using(comparer)`, `InAnyOrder()` |

`With…` continues the sentence of the previous expectation, "throws a `CustomException` with message …", while `Has…`
starts a new sentence about the subject. Offer both when your subject can appear in both positions, like the
[exception expectations](../06-behavior/01-delegates.md#exception-details) do.

## Grammar

The `grammars` a constraint receives tell it how its texts are used in the sentence. They are an
`ExpectationGrammars` flags enum:

| Flag         | Set when                                                                                                      |
|--------------|---------------------------------------------------------------------------------------------------------------|
| `Negated`    | the expectation is negated; the helper classes then call `AppendNegatedExpectation` and `AppendNegatedResult` |
| `Plural`     | the subject of the sentence is plural, e.g. `whose Tracks are radio friendly for all items`                   |
| `Nested`     | the expectation continues the sentence of another one, e.g. for a member or an item                           |
| `Active`     | the expectation continues a `With…` clause, which drops the verb, e.g. `with message equal to "Yesterday"`    |
| `Introduced` | the subject was already introduced by a connector like the `that` of `has item that`                          |

`ExpectationGrammarsExtensions` checks them with `IsNegated()`, `IsNested()`, `IsPlural()`, `IsActive()` or
`IsIntroduced()`, and toggles the negation with `Negate()`. Use the plural form of the verb in the expectation text
when the grammars are plural. In the result text, `SubjectVerb(It, singular, plural)` picks the verb, as the singular
form is required as long as the subject is the pronoun `it`:

```csharp
private sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IValueConstraint<Track?>
{
    public ConstraintResult IsMetBy(Track? actual)
    {
        Actual = actual;
        Outcome = actual?.Duration <= TimeSpan.FromMinutes(3) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(Grammars.IsPlural() ? "are radio friendly" : "is radio friendly");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
        Formatter.Format(stringBuilder, Actual?.Duration);
        stringBuilder.Append(" long");
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(Grammars.IsPlural() ? "are not radio friendly" : "is not radio friendly");
}
```

<details>
<summary>The Active flag in string comparisons</summary>

`Active` has the opposite meaning for the expectation text of a string comparison (`StringEqualityOptions.GetExpectation`):
there it asks for the text with the verb (`is equal to "Yesterday"`), and without it the text starts with the
comparison (`equal to "Yesterday"`). Core writes both from the `MatchPhrase` of the match type.

</details>

## Contexts

Information that does not fit into one sentence, such as the items of a collection or the expected and the actual
value of a long comparison, belongs in a context. A context is shown below the message with its title, e.g.
`Collection:`, `Expected:`, `Actual:` or `Not matching items:`. The result adds its contexts in `AppendContexts`:

```csharp
private sealed class HasPlaylistConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<Track[]>(it, grammars),
        IValueConstraint<Track[]?>
{
    public ConstraintResult IsMetBy(Track[]? actual)
    {
        Actual = actual;
        Outcome = actual?.Length is > 0 and <= 20 ? Outcome.Success : Outcome.Failure;
        return this;
    }

    public override void AppendContexts(ResultContextCollector contexts)
    {
        if (Actual is { } tracks)
        {
            contexts.Add(new ResultContext.SyncCallback("Playlist",
                () => Formatter.Format(tracks, FormattingOptions.MultipleLines)));
        }
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("has a playlist of up to 20 tracks");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" had ").Append(Actual?.Length).Append(" tracks");

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not have a playlist of up to 20 tracks");
}
```

- `AppendContexts` is only called while the failure message is created, so an expectation that is met creates no
  context. Keep the values that the context needs in the constraint, and format them in the callback.
- It is only called for the parts of a combined expectation that explain the failure, after every negation, e.g. not
  for a succeeding operand of `And` or for a negated part that succeeds again under `DoesNotComplyWith`. Decide in
  `AppendContexts` on the final `Grammars` and `Outcome` instead of in `IsMetBy` or `NegateInPlace()`.
- Capture the state in the callback when `AppendContexts` runs (`tracks` above), as the result of an item expectation
  is evaluated again for the next item before the content is created.
- A result that combines other results adds their contexts with `contexts.Visit(result)` for the parts that explain
  its outcome, or `contexts.VisitMember("name", result)` to label them with a member. `contexts.VisitItem(2, result)`
  labels them with the item of a collection, e.g. `Actual (item [2]):`, after the contexts of the collection.
- To show the subject whenever any expectation on it fails, not only your own, register a callback once with
  `expectationBuilder.AddSubjectContexts<T>((subject, contexts) => …)`, e.g. the HTTP request of a response. Pass a
  cached delegate, as the same callback is only added once per subject.
- The contexts of the built-in expectations are available as extensions on the `ResultContextCollector`, so that a
  custom expectation shows them alike: `AddCollectionContext`, `AddDictionaryContext`, `AddExpectedValuesContext`,
  `AddStringContext`, `AddStringDifferencesContext`, `AddEqualityOptionsContexts` and `AddEquivalencyContext`.

<details>
<summary>Labels, order and duplicates of contexts</summary>

- A context of a member is labeled with it, e.g. `Playlist (Albums):`. Contexts with the same title and member are shown once when their content is the same, and are numbered otherwise, e.g. `Playlist #1:`.
- The contexts of `AddSubjectContexts` precede those of the expectations and are labeled with the member or item when added for one.

</details>

## Exceptions

An exception that rejects an invalid argument is a complete sentence that starts with `The` and ends with a period,
e.g. `The maximum must be greater than or equal to the minimum.` or `The tolerance must be a whole number of days.`.
