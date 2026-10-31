---
description: Verify strings for equality, patterns, length, lines, prefixes, substrings, casing and parsing, with options for casing and whitespace.
---

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

await That(title).IsEqualTo("Abbey Road");
await That(title).IsNotEqualTo("Let It Be");
```

The comparison is ordinal and can be configured with the [string options](#string-options) or compare against a
[pattern](#match-types).

The failure message points at the first difference. When multi-line values differ in several places, a `Differences`
context lists the changed lines with their line numbers in the actual and the expected value. Each difference is
surrounded by one unchanged line, and differences that are close together are shown as one block:

```text title="Failure message"
Differences:
     2   2    host = localhost
  -  3        port = 8080
  +      3    port = 8081
     4   4    timeout = 30
     5   5    [client]
     6   6    retries = 3
  -  7        verbose = true
  -  8        log = file
  +      7    log = console
```

Tabs and other invisible characters are shown as escape sequences.

<details>
<summary>Which differences are listed</summary>

Lines that only differ in what the [string options](#string-options) ignore are not listed. At most
`MaximumNumberOfCollectionItems` differences are listed (see [formatting](../02-how-it-works/07-configuration.md#formatting)).
With a custom comparer (`Using(…)`), which compares whole values, no differences are listed.

</details>

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
| `NormalizingWhiteSpace()`      | collapses whitespace runs to a space and trims (see [below](#whitespace))  |
| `AsBlock()`                    | matches an indented block of lines (see [below](#blocks))                  |
| `Using(comparer)`              | compares with a custom `IEqualityComparer<string>`                         |

```csharp
string title = "Abbey Road";

await That(title).IsEqualTo("ABBEY ROAD").IgnoringCase();
await That("Abbey\r\nRoad").IsEqualTo("Abbey\nRoad").IgnoringNewlineStyle();
await That("  Abbey\n    Road").IsEqualTo("Abbey\nRoad").IgnoringIndentation();
await That(title).IsEqualTo("  Abbey Road").IgnoringLeadingWhiteSpace();
await That(title).IsEqualTo("Abbey Road \t").IgnoringTrailingWhiteSpace();
await That("Abbey\n\t Road ").IsEqualTo("Abbey  Road").NormalizingWhiteSpace();
await That(title).StartsWith("ABBEY").Using(StringComparer.OrdinalIgnoreCase);
```

The same options apply wherever strings are compared, e.g. to the items of a collection of strings or to the message of
an exception.

The negations take the same options, so an option can make them fail:

```csharp
string title = "Abbey Road";

await That(title).DoesNotEndWith("ROAD")
  .Because("the casing differs, which would not count with `IgnoringCase()`");
```

<details>
<summary>Whitespace at the inner end and ignoring the case with `Using`</summary>

The whitespace options only ignore whitespace at the start or the end of the subject. A prefix, suffix or substring
keeps the whitespace at its inner end, unless that end reaches the corresponding end of the subject:

```csharp
await That("Abbey").StartsWith("Abbey ").IgnoringTrailingWhiteSpace();
await That("AbbeyRoad").DoesNotStartWith("Abbey ").IgnoringTrailingWhiteSpace();
```

To ignore the case with `Using(…)`, pass a case-insensitive comparer such as `StringComparer.OrdinalIgnoreCase`
instead of adding `IgnoringCase()`.

</details>

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

await That(code).Contains("""
                          public string Album => "Abbey Road";
                          """).IgnoringIndentation();
```

This also normalizes the newline style. Trailing whitespace within a line is kept, but a line that consists only of
whitespace becomes empty.

### Whitespace

`NormalizingWhiteSpace` replaces every run of whitespace, including line breaks and tabs, with a single space and
removes the whitespace at the start and the end. This compares generated SQL, HTML or log output regardless of its
layout:

```csharp
string sql = """
             SELECT *
               FROM Albums
              WHERE Title = 'Abbey Road'
             """;

await That(sql).IsEqualTo("SELECT * FROM Albums WHERE Title = 'Abbey Road'").NormalizingWhiteSpace();
await That(sql).Contains("FROM Albums WHERE").NormalizingWhiteSpace();
await That("AbbeyRoad").IsNotEqualTo("Abbey Road").NormalizingWhiteSpace();
```

Whitespace is collapsed, not removed, so `"AbbeyRoad"` still differs from `"Abbey Road"`. Any Unicode whitespace
counts, e.g. also a non-breaking space.

<details>
<summary>Combining `NormalizingWhiteSpace` with other options</summary>

- It makes `IgnoringIndentation`, `IgnoringNewlineStyle`, `IgnoringLeadingWhiteSpace` and `IgnoringTrailingWhiteSpace`
  redundant, as it removes every difference that they ignore. They can still be specified.
- It applies to the patterns of `AsWildcard` and `AsRegex` as well, so a regex sees a single space where the subject
  had a line break. As the line breaks of the pattern are collapsed too, a comment in a pattern with
  `RegexOptions.IgnorePatternWhitespace` extends to the end of the pattern.
- Like with the other whitespace options, a prefix, suffix or substring keeps the whitespace at its inner end, which
  then has to match whitespace in the subject, unless that end reaches the corresponding end of the subject.
- It cannot be combined with [`AsBlock`](#blocks), which compares the lines that the normalization joins.
- A failure shows the values with normalized whitespace and the position of the first difference refers to them.
  Where the failure shows the normalized subject, the original one is listed below it. No differences per line are
  listed.

</details>

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

await That(code).Contains("""
                          public string Album
                          {
                              get;
                          }
                          """).AsBlock();
```

The block must start and end at line boundaries, and all its lines must share the same whitespace prefix in the
subject.

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

await That(album).IsEqualTo("""
                            <album>
                              <title>Abbey Road</title>
                              <length>47:03</length>
                            </album>
                            """).AsBlock();
```

<details>
<summary>Whitespace lines, newlines and other options in a block</summary>

A line that consists only of whitespace matches any line that consists only of whitespace. The newline style is always
ignored, and a single trailing line terminator does not start a new line, so `"a\nb\n"` has the same two lines as
`"a\nb"` (as for [lines](#lines)). `AsBlock` can be combined with `IgnoringCase`, `Using` and the count quantifiers;
the whitespace, newline and indentation options throw an `InvalidOperationException` after it.

</details>

## Match types

Instead of comparing for equality, `IsEqualTo` can match the subject against a pattern, a prefix or a suffix. The
same match types are available for `IsNotEqualTo`, `IsOneOf` and `IsNotOneOf`. `Contains` and `DoesNotContain` search
within the subject, so they take a pattern, but no prefix or suffix: use [`StartsWith` or `EndsWith`](#start--end)
for that.

### Wildcards

```csharp
string title = "Let It Be";

await That(title).IsEqualTo("Let*B?").AsWildcard();
```

| Wildcard specifier | Matches                                                |
|--------------------|--------------------------------------------------------|
| * (asterisk)       | Zero or more characters                                |
| ? (question mark)  | Exactly one character (a surrogate pair counts as one) |

The pattern has to cover the complete subject, including all its lines and a trailing newline.

### Regular expressions

```csharp
string title = "Let It Be";

await That(title).IsEqualTo("(.*)Be").AsRegex();
```

Unlike a wildcard, the pattern may match any part of the subject: `IsEqualTo("It").AsRegex()` succeeds for
`"Let It Be"`. Enclose the pattern in `\A` and `\z` to match the complete subject. `IgnoringCase()` adds
`RegexOptions.IgnoreCase` and `RegexOptions.CultureInvariant`. Every other
[option](https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regexoptions#fields)
is opt-in:

```csharp
using System.Text.RegularExpressions;

string lyrics = "Come together\nRight now";

await That(lyrics).IsEqualTo("^Right now$").AsRegex(RegexOptions.Multiline);
```

### Prefix / suffix

```csharp
string title = "Abbey Road";

await That(title).IsEqualTo("Abbey").AsPrefix();
await That(title).IsEqualTo("Road").AsSuffix();
```

<details>
<summary>Regex engine, timeout and corner cases</summary>

- A wildcard or regex pattern is matched by the regex engine, which can't use a custom comparer, so combining
  `Using(…)` with `AsWildcard()` or `AsRegex()` throws an `InvalidOperationException`.
- Matching a pattern is limited to the
  [`DefaultRegexTimeout`](../02-how-it-works/07-configuration.md#settings), one second by default. A pattern that
  takes longer, e.g. because of catastrophic backtracking, throws an `ArgumentException` that names the timeout and
  asks you to simplify the pattern.

In a regex, `^` and `$` bind to the start and the end of the subject and not to every line, but `$` also matches before
a trailing newline. The regex engine applies its own case folding, which can differ from the other expectations for a
few characters, e.g. the Kelvin sign (U+212A) on modern .NET.

</details>

## One of

You can verify that the `string` is one of many alternatives, with the same [options](#string-options) as for
equality:

```csharp
string title = "Abbey Road";

await That(title).IsOneOf("Help!", "Abbey Road", "Revolver");
await That(title).IsOneOf("HELP!", "ABBEY ROAD", "REVOLVER").IgnoringCase();
await That(title).IsNotOneOf("Help!", "Revolver");
```

## Null, empty or whitespace

You can verify that the `string` is `null`, empty or contains only whitespace:

```csharp
string? title = null;

await That(title).IsNull();
await That("Abbey Road").IsNotNull();

await That("").IsEmpty();
await That("Abbey Road").IsNotEmpty();

await That(title).IsNullOrEmpty();
await That("Abbey Road").IsNotNullOrEmpty();
await That(title).IsNullOrWhiteSpace();
await That("Abbey Road").IsNotNullOrWhiteSpace();
```

## Length

You can verify that the `string` has the expected length:

```csharp
string title = "Abbey Road";

await That(title).HasLength(10);
await That(title).HasLength().Between(8).And(12);
await That(title).HasLength().NotGreaterThan(12);
```

<PropertyComparisons example="HasLength(10)" />

## Lines

You can verify how many lines the `string` has:

```csharp
string lyrics = """
                Come together
                Right now
                Over me
                """;

await That(lyrics).HasLineCount(3);
await That(lyrics).HasLineCount().NotEqualTo(4);
await That(lyrics).HasLineCount().GreaterThan(2);
```

`HasLineCount()` takes the same [comparisons](#length) as `HasLength()`.

You can also verify the lines themselves. `HasLines` applies the expectations on the lines as a collection, so all
[collection expectations](../05-collections/00-index.md) are available:

```csharp
await That(lyrics).HasLines(lines => lines.Contains("Right now"));
await That(lyrics).HasLines(lines => lines.StartsWith("Come together"));
await That(lyrics).HasLines(lines => lines.All().Satisfy(line => line?.Length < 20));
```

Lines are separated by `\r\n`, `\n` or `\r`, which are all treated equivalently. A single trailing line terminator
does not start a new line, so `"Come together\n"` has one line and `""` has none, matching how `File.ReadLines` counts
the lines of a file:

```csharp
await That("").HasLineCount(0);
await That("Come together").HasLineCount(1);
await That("Come together\n").HasLineCount(1);
await That("Come together\n\n").HasLineCount(2);
```

## Start / end

You can verify that the `string` starts or ends with a given string, or that it does not, with the same
[options](#string-options) as for equality:

```csharp
string title = "Abbey Road";

await That(title).StartsWith("Abbey");
await That(title).EndsWith("ROAD").IgnoringCase();
await That(title).DoesNotStartWith("Road");
await That(title).DoesNotEndWith("Abbey");
```

## Contains

You can verify that the `string` contains a given substring, or that it does not, with the same
[options](#string-options) as for equality:

```csharp
string title = "Strawberry Fields Forever";

await That(title).Contains("Fields");
await That(title).Contains("FIELDS").IgnoringCase();
await That(title).DoesNotContain("Penny Lane");
```

You can also specify how often the substring should be found:

```csharp
string lyrics = "get back, get back, get back to where you once belonged.";

// 'get' can be found 3 times
await That(lyrics).Contains("get").MoreThan(1)
  .Because("count should be '> 1'");
await That(lyrics).Contains("get").AtLeast(2)
  .Because("count should be '>= 2'");
await That(lyrics).Contains("get").Exactly(3)
  .Because("count should be '== 3'");
await That(lyrics).Contains("get").AtMost(4)
  .Because("count should be '<= 4'");
await That(lyrics).Contains("get").LessThan(5)
  .Because("count should be '< 5'");
await That(lyrics).Contains("get").Between(1).And(6)
  .Because("count should be '>= 1 AND <= 6'");

await That(lyrics).Contains("help").Never();
await That(lyrics).Contains("where").Once();
await That(lyrics).Contains("back").AtLeast().Twice();
```

## Character casing

You can verify that the characters in a `string` are all upper or lower cased:

```csharp
await That("1ST PLACE").IsUpperCased()
  .Because("it contains no lowercase characters");
await That("1st PLACE").IsNotUpperCased()
  .Because("it contains at least one lowercase character");

await That("1st place").IsLowerCased()
  .Because("it contains no uppercase characters");
await That("1st PLACE").IsNotLowerCased()
  .Because("it contains at least one uppercase character");
```

Letters without an upper-case (lower-case) form, like `ß`, count as upper-cased (lower-cased). Use
`IncludingUncasedLetters()` to also reject them and titlecase letters:

```csharp
await That("STRAßE").IsNotUpperCased().IncludingUncasedLetters()
  .Because("ß is a lowercase letter without an uppercase form");
```

## Parsing

:::info[.NET 8 or later]
The parsing expectations are only available on .NET 8 or later.
:::

You can verify that the `string` can be parsed into a type that implements `IParsable<T>`, and continue with
expectations on the parsed value with `Which`:

```csharp
using System.Globalization;

await That("42").IsParsableInto<int>();
await That("42").IsParsableInto<int>().Which.IsGreaterThan(40);
await That("1,5").IsParsableInto<double>(new CultureInfo("de-DE"))
  .Because("the format provider is passed to `Parse`");

await That("Abbey Road").IsNotParsableInto<int>();
```

A failure shows the exception thrown by `Parse` and keeps it as inner exception.

The same expectations are available for a `ReadOnlySpan<char>` of a type that implements `ISpanParsable<T>` and for a
UTF-8 `ReadOnlySpan<byte>` of a type that implements `IUtf8SpanParsable<T>`:

```csharp
await That("42".AsSpan()).IsParsableInto<int>();
await That("42"u8).IsParsableInto<int>();
```
