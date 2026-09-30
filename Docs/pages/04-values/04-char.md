# Char

Describes the possible expectations for `char` values.

| Expectation                          | Negated                  | Summary                                 |
|--------------------------------------|--------------------------|-----------------------------------------|
| [`IsEqualTo`](#equality)             | `IsNotEqualTo`           | equal to the expected character         |
| [`IsOneOf`](#one-of)                 | `IsNotOneOf`             | equal to one of the expected characters |
| [`IsAnAsciiLetter`](#categories)     | `IsNotAnAsciiLetter`     | an ASCII letter                         |
| [`IsAnAsciiDigit`](#categories)      | `IsNotAnAsciiDigit`      | an ASCII digit                          |
| [`IsAnAsciiHexDigit`](#categories)   | `IsNotAnAsciiHexDigit`   | an ASCII hexadecimal digit              |
| [`IsALetter`](#categories)           | `IsNotALetter`           | a Unicode letter                        |
| [`IsADigit`](#categories)            | `IsNotADigit`            | a decimal digit                         |
| [`IsANumber`](#categories)           | `IsNotANumber`           | a Unicode number                        |
| [`IsUpperCased`](#categories)        | `IsNotUpperCased`        | an upper-case letter                    |
| [`IsLowerCased`](#categories)        | `IsNotLowerCased`        | a lower-case letter                     |
| [`IsAControlCharacter`](#categories) | `IsNotAControlCharacter` | a control character                     |
| [`IsWhiteSpace`](#categories)        | `IsNotWhiteSpace`        | whitespace                              |

A `null` subject, i.e. a `char?`, fails every expectation on this page except equality and one of, as the
[rule for `null` subjects](../03-how-it-works/04-null-subjects.md) says, so both `IsALetter()` and `IsNotALetter()` fail
for it.

## Equality

You can verify that the `char` is equal to another one or not:

```csharp
char initial = 'a';

await Expect.That(initial).IsEqualTo('a');
await Expect.That(initial).IsNotEqualTo('b');
await Expect.That(initial).IsEqualTo('A').IgnoringCase();
```

`IgnoringCase()` compares the characters the same way as `string`s with `IgnoringCase()`, i.e. with
`StringComparison.OrdinalIgnoreCase`.

## One of

You can verify that the `char` is one of many alternatives:

```csharp
char initial = 'a';

await Expect.That(initial).IsOneOf('a', 'b', 'c');
await Expect.That(initial).IsNotOneOf('x', 'y', 'z');
await Expect.That(initial).IsOneOf('A', 'B', 'C').IgnoringCase();
```

## Categories

You can verify the category of the `char`, or that it does not belong to it:

```csharp
await Expect.That('a').IsAnAsciiLetter();
await Expect.That('乐').IsALetter();
await Expect.That('3').IsADigit();
await Expect.That('½').IsANumber().And.IsNotADigit();
await Expect.That('A').IsUpperCased();
await Expect.That('\t').IsWhiteSpace().And.IsAControlCharacter();
await Expect.That(' ').IsNotAControlCharacter();
```

Each expectation follows the corresponding method of `char`:

| Expectation           | Method                                                                                             |
|-----------------------|----------------------------------------------------------------------------------------------------|
| `IsAnAsciiLetter`     | [`char.IsAsciiLetter`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isasciiletter)     |
| `IsAnAsciiDigit`      | [`char.IsAsciiDigit`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isasciidigit)       |
| `IsAnAsciiHexDigit`   | [`char.IsAsciiHexDigit`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isasciihexdigit) |
| `IsALetter`           | [`char.IsLetter`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isletter)               |
| `IsADigit`            | [`char.IsDigit`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isdigit)                 |
| `IsANumber`           | [`char.IsNumber`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isnumber)               |
| `IsUpperCased`        | [`char.IsUpper`](https://learn.microsoft.com/en-us/dotnet/api/system.char.isupper)                 |
| `IsLowerCased`        | [`char.IsLower`](https://learn.microsoft.com/en-us/dotnet/api/system.char.islower)                 |
| `IsAControlCharacter` | [`char.IsControl`](https://learn.microsoft.com/en-us/dotnet/api/system.char.iscontrol)             |
| `IsWhiteSpace`        | [`char.IsWhiteSpace`](https://learn.microsoft.com/en-us/dotnet/api/system.char.iswhitespace)       |

`IsADigit` only accepts decimal digits, while `IsANumber` also accepts characters like `'½'`.

`IsUpperCased` and `IsLowerCased` differ from the [`string` casing expectations](./03-string.md#character-casing),
which only look at cased letters: `'1'` is neither upper-cased nor lower-cased, while `"1"` is both.
