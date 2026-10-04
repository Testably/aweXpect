# Message conventions

A failure message of aweXpect reads like one English sentence. The built-in expectations follow the conventions on
this page, so that an extension that follows them as well reads like part of the library.

The samples on this page use the following namespaces:

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;
```

## Shape of a failure message

aweXpect composes the failure message from the subject, the expectation texts and the result texts of the
constraints, and the reason from `Because(…)`:

```text title="Failure message"
Expected that <subject>
<expectation>[, because <reason>],
but <result>
```

Your constraint only writes its expectation text (`AppendExpectation`) and its result text (`AppendResult`); the rest
is added around them. For the `IsRadioFriendly` expectation from
[constraints and results](./02-constraints-and-results.md), `await Expect.That(track).IsRadioFriendly()` fails for
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
- Append the options after the expectation, e.g. ` ignoring case`, ` using MyComparer` or ` in any order`.
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
`has a title matching title => title.StartsWith("Let")`. `CallerArgumentExpressionAttribute` requires C# 10 and is missing
in `netstandard2.0` and `net48`. Declare it as an `internal` type in your own package there.

## Result text

- Start with the name of the subject, the `it` parameter of the constraint (exposed as `It` by the helper classes),
  followed by a verb in the past tense: `it was 7:11 long`, `it had 3 items`, `ASide was 7:11 long`. The name is
  `it` or the name of a member, so never write the subject yourself.
- Describe what was found instead, without repeating the expectation. A short elliptical result is normal:
  `it was`, `it did`, `it was not`.
- Add a detail after a comma: `, which differs at index 12`, `, which differs by 2`.
- Put longer information, such as the items of a collection, in a context below the message instead (see
  [contexts](#contexts)).

Some results are written for you:

| Situation                   | Result text                                                             | Written by                                                              |
|-----------------------------|-------------------------------------------------------------------------|-------------------------------------------------------------------------|
| a `null` subject            | `it was <null>`                                                         | `ConstraintResult.WithNotNullValue<T>`                                  |
| the evaluation was canceled | `it could not be verified, because the evaluation was already canceled` | `AppendCanceledResult`, the default of `AppendUndecidedResult`          |
| code of the caller threw    | `the predicate did throw an InvalidOperationException`                  | [`UserCode.Invoke`](./02-constraints-and-results.md#code-of-the-caller) |

## Formatting values

Format every value with `Formatter.Format` from `aweXpect.Formatting.Format`, so that it looks the same as in the
built-in messages and respects the [formatting settings](../03-how-it-works/07-configuration.md#formatting):

| Value         | Formatted as                                                                                                  |
|---------------|---------------------------------------------------------------------------------------------------------------|
| `null`        | `<null>`                                                                                                      |
| `string`      | `"Let It Be"`, in quotes and truncated after `MaximumStringLength` characters                                 |
| `char`        | `'a'`                                                                                                         |
| numbers       | `42` or `-3.5`, independent of the current culture                                                            |
| `TimeSpan`    | `0:02` or `0:00.015`                                                                                          |
| `DateTime`    | `2024-12-24T13:15:00.0000000`                                                                                 |
| `Type`        | its C# name without namespace, e.g. `int` or `List<string>`                                                   |
| `enum`        | `Read`, or `Read \| Write` for a combination of flags                                                         |
| `Exception`   | its type and message, e.g. `InvalidOperationException: Yesterday`                                             |
| collection    | `["Help!", "Revolver"]`, with `(… and 2 more)` after `MaximumNumberOfCollectionItems` items                   |
| other objects | their `ToString()` if it is overridden, otherwise their public members, e.g. `Album { Title = "Abbey Road" }` |

Chars and strings on a single line are escaped like C# literals, so that every character can be told apart: a
backslash, the enclosing quote, line breaks, tabs, control characters, invisible characters (like a non-breaking or a
zero-width space), combining marks in text that is not normalized (like the accent of a decomposed `é`) and unpaired
surrogates are shown as `\\`, `\"` (or `\'` in a char), `\n`, `\r`, `\t`, `\0` or `\uXXXX`. Exception messages and the
`ToString()` of other objects are not quoted, so only their line breaks, control and invisible characters are escaped
when they are written on a single line.

The `FormattingOptions` change the layout: `FormattingOptions.MultipleLines` puts every item of a collection on its
own line, e.g. for a context, `FormattingOptions.WithType` prefixes the type (`int[] [1, 2]`), and
`FormattingOptions.Indented(indentation)` indents the following lines. Without options, an object that is not an item
of a collection puts each member on its own line; `FormattingOptions.SingleLine` keeps it on one line as in the table.
Register an `IValueFormatter` to format your own types, see [initialization](./05-initialization.md).

Nested objects, collections and tuples are written up to 20 levels deep and up to 1000 of them per value. Beyond that,
their content is left out as `{ … }`, `[ … ]` or `( … )`, so that a long chain or a graph that shares its nodes on
every level neither overflows the stack nor grows without bound. An object or collection that contains itself is
written as `{ *recursive* }` or `[ *recursive* ]` where it repeats.

## Vocabulary

Name the expectation methods like the built-in ones, so that the whole chain reads like a sentence:

| Pattern                                    | Use it for                                                     | Examples                                            |
|--------------------------------------------|----------------------------------------------------------------|-----------------------------------------------------|
| `Is…`, `IsNot…`                            | a state or a comparison of the subject                         | `IsEmpty`, `IsNotEqualTo`, `IsRadioFriendly`        |
| `Has…`                                     | a property of the subject, optionally with a comparison        | `HasLength(3)`, `HasCount().GreaterThan(2)`         |
| `DoesNot…`                                 | the negation of a verb                                         | `DoesNotContain`, `DoesNotStartWith`                |
| `With…`                                    | a property of the result of the previous expectation           | `Throws<T>().WithMessage(…)`                        |
| `Which`, `Whose`                           | continuing with a new subject, or with a member of the subject | `HasSingle().Which`, `Whose(x => x.Title, …)`       |
| `Ignoring…`, `Using`, `Within`, `In…Order` | options of the previous expectation                            | `IgnoringCase()`, `Using(comparer)`, `InAnyOrder()` |

`With…` continues the sentence of the previous expectation, "throws a `CustomException` with message …", while `Has…`
starts a new sentence about the subject. Offer both when your subject can appear in both positions, like the
[exception expectations](../06-behaviour/01-delegates.md#with-after-throws-has-on-the-exception) do.

## Grammar

The `grammars` a constraint receives tell it how its texts are used in the sentence. They are an
`ExpectationGrammars` flags enum:

| Flag         | Set when                                                                                                      |
|--------------|---------------------------------------------------------------------------------------------------------------|
| `Negated`    | the expectation is negated; the helper classes then call `AppendNegatedExpectation` and `AppendNegatedResult` |
| `Plural`     | the subject of the sentence is plural, e.g. `whose Tracks are radio friendly for all items`                   |
| `Nested`     | the expectation continues the sentence of another one, e.g. for a member or an item                           |
| `Active`     | the expectation continues a `With…` clause, which drops the verb, e.g. `with message equal to "bar"`          |
| `Introduced` | the subject was already introduced by a connector like the `that` of `has item that`                          |

`Active` has the opposite meaning for the expectation text of a string match type (`IStringMatchType.GetExpectation`):
there it asks for the text with the verb (`is equal to "bar"`), and without it the text starts with the comparison
(`equal to "bar"`).

`ExpectationGrammarsExtensions` checks them with `IsNegated()`, `IsNested()`, `IsPlural()` or
`HasAnyFlag(…)`, and toggles the negation with `Negate()`. Use the plural form of the verb in the expectation text
when the grammars are plural, but keep the singular form in the result text as long as the subject is the pronoun
`it`:

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
        stringBuilder.Append(It).Append(Grammars.IsPlural() && It != "it" ? " were " : " was ");
        Formatter.Format(stringBuilder, Actual?.Duration);
        stringBuilder.Append(" long");
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(Grammars.IsPlural() ? "are not radio friendly" : "is not radio friendly");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => AppendNormalResult(stringBuilder, indentation);
}
```

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

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => AppendNormalResult(stringBuilder, indentation);
}
```

- `AppendContexts` is only called while the failure message is created, so an expectation that is met creates no
  context. Keep the values that the context needs in the constraint, and format them in the callback.
- It is only called for the parts of a combined expectation that explain the failure, after every negation, e.g. not
  for a succeeding operand of `And` or for a negated part that succeeds again under `DoesNotComplyWith`. Decide in
  `AppendContexts` on the final `Grammars` and `Outcome` instead of in `IsMetBy` or `Negate()`.
- Capture the state in the callback when `AppendContexts` runs (`tracks` above), as the result of an item expectation
  is evaluated again for the next item before the content is created.
- A context of a member is labelled with it, e.g. `Playlist (Albums):`. Contexts with the same title and member are
  shown once when their content is the same, and are numbered otherwise, e.g. `Playlist #1:`.
- A result that combines other results adds their contexts with `contexts.Visit(result)` for the parts that explain
  its outcome, or `contexts.VisitMember("name", result)` to label them with a member. `contexts.VisitItem(2, result)`
  labels them with the item of a collection, e.g. `Actual (item [2]):`, after the contexts of the collection.
- The contexts of the built-in expectations are available as extensions on the `ResultContextCollector`, so that a
  custom expectation shows them alike: `AddCollectionContext`, `AddDictionaryContext`, `AddExpectedValuesContext`,
  `AddStringContext`, `AddEqualityOptionsContexts` and `AddEquivalencyContext`. An `ObjectEqualityOptions<T>`
  compares like `IsEquivalentTo` with `SetMatchType(new EquivalencyMatchType(options), "Equivalent")`.

## Exceptions

An exception that rejects an invalid argument is a complete sentence that starts with `The` and ends with a period,
e.g. `The maximum must be greater than or equal to the minimum.` or `The tolerance must be a whole number of days.`.
