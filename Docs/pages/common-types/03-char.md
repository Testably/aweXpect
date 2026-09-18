# Char

Describes the possible expectations for `char` values.

## Equality

You can verify that the `char` is equal to another one.

```csharp
char subject = 'a';

await Expect.That(subject).IsEqualTo('a');
await Expect.That(subject).IsNotEqualTo('b');
```

## One of

You can verify that the `char` is one of many alternatives.

```csharp
char subject = 'a';

await Expect.That(subject).IsOneOf('a', 'b', 'c');
await Expect.That(subject).IsNotOneOf('x', 'y', 'z');
```

## Is

### An ASCII letter

You can verify that the `char` is an ASCII letter.

```csharp
await Expect.That('a').IsAnAsciiLetter();
```

This verifies that the subject is an ASCII letter
(see [`char.IsAsciiLetter(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isasciiletter)).

### An ASCII digit

You can verify that the `char` is an ASCII digit.

```csharp
await Expect.That('3').IsAnAsciiDigit();
```

This verifies that the subject is an ASCII digit
(see [`char.IsAsciiDigit(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isasciidigit)).

### An ASCII hex digit

You can verify that the `char` is an ASCII hexadecimal digit.

```csharp
await Expect.That('f').IsAnAsciiHexDigit();
```

This verifies that the subject is an ASCII hexadecimal digit
(see [`char.IsAsciiHexDigit(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isasciihexdigit)).

### A letter

You can verify that the `char` is a letter.

```csharp
await Expect.That('a').IsALetter();
await Expect.That('乐').IsALetter();
```

This verifies that the subject is categorized as a Unicode letter
(see [`char.IsLetter(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isletter)).

### A digit

You can verify that the `char` is a digit.

```csharp
await Expect.That('3').IsADigit();
```

This verifies that the subject is categorized as a decimal digit
(see [`char.IsDigit(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isdigit)).
In contrast to `IsANumber()`, this excludes characters like `'½'`.

### A number

You can verify that the `char` is a number.

```csharp
await Expect.That('3').IsANumber();
```

This verifies that the subject is categorized as a Unicode number
(see [`char.IsNumber(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isnumber)).

### Upper-cased / Lower-cased

You can verify that the `char` is an uppercase or a lowercase letter.

```csharp
await Expect.That('A').IsUpperCased();
await Expect.That('a').IsLowerCased();
```

This verifies that the subject is categorized as an uppercase or lowercase letter
(see [`char.IsUpper(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.isupper) and
[`char.IsLower(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.islower)).

### A control character

You can verify that the `char` is a control character.

```csharp
await Expect.That('\n').IsAControlCharacter();
```

This verifies that the subject is categorized as a control character
(see [`char.IsControl(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.iscontrol)).

### White-Space

You can verify that the `char` is white-space.

```csharp
await Expect.That('\t').IsWhiteSpace();
```

This verifies that the subject is categorized as white-space
(see [`char.IsWhiteSpace(char)`](https://learn.microsoft.com/de-de/dotnet/api/system.char.iswhitespace)).
