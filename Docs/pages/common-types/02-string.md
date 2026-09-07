# String

Describes the possible expectations for strings.

## Equality

You can verify that the `string` is equal to another one.  
This expectation can be configured to ignore case, ignore newline style, ignore the indentation, ignoring leading or
trailing white-space, or use a custom `IEqualityComparer<string>`:

```csharp
string subject = "Abbey Road";

await Expect.That(subject).IsEqualTo("Abbey Road")
  .Because("it is equal");
await Expect.That(subject).IsEqualTo("ABBEY ROAD").IgnoringCase()
  .Because("we ignored the casing");
await Expect.That("Abbey\r\nRoad").IsEqualTo("Abbey\nRoad").IgnoringNewlineStyle()
  .Because("we ignored the newline style");
await Expect.That("  Abbey\n    Road").IsEqualTo("Abbey\nRoad").IgnoringIndentation()
  .Because("we ignored the indentation of every line");
await Expect.That(subject).IsEqualTo("  Abbey Road").IgnoringLeadingWhiteSpace()
  .Because("we ignored leading white-space");
await Expect.That(subject).IsEqualTo("Abbey Road \t").IgnoringTrailingWhiteSpace()
  .Because("we ignored trailing white-space");
await Expect.That(subject).IsEqualTo("ABBEY ROAD").Using(StringComparer.OrdinalIgnoreCase)
  .Because("the comparer ignored the casing");
```

### Indentation

While `IgnoringLeadingWhiteSpace` only trims the start of the complete `string`, `IgnoringIndentation` removes the
leading white-space from *every* line. This allows comparing against a raw string literal that is indented differently
than the subject:

```csharp
string subject = """
                 public class Beatles
                 {
                     public string Album => "Abbey Road";
                 }
                 """;

await Expect.That(subject).Contains("""
                                    public string Album => "Abbey Road";
                                    """).IgnoringIndentation();
```

As the lines are split on `\r\n`, `\n` and `\r`, this also normalizes the newline style, which makes
`IgnoringNewlineStyle` redundant.  
Trailing white-space within a line is kept, but a line that consists only of white-space becomes empty.  
To keep the relative indentation within the snippet, use [`AsBlock`](#blocks) instead.

### Wildcards

You can also compare strings using wildcards:

```csharp
string subject = "some text";

await Expect.That(subject).IsEqualTo("*me tex?").AsWildcard();
```

When using `AsWildcard`, the following wildcard specifiers are supported:

| Wildcard specifier | Matches                 |
|--------------------|-------------------------|
| * (asterisk)       | Zero or more characters |
| ? (question mark)  | Exactly one character   |

### Regular expressions

You can also compare strings
using [regular expressions](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions):

```csharp
string subject = "some text";

await Expect.That(subject).IsEqualTo("(.*)xt").AsRegex();
```

The regex comparison uses the following [
`options`](https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regexoptions?view=net-8.0#fields):

- `Multiline` (always)
- `IgnoreCase` (if the `IgnoringCase` method is also used)

### Prefix / Suffix

You can also verify that the subject starts with or ends with a given string.

```csharp
string subject = "Abbey Road";

await Expect.That(subject).IsEqualTo("Abbey").AsPrefix();
await Expect.That(subject).IsEqualTo("Road").AsSuffix();
```

## One of

You can verify that the `string` is one of many alternatives.  
This expectation can be configured to ignore case, ignore newline style, ignore the indentation, ignoring leading or
trailing white-space, or use a custom `IEqualityComparer<string>`:

```csharp
string subject = "Abbey Road";

await Expect.That(subject).IsOneOf("Help!", "Abbey Road", "Revolver");
await Expect.That(subject).IsOneOf("HELP!", "ABBEY ROAD", "REVOLVER").IgnoringCase()
  .Because("we ignored the casing");
await Expect.That(subject).IsOneOf("HELP!", "ABBEY ROAD", "REVOLVER").Using(StringComparer.OrdinalIgnoreCase)
  .Because("the comparer ignored the casing");
```

## Null, empty or white-space

You can verify that the `string` is null, empty or contains only whitespace:

```csharp
string? subject = null;

await Expect.That(subject).IsNull();
await Expect.That("Abbey Road").IsNotNull();

await Expect.That("").IsEmpty();
await Expect.That("Abbey Road").IsNotEmpty()
  .Because("the string is not empty");

await Expect.That(subject).IsNullOrEmpty();
await Expect.That("Abbey Road").IsNotNullOrEmpty();
await Expect.That(subject).IsNullOrWhiteSpace();
await Expect.That("Abbey Road").IsNotNullOrWhiteSpace();
```

## Length

You can verify that the `string` has the expected length:

```csharp
string subject = "Abbey Road";

await Expect.That(subject).HasLength().EqualTo(10);
await Expect.That(subject).HasLength().NotEqualTo(9);

await Expect.That(subject).HasLength().GreaterThan(8);
await Expect.That(subject).HasLength().GreaterThanOrEqualTo(9);
await Expect.That(subject).HasLength().LessThanOrEqualTo(11);
await Expect.That(subject).HasLength().LessThan(12);
```

## Lines

You can verify how many lines the `string` has:

```csharp
string subject = """
                 Come together
                 Right now
                 Over me
                 """;

await Expect.That(subject).HasLineCount().EqualTo(3);
await Expect.That(subject).HasLineCount().NotEqualTo(4);
await Expect.That(subject).HasLineCount().GreaterThan(2);
await Expect.That(subject).HasLineCount().LessThanOrEqualTo(3);
```

You can also verify the lines themselves. `HasLines` applies the expectations on the lines as a
collection, so all collection expectations are available:

```csharp
await Expect.That(subject).HasLines(lines => lines.Contains("Right now"));
await Expect.That(subject).HasLines(lines => lines.DoesNotContain("Something"));
await Expect.That(subject).HasLines(lines => lines.StartsWith("Come together"));
await Expect.That(subject).HasLines(lines => lines.All().Satisfy(line => line?.Length < 20));
```

Lines are separated by `\r\n`, `\n` or `\r`, which are all treated equivalently.  
A single trailing line terminator does not start a new line, so `"Come together\n"` has one line and
`""` has none — matching how `File.ReadLines` counts the lines of a file:

```csharp
await Expect.That("").HasLineCount().EqualTo(0);
await Expect.That("Come together").HasLineCount().EqualTo(1);
await Expect.That("Come together\n").HasLineCount().EqualTo(1);
await Expect.That("Come together\n\n").HasLineCount().EqualTo(2);
```

## String start / end

You can verify that the `string` starts or ends with a given string.  
These expectations can be configured to ignore case, ignore newline style, ignore the indentation, ignoring leading or
trailing white-space, or use a custom `IEqualityComparer<string>`:

```csharp
string subject = "Abbey Road";

await Expect.That(subject).StartsWith("Abbey");
await Expect.That(subject).StartsWith("ABBEY").IgnoringCase()
  .Because("we ignored the casing");
await Expect.That(subject).StartsWith("ABBEY").Using(StringComparer.OrdinalIgnoreCase)
  .Because("the comparer ignored the casing");

await Expect.That(subject).EndsWith("Road");
await Expect.That(subject).EndsWith("ROAD").IgnoringCase()
  .Because("we ignored the casing");
await Expect.That(subject).EndsWith("ROAD").Using(StringComparer.OrdinalIgnoreCase)
  .Because("the comparer ignored the casing");
```

## Contains

You can verify that the `string` contains a given substring.  
These expectations can be configured to ignore case, ignore newline style, ignore the indentation, ignoring leading or
trailing white-space, or use a custom `IEqualityComparer<string>`:

```csharp
string subject = "Strawberry Fields Forever";

await Expect.That(subject).Contains("Fields");
await Expect.That(subject).Contains("FIELDS").IgnoringCase()
  .Because("we ignored the casing");
await Expect.That(subject).Contains("FIELDS").Using(StringComparer.OrdinalIgnoreCase)
  .Because("the comparer ignored the casing");
```

You can also specify, how often the substring should be found:

```csharp
string subject = "get back, get back, get back to where you once belonged.";

// 'get' can be found 3 times
await Expect.That(subject).Contains("get").MoreThan(1)
  .Because("count should be '> 1'");
await Expect.That(subject).Contains("get").AtLeast(2)
  .Because("count should be '>= 2'");
await Expect.That(subject).Contains("get").Exactly(3)
  .Because("count should be '== 3'");
await Expect.That(subject).Contains("get").AtMost(4)
  .Because("count should be '<= 4'");
await Expect.That(subject).Contains("get").LessThan(5)
  .Because("count should be '< 5'");
await Expect.That(subject).Contains("get").Between(1).And(6)
  .Because("count should be '>= 1 AND <= 6'");
```

### Blocks

While `IgnoringIndentation` removes the leading white-space from every line, `AsBlock` keeps the relative
indentation within the expected block and only allows the block as a whole to be indented in the subject.
This is stricter, as a line that is indented differently from the rest of the block does not match:

```csharp
string subject = """
                 public class Beatles
                 {
                     public string Album
                     {
                         get;
                     }
                 }
                 """;

await Expect.That(subject).Contains("""
                                    public string Album
                                    {
                                        get;
                                    }
                                    """).AsBlock();
```

The block must start and end at line boundaries, and all its lines must share the same white-space prefix in the
subject. A line that consists only of white-space matches any line that consists only of white-space.  
The newline style is always ignored, and a single trailing line terminator does not start a new line, so
`"a\nb\n"` has the same two lines as `"a\nb"` (as for [lines](#lines)).  
`AsBlock` can be combined with `IgnoringCase`, `Using` and the count quantifiers.

## Character casing

You can verify that the characters in a `string` are all upper or lower cased:

```csharp
await Expect.That("1ST PLACE").IsUpperCased()
  .Because("it contains no lowercase characters");
await Expect.That("1st PLACE").IsNotUpperCased()
  .Because("it contains at least one lowercase characters");

await Expect.That("1st place").IsLowerCased()
  .Because("it contains no uppercase characters");
await Expect.That("1st PLACE").IsNotLowerCased()
  .Because("it contains at least one uppercase characters");
```
