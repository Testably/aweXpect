---
description: Format values in failure messages like the built-in expectations and register value formatters for the types of your extension.
---

# Formatting

A failure message shows values the same way in every expectation. Format the values of your extension like the
built-in ones, and teach aweXpect how to format the types that your extension declares.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;
```

</details>

## Formatting values

Format every value with `Formatter.Format` from `aweXpect.Formatting.Format`, so that it looks the same as in the
built-in messages and respects the [formatting settings](../02-how-it-works/07-configuration.md#formatting):

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

The `FormattingOptions` change the layout: `FormattingOptions.MultipleLines` puts every item of a collection on its
own line, e.g. for a context, `FormattingOptions.WithType` prefixes the type (`int[] [1, 2]`), and
`FormattingOptions.Indented(indentation)` indents the following lines. Without options, an object that is not an item
of a collection puts each member on its own line; `FormattingOptions.SingleLine` keeps it on one line as in the table.
`FormattingOptions.Snapshot`, or `IsSnapshot = true` on any other options, writes the value for a snapshot: without
shortening it, and the items of collections whose order depends on hash codes or threads, like a `HashSet<T>` or a
`ConcurrentDictionary<TKey, TValue>`, sorted by their formatted text or key, so that it is the same in every run.
Register an `IValueFormatter` to format your own types, see [value formatters](#value-formatters).

`FormatValue` on the `FormattingOptions` decides for every nested value whether it is written
(`ValueFormatting.Write`), left out (`ValueFormatting.Omit`) or replaced by another value that is formatted in its
place (`ValueFormatting.Replace(value)`), e.g. to leave out values that change from run to run in a snapshot. It is
offered the members of objects that are written member-wise, the items of tuples and collections, and the values of
dictionary entries, as a `FormattedValue` with its `Kind`, `Name`, `Type`, `DeclaringType` and `Path`. The path has the
format of an equivalency comparison (`Artist.Name`, `Tracks[0].Id`, `Ratings[4.5]`, `Seats[0,1]`), with `[*]` as the
index of an item of a collection whose order depends on hash codes or threads, like a `HashSet<T>`, so that the rules
to ignore members in an equivalency comparison can be reused:

```csharp
MemberToIgnore ignoreDuration = new MemberToIgnore.ByName("Duration");
FormattingOptions options = FormattingOptions.Indented("") with
{
    IsSnapshot = true,
    FormatValue = value => ignoreDuration.IgnoreMember(value.Path, value.Type)
        ? ValueFormatting.Omit
        : value.Name == "Title"
            ? ValueFormatting.Replace("{title}")
            : ValueFormatting.Write,
};
string snapshot = Formatter.Format(new Track("Let It Be", TimeSpan.FromSeconds(243)), options);
```

<details>
<summary>Exceptions in snapshots, FormatValue, escaping and limits</summary>

A snapshot writes an exception member-wise, e.g. `ArgumentException { Message = "…", ParamName = "track" }`, with its
message, its own public members, its `Data` when it is not empty and its inner exceptions (all of them for an
`AggregateException`), but without `StackTrace`, `TargetSite`, `Source`, `HelpLink` and `HResult`, which change between
builds, runtimes and Native AOT.

`FormatValue` is not offered the value itself, nor the keys of a dictionary, nor anything inside a value that a
registered `IValueFormatter` or its own `ToString()` renders. A member is only read when it is written or when
`GetValue()` asks for it, so a member that is left out is not read at all. Leaving out an item of a tuple shifts the
following items, so that their position in the parentheses no longer matches their position in the tuple. A getter
that throws is written as the usual placeholder, while an exception of `FormatValue` itself is thrown out of the
formatting. Without `FormatValue`, nothing changes.

Chars and strings are always written on a single line and escaped like C# literals, so that every character can be
told apart: a backslash, the enclosing quote, line breaks, tabs, control characters, invisible characters (like a
non-breaking or a zero-width space), combining marks in text that is not normalized (like the accent of a decomposed
`é`) and unpaired surrogates are shown as `\\`, `\"` (or `\'` in a char), `\n`, `\r`, `\t`, `\0` or `\uXXXX`.
Exception messages and the `ToString()` of other objects are not quoted, so only their line breaks, control and
invisible characters are escaped when they are written on a single line. A multi-dimensional array is written with a
nested collection per dimension, e.g. `[[1, 2], [3, 4]]` for an `int[2,2]`.

Nested objects, collections and tuples are written up to 20 levels deep and up to 1000 of them per value. Beyond that,
their content is left out as `{ … }`, `[ … ]` or `( … )`, so that a long chain or a graph that shares its nodes on
every level neither overflows the stack nor grows without bound. An object or collection that contains itself is
written as `{ *recursive* }` or `[ *recursive* ]` where it repeats. Snapshot options lift these limits and those of the
[formatting settings](../02-how-it-works/07-configuration.md#formatting); a value that is nested too deeply for the
remaining stack then throws an `InsufficientExecutionStackException` instead.

</details>

## Value formatters

A value formatter returns `false` for every value it does not format, so that the next formatter or the built-in
formatting applies. This `TrackFormatter` writes a `Track` with its title and duration:

```csharp
internal sealed class TrackFormatter : IValueFormatter
{
    public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
    {
        if (value is not Track track)
        {
            return false;
        }

        stringBuilder.Append(track.Title).Append(" (");
        Format.Formatter.Format(stringBuilder, track.Duration);
        stringBuilder.Append(')');
        return true;
    }
}
```

Register it with `ValueFormatter.Register(new TrackFormatter())` in a
[module initializer](./11-initialization.md#module-initializers), which runs before your extension is first used.

A registered formatter applies process-wide, to all threads and async flows, until the returned `IDisposable` is
disposed. It is asked for every value that is not `null`, including built-in types such as `DateTime` or `int`,
whether the value is formatted on its own, as an item of a collection or as a member of an object. When several
registered formatters can format a value, the most recently registered one is used.

Register a formatter only for the types that your extension declares. The module initializer runs before any of them
is used, so their messages never depend on the order of the tests. A formatter for a type that your extension does not
own, e.g. `System.Version`, would only apply to the tests that run after the first test that used your extension, and
would change how other tests format it. Add such a formatter to your expectations instead.

### Formatters of an expectation

`ExpectationBuilder.AddFormatter` formats the values of one expectation with a formatter, before the registered ones.
It applies while the expectation is evaluated and while its failure message is created, also to the expectations
chained to it, e.g. collection expectations on the items of a member, but never outside of the expectation:

```csharp
internal static class MusicStoreFormatting
{
    private static readonly IValueFormatter VersionFormatter = new ShortVersionFormatter();

    internal static ExpectationBuilder WithVersionFormatter(this ExpectationBuilder expectationBuilder)
        => expectationBuilder.AddFormatter(VersionFormatter);

    private sealed class ShortVersionFormatter : IValueFormatter
    {
        public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
        {
            if (value is not Version version)
            {
                return false;
            }

            stringBuilder.Append('v').Append(version.ToString(3));
            return true;
        }
    }
}
```

When the expectation is nested in another one, e.g. with `ComplyWith`, or combined with others in `Expect.ThatAll` or
`Expect.ThatAny`, the whole failure message uses the formatter. Pass a cached instance, as adding the same formatter
again has no effect.
