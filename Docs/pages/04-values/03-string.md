import PropertyComparisons from '../_property-comparisons.md';

# String

Describes the possible expectations for strings.

| Expectation                                       | Negated                 | Summary                                                             |
|---------------------------------------------------|-------------------------|---------------------------------------------------------------------|
| [`IsEqualTo`](#equality)                          | `IsNotEqualTo`          | equal to the expected string, or matching a [pattern](#match-types) |
| [`IsOneOf`](#one-of)                              | `IsNotOneOf`            | equal to one of the expected strings                                |
| [`IsNull`](#null-empty-or-whitespace)             | `IsNotNull`             | `null`                                                              |
| [`IsEmpty`](#null-empty-or-whitespace)            | `IsNotEmpty`            | the empty string                                                    |
| [`IsNullOrEmpty`](#null-empty-or-whitespace)      | `IsNotNullOrEmpty`      | `null` or the empty string                                          |
| [`IsNullOrWhiteSpace`](#null-empty-or-whitespace) | `IsNotNullOrWhiteSpace` | `null`, empty or only whitespace                                    |
| [`HasLength`](#length)                            | negated comparison      | has the expected number of characters                               |
| [`HasLineCount`](#lines)                          | negated comparison      | has the expected number of lines                                    |
| [`HasLines`](#lines)                              |                         | its lines meet collection expectations                              |
| [`StartsWith`](#start--end)                       | `DoesNotStartWith`      | starts with the expected string                                     |
| [`EndsWith`](#start--end)                         | `DoesNotEndWith`        | ends with the expected string                                       |
| [`Contains`](#contains)                           | `DoesNotContain`        | contains the expected substring, optionally a number of times       |
| [`IsUpperCased`](#character-casing)               | `IsNotUpperCased`       | every letter with an upper-case form is upper-case                  |
| [`IsLowerCased`](#character-casing)               | `IsNotLowerCased`       | every letter with a lower-case form is lower-case                   |
| [`IsParsableInto<T>`](#parsing)                   | `IsNotParsableInto<T>`  | can be parsed into `T`                                              |

## Equality

You can verify that the `string` is equal to another one or not:

```csharp
string title = "Abbey Road";

await Expect.That(title).IsEqualTo("Abbey Road");
await Expect.That(title).IsNotEqualTo("Let It Be");
```

The comparison is ordinal and can be configured with the [string options](#string-options) or compare against a
[pattern](#match-types).

## String options

The expectations that compare the `string` with an expected string (`IsEqualTo`, `IsOneOf`, `StartsWith`, `EndsWith`,
`Contains` and their negations) take the following options:

| Option                         | Effect                                                                     |
|--------------------------------|----------------------------------------------------------------------------|
| `IgnoringCase()`               | compares with `StringComparison.OrdinalIgnoreCase`                         |
| `IgnoringNewlineStyle()`       | treats `\r\n`, `\n` and `\r` as equal                                      |
| `IgnoringIndentation()`        | removes the leading whitespace from every line (see [below](#indentation)) |
| `IgnoringLeadingWhiteSpace()`  | ignores whitespace at the start of the strings                             |
| `IgnoringTrailingWhiteSpace()` | ignores whitespace at the end of the strings                               |
| `Using(comparer)`              | compares with a custom `IEqualityComparer<string>`                         |

```csharp
string title = "Abbey Road";

await Expect.That(title).IsEqualTo("ABBEY ROAD").IgnoringCase();
await Expect.That("Abbey\r\nRoad").IsEqualTo("Abbey\nRoad").IgnoringNewlineStyle();
await Expect.That("  Abbey\n    Road").IsEqualTo("Abbey\nRoad").IgnoringIndentation();
await Expect.That(title).IsEqualTo("  Abbey Road").IgnoringLeadingWhiteSpace();
await Expect.That(title).IsEqualTo("Abbey Road \t").IgnoringTrailingWhiteSpace();
await Expect.That(title).StartsWith("ABBEY").Using(StringComparer.OrdinalIgnoreCase);
```

The same options apply wherever strings are compared, e.g. to the items of a collection of strings or to the message of
an exception.

The whitespace options only ignore whitespace at the start or the end of the subject. A prefix, suffix or substring
keeps the whitespace at its inner end, unless that end reaches the corresponding end of the subject:

```csharp
await Expect.That("Abbey").StartsWith("Abbey ").IgnoringTrailingWhiteSpace();
await Expect.That("AbbeyRoad").DoesNotStartWith("Abbey ").IgnoringTrailingWhiteSpace();
```

`IgnoringCase()` and `Using(…)` can't be combined, because only one of them could decide how the casing is compared:
the second one throws an `InvalidOperationException`, whichever order they are specified in. Use a case-insensitive
comparer such as `StringComparer.OrdinalIgnoreCase` instead.

The negations take the same options, so an option can make them fail:

```csharp
string title = "Abbey Road";

await Expect.That(title).DoesNotEndWith("ROAD")
  .Because("the casing differs, which would not count with `IgnoringCase()`");
```

### Indentation

While `IgnoringLeadingWhiteSpace` only trims the start of the complete `string`, `IgnoringIndentation` removes the
leading whitespace from *every* line. This allows comparing against a raw string literal that is indented differently
than the subject:

```csharp
string code = """
              public class Beatles
              {
                  public string Album => "Abbey Road";
              }
              """;

await Expect.That(code).Contains("""
                                 public string Album => "Abbey Road";
                                 """).IgnoringIndentation();
```

As the lines are split on `\r\n`, `\n` and `\r`, this also normalizes the newline style, which makes
`IgnoringNewlineStyle` redundant. Trailing whitespace within a line is kept, but a line that consists only of
whitespace becomes empty. To keep the relative indentation within the snippet, use [`AsBlock`](#blocks) instead.

## Match types

Instead of comparing for equality, `IsEqualTo` can match the subject against a pattern, a prefix or a suffix. The
same match types are available for `IsNotEqualTo`, `IsOneOf` and `IsNotOneOf`. `Contains` and `DoesNotContain` search
within the subject, so they take a pattern, but no prefix or suffix: use [`StartsWith` or `EndsWith`](#start--end)
for that.

:::note[A `null` subject has no content to match]
Every match type except the plain comparison asks about the content of the subject, so it fails for a `null` subject in
both directions, exactly like `StartsWith` and `DoesNotStartWith` do.
:::

### Wildcards

```csharp
string title = "Let It Be";

await Expect.That(title).IsEqualTo("Let*B?").AsWildcard();
```

| Wildcard specifier | Matches                                        |
|--------------------|------------------------------------------------|
| * (asterisk)       | Zero or more characters                        |
| ? (question mark)  | Exactly one character (an emoji counts as one) |

The pattern has to cover the complete subject, including all its lines and a trailing newline. An empty pattern
therefore matches only an empty subject. A `null` pattern is rejected with an `ArgumentNullException`, because it
matches no subject at all.

### Regular expressions

```csharp
string title = "Let It Be";

await Expect.That(title).IsEqualTo("(.*)Be").AsRegex();
```

The pattern is matched like `Regex.IsMatch(subject, pattern)`, so unlike a wildcard it may match any part of the
subject: `IsEqualTo("It").AsRegex()` succeeds for `"Let It Be"`. Enclose the pattern in `\A` and `\z` to match the
complete subject. `^` and `$` bind to the start and the end of the subject and not to every line, but `$` also matches
before a trailing newline. `IgnoreCase` and `CultureInvariant` are added when the `IgnoringCase` method is
also used, so that ignoring the casing never depends on the current culture. The regex engine still applies its own
case folding, which can differ from the other expectations for a few characters, e.g. the Kelvin sign (U+212A) on
modern .NET. Every other
[option](https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regexoptions#fields)
is opt-in:

```csharp
using System.Text.RegularExpressions;

string lyrics = "Come together\nRight now";

await Expect.That(lyrics).IsEqualTo("^Right now$").AsRegex(RegexOptions.Multiline);
```

An empty pattern is rejected with an `ArgumentException` and a `null` pattern with an `ArgumentNullException`, because
an empty pattern matches every subject and a `null` pattern matches no subject, so one of the two expectations could
never fail. A pattern that is not a valid regex is rejected with an `ArgumentException` that carries the parse error as
its inner exception, even for a `null` subject.

A wildcard or regex pattern is matched by the regex engine, which can't use a custom comparer, so `Using(…)` after
`AsWildcard()` or `AsRegex()` throws an `InvalidOperationException`. Matching a pattern is limited to one second. A
pattern that takes longer, e.g. because of catastrophic backtracking, throws an `ArgumentException` that asks you to
simplify the pattern.

### Prefix / Suffix

```csharp
string title = "Abbey Road";

await Expect.That(title).IsEqualTo("Abbey").AsPrefix();
await Expect.That(title).IsEqualTo("Road").AsSuffix();
```

An empty prefix or suffix is rejected with an `ArgumentException`, because every subject starts and ends with the empty
string, so such an expectation says nothing about the subject. A `null` prefix or suffix is rejected with an
`ArgumentNullException`, just like a `null` pattern.

## One of

You can verify that the `string` is one of many alternatives, with the same [options](#string-options) as for
equality:

```csharp
string title = "Abbey Road";

await Expect.That(title).IsOneOf("Help!", "Abbey Road", "Revolver");
await Expect.That(title).IsOneOf("HELP!", "ABBEY ROAD", "REVOLVER").IgnoringCase();
await Expect.That(title).IsNotOneOf("Help!", "Revolver");
```

## Null, empty or whitespace

You can verify that the `string` is `null`, empty or contains only whitespace:

```csharp
string? title = null;

await Expect.That(title).IsNull();
await Expect.That("Abbey Road").IsNotNull();

await Expect.That("").IsEmpty();
await Expect.That("Abbey Road").IsNotEmpty();

await Expect.That(title).IsNullOrEmpty();
await Expect.That("Abbey Road").IsNotNullOrEmpty();
await Expect.That(title).IsNullOrWhiteSpace();
await Expect.That("Abbey Road").IsNotNullOrWhiteSpace();
```

## Length

You can verify that the `string` has the expected length:

```csharp
string title = "Abbey Road";

await Expect.That(title).HasLength(10);
await Expect.That(title).HasLength().Between(8).And(12);
await Expect.That(title).HasLength().NotGreaterThan(12);
```

<PropertyComparisons />

## Lines

You can verify how many lines the `string` has:

```csharp
string lyrics = """
                Come together
                Right now
                Over me
                """;

await Expect.That(lyrics).HasLineCount(3);
await Expect.That(lyrics).HasLineCount().NotEqualTo(4);
await Expect.That(lyrics).HasLineCount().GreaterThan(2);
```

`HasLineCount()` takes the same [comparisons](#length) as `HasLength()`.

You can also verify the lines themselves. `HasLines` applies the expectations on the lines as a collection, so all
[collection expectations](../05-collections/index.md) are available:

```csharp
await Expect.That(lyrics).HasLines(lines => lines.Contains("Right now"));
await Expect.That(lyrics).HasLines(lines => lines.StartsWith("Come together"));
await Expect.That(lyrics).HasLines(lines => lines.All().Satisfy(line => line?.Length < 20));
```

Lines are separated by `\r\n`, `\n` or `\r`, which are all treated equivalently. A single trailing line terminator
does not start a new line, so `"Come together\n"` has one line and `""` has none, matching how `File.ReadLines` counts
the lines of a file:

```csharp
await Expect.That("").HasLineCount(0);
await Expect.That("Come together").HasLineCount(1);
await Expect.That("Come together\n").HasLineCount(1);
await Expect.That("Come together\n\n").HasLineCount(2);
```

## Start / end

You can verify that the `string` starts or ends with a given string, or that it does not, with the same
[options](#string-options) as for equality:

```csharp
string title = "Abbey Road";

await Expect.That(title).StartsWith("Abbey");
await Expect.That(title).EndsWith("ROAD").IgnoringCase();
await Expect.That(title).DoesNotStartWith("Road");
await Expect.That(title).DoesNotEndWith("Abbey");
```

In all four expectations, a `null` expected string throws an `ArgumentNullException` and an empty one an
`ArgumentException`, because every string starts and ends with the empty string.

## Contains

You can verify that the `string` contains a given substring, or that it does not, with the same
[options](#string-options) as for equality:

```csharp
string title = "Strawberry Fields Forever";

await Expect.That(title).Contains("Fields");
await Expect.That(title).Contains("FIELDS").IgnoringCase();
await Expect.That(title).DoesNotContain("Penny Lane");
```

As for [`StartsWith`](#start--end), a `null` substring in `Contains` or `DoesNotContain` throws an
`ArgumentNullException` and an empty one an `ArgumentException`.

You can also specify how often the substring should be found:

```csharp
string lyrics = "get back, get back, get back to where you once belonged.";

// 'get' can be found 3 times
await Expect.That(lyrics).Contains("get").MoreThan(1)
  .Because("count should be '> 1'");
await Expect.That(lyrics).Contains("get").AtLeast(2)
  .Because("count should be '>= 2'");
await Expect.That(lyrics).Contains("get").Exactly(3)
  .Because("count should be '== 3'");
await Expect.That(lyrics).Contains("get").AtMost(4)
  .Because("count should be '<= 4'");
await Expect.That(lyrics).Contains("get").LessThan(5)
  .Because("count should be '< 5'");
await Expect.That(lyrics).Contains("get").Between(1).And(6)
  .Because("count should be '>= 1 AND <= 6'");
```

### Blocks

While `IgnoringIndentation` removes the leading whitespace from every line, `AsBlock` keeps the relative
indentation within the expected block and only allows the block as a whole to be indented in the subject.
This is stricter, as a line that is indented differently from the rest of the block does not match:

```csharp
string code = """
              public class Beatles
              {
                  public string Album
                  {
                      get;
                  }
              }
              """;

await Expect.That(code).Contains("""
                                 public string Album
                                 {
                                     get;
                                 }
                                 """).AsBlock();
```

The block must start and end at line boundaries, and all its lines must share the same whitespace prefix in the
subject. A line that consists only of whitespace matches any line that consists only of whitespace. The newline style
is always ignored, and a single trailing line terminator does not start a new line, so `"a\nb\n"` has the same two
lines as `"a\nb"` (as for [lines](#lines)). `AsBlock` can be combined with `IgnoringCase`, `Using` and the count
quantifiers.

`AsBlock` also works with `IsEqualTo`, where the whole text has to be the block, e.g. for an XML element that keeps
the indentation of the document it was taken from, and with the expectations on a collection of strings, such as
`HasItem`:

```csharp
string album = """
                   <album>
                     <title>Abbey Road</title>
                     <length>47:03</length>
                   </album>
               """;

await Expect.That(album).IsEqualTo("""
                                   <album>
                                     <title>Abbey Road</title>
                                     <length>47:03</length>
                                   </album>
                                   """).AsBlock();
```

## Character casing

You can verify that the characters in a `string` are all upper or lower cased:

```csharp
await Expect.That("1ST PLACE").IsUpperCased()
  .Because("it contains no lowercase characters");
await Expect.That("1st PLACE").IsNotUpperCased()
  .Because("it contains at least one lowercase character");

await Expect.That("1st place").IsLowerCased()
  .Because("it contains no uppercase characters");
await Expect.That("1st PLACE").IsNotLowerCased()
  .Because("it contains at least one uppercase character");
```

Letters without an upper-case (lower-case) form, like `ß`, count as upper-cased (lower-cased). Use
`IncludingUncasedLetters()` to also reject them and titlecase letters:

```csharp
await Expect.That("STRAßE").IsNotUpperCased().IncludingUncasedLetters()
  .Because("ß is a lowercase letter without an uppercase form");
```

`IncludingUncasedLetters(false)` behaves as if the option was not specified.

## Parsing

:::note[.NET 8 or later]
The parsing expectations are only available on .NET 8 or later.
:::

You can verify that the `string` can be parsed into a type that implements `IParsable<T>`, and continue with
expectations on the parsed value with `Which`:

```csharp
using System.Globalization;

await Expect.That("42").IsParsableInto<int>();
await Expect.That("42").IsParsableInto<int>().Which.IsGreaterThan(40);
await Expect.That("1,5").IsParsableInto<double>(new CultureInfo("de-DE"))
  .Because("the format provider is passed to `Parse`");

await Expect.That("Abbey Road").IsNotParsableInto<int>();
```

A failure shows the exception thrown by `Parse` and keeps it as inner exception. A `null` subject fails both
expectations.

The same expectations are available for a `ReadOnlySpan<char>` of a type that implements `ISpanParsable<T>` and for a
UTF-8 `ReadOnlySpan<byte>` of a type that implements `IUtf8SpanParsable<T>`:

```csharp
await Expect.That("42".AsSpan()).IsParsableInto<int>();
await Expect.That("42"u8).IsParsableInto<int>();
```
