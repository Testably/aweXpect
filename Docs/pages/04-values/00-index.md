---
description: Overview of the expectations for single values, from booleans and numbers to strings, streams and any object.
---

# Values

Describes the possible expectations for single values, from `bool` and numbers to strings, streams and any object.

| Page                                                           | Expectations                                                                           |
|----------------------------------------------------------------|----------------------------------------------------------------------------------------|
| [Boolean](./01-boolean.md)                                     | `IsTrue`, `IsFalse`, `Implies`                                                         |
| [Number](./02-number.md)                                       | `IsEqualTo`, `IsGreaterThan`, `IsBetween`, `IsPositive`, `IsNaN`, `Within`             |
| [String](./03-string.md)                                       | `IsEqualTo` with wildcards or regex, `StartsWith`, `Contains`, `HasLength`             |
| [Char](./04-char.md)                                           | `IsEqualTo`, categories like `IsALetter` or `IsWhiteSpace`                             |
| [Enum](./05-enum.md)                                           | `IsEqualTo`, `HasValue`, `IsDefined`, `HasFlag`                                        |
| [Guid](./06-guid.md)                                           | `IsEqualTo`, `IsEmpty`, `IsNullOrEmpty`                                                |
| [Version](./07-version.md)                                     | `IsEqualTo`, `IsGreaterThan`, `IsBetween`, `HasMajor`                                  |
| [Stream](./08-stream.md)                                       | `IsReadable`, `IsSeekable`, `HasLength`, `HasPosition`                                 |
| [TimeSpan](./09-timespan.md)                                   | `IsEqualTo`, `IsGreaterThan`, `IsBetween`, `IsPositive`, `Within`                      |
| [DateTime / DateTimeOffset](./10-datetime-offset.md)           | `IsEqualTo`, `IsAfter`, `IsBefore`, `IsBetween`, `HasYear`, `Within`                   |
| [DateOnly / TimeOnly](./11-date-time-only.md)                  | `IsEqualTo`, `IsAfter`, `IsBefore`, `IsBetween`, `HasYear`                             |
| [Exception](../06-behavior/01-delegates.md#exception-details) | `HasMessage`, `HasParamName`, `HasHResult`, `HasInner`, `HasRecursiveInnerExceptions`  |
| [Object](./12-object.md)                                       | `IsEqualTo`, `IsSameAs`, `Is<T>`, `IsNull`, `Satisfies`, `CompliesWith`, `IsCollected` |
| [Equivalency](./13-equivalency.md)                             | `IsEquivalentTo` and its options                                                       |

A few rules apply to all of these pages:

- The [object expectations](./12-object.md), such as `IsNull`, `IsSameAs`, `Is<T>`, `Satisfies`, `CompliesWith` or
  `IsEquivalentTo`, work on every subject, also on the types of the other pages.
- A `null` subject fails every expectation that inspects it, and its negation as well, as the
  [rule for `null` subjects](../02-how-it-works/04-null-subjects.md) describes.
- The `Has…` expectations, such as `HasLength()` or `HasYear()`, continue with a comparison like `GreaterThan` or
  `Between`, as shown for the [length of a string](./03-string.md#length). They have no negated twin, so the overview
  tables list "negated comparison" for them: negate the comparison instead, e.g. `HasLength().NotEqualTo(3)`.
- The [options](../02-how-it-works/05-options.md) page lists the options that several expectations share, e.g. a
  tolerance with `Within` or the string options.
