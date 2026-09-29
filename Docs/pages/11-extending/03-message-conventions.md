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
is added around them. For the `IsAbsolutePath` expectation from
[constraints and results](./02-constraints-and-results.md), `await Expect.That(path).IsAbsolutePath()` fails with:

```text title="Failure message"
Expected that path
is an absolute path,
but it was "album.txt"
```

The same texts are reused when the expectation is combined or nested, e.g. inside `Whose`, where `it` is replaced by
the name of the member:

```text title="Failure message"
Expected that playlist
whose Path is an absolute path,
but Path was "album.txt"
```

## Expectation text

- Start with the verb in the present tense, in lower case, and end without punctuation: `is an absolute path`,
  `is equal to "Abbey Road"`, `starts with "Abbey"`, `has flag A`.
- Write the negated text with `not`: `is not an absolute path`, `does not start with "Abbey"`.
- Append the options after the expectation, e.g. ` ignoring case`, ` using MyComparer` or ` in any order`.
- Describe the expected value, not the check: `is an absolute path` instead of `Path.IsPathRooted returns true`.

## Result text

- Start with the name of the subject, the `it` parameter of the constraint (exposed as `It` by the helper classes),
  followed by a verb in the past tense: `it was "album.txt"`, `it had 3 items`, `Path was "album.txt"`. The name is
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
| `Exception`   | its type and message, e.g. `InvalidOperationException: Yesterday`                                             |
| collection    | `["Help!", "Revolver"]`, with `(… and 2 more)` after `MaximumNumberOfCollectionItems` items                   |
| other objects | their `ToString()` if it is overridden, otherwise their public members, e.g. `Album { Title = "Abbey Road" }` |

The `FormattingOptions` change the layout: `FormattingOptions.MultipleLines` puts every item of a collection on its
own line, e.g. for a context, `FormattingOptions.WithType` prefixes the type (`int[] [1, 2]`), and
`FormattingOptions.Indented(indentation)` indents the following lines. Register an `IValueFormatter` to format your
own types, see [initialization](./05-initialization.md).

## Vocabulary

Name the expectation methods like the built-in ones, so that the whole chain reads like a sentence:

| Pattern                                    | Use it for                                                     | Examples                                            |
|--------------------------------------------|----------------------------------------------------------------|-----------------------------------------------------|
| `Is…`, `IsNot…`                            | a state or a comparison of the subject                         | `IsEmpty`, `IsNotEqualTo`, `IsAbsolutePath`         |
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
| `Plural`     | the subject of the sentence is plural, e.g. `whose Files are absolute paths for all items`                    |
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
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<string>(it, grammars),
        IValueConstraint<string>
{
    public ConstraintResult IsMetBy(string actual)
    {
        Actual = actual;
        Outcome = Path.IsPathRooted(actual) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(Grammars.IsPlural() ? "are absolute paths" : "is an absolute path");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(Grammars.IsPlural() && It != "it" ? " were " : " was ");
        Formatter.Format(stringBuilder, Actual);
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(Grammars.IsPlural() ? "are not absolute paths" : "is not an absolute path");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => AppendNormalResult(stringBuilder, indentation);
}
```

## Contexts

Information that does not fit into one sentence, such as the items of a collection or the expected and the actual
value of a long comparison, belongs in a context. A context is shown below the message with its title, e.g.
`Collection:`, `Expected:`, `Actual:` or `Not matching items:`. Use the overload of `AddConstraint` that also passes the
`ExpectationBuilder`, and add the context while the constraint is evaluated:

```csharp no-compile
expectationBuilder.AddContext(new ResultContext.Fixed("Playlist", Formatter.Format(files, FormattingOptions.MultipleLines)));
```

## Exceptions

An exception that rejects an invalid argument is a complete sentence that starts with `The` and ends with a period,
e.g. `The maximum must be greater than or equal to the minimum.` or `The tolerance must be a whole number of days.`.
