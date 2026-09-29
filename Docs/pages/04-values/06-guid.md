# Guid

Describes the possible expectations for `Guid` values.

| Expectation               | Negated            | Summary                               |
|---------------------------|--------------------|---------------------------------------|
| [`IsEqualTo`](#equality)  | `IsNotEqualTo`     | equal to the expected value           |
| [`IsOneOf`](#one-of)      | `IsNotOneOf`       | equal to one of the expected values   |
| [`IsEmpty`](#empty)       | `IsNotEmpty`       | `Guid.Empty`                          |
| [`IsNullOrEmpty`](#empty) | `IsNotNullOrEmpty` | `null` or `Guid.Empty` (only `Guid?`) |

## Equality

You can verify that the `Guid` is equal to another one or not:

```csharp
Guid albumId = Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc");

await Expect.That(albumId).IsEqualTo(Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc"));
await Expect.That(albumId).IsNotEqualTo(Guid.Parse("cdd7a485-40a1-4bba-bb8b-d0e903704b02"));
```

## One of

You can verify that the `Guid` is one of many alternatives:

```csharp
Guid albumId = Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc");

await Expect.That(albumId).IsOneOf(
  Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc"),
  Guid.Parse("cdd7a485-40a1-4bba-bb8b-d0e903704b02"));
await Expect.That(albumId).IsNotOneOf(
  Guid.Parse("cdd7a485-40a1-4bba-bb8b-d0e903704b02"),
  Guid.Parse("f1a0a5f4-1f63-4dd4-8d42-6d2f7a0a1c11"));
```

## Empty

You can verify that the `Guid` is (`null` or) empty or not:

```csharp
await Expect.That(Guid.Empty).IsEmpty();
await Expect.That(Guid.NewGuid()).IsNotEmpty();

Guid? missingId = Guid.Empty;
await Expect.That(missingId).IsNullOrEmpty();
Guid? albumId = Guid.NewGuid();
await Expect.That(albumId).IsNotNullOrEmpty();
```
