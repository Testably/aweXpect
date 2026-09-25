# Guid

Describes the possible expectations for `Guid` values.

## Equality

You can verify that the `Guid` is equal to another one or not:

```csharp
Guid subject = Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc");

await Expect.That(subject).IsEqualTo(Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc"))
  .Because("they are the same");
await Expect.That(subject).IsNotEqualTo(Guid.Parse("cdd7a485-40a1-4bba-bb8b-d0e903704b02"))
  .Because("they differ");
```

## One of

You can verify that the `Guid` is one of many alternatives:

```csharp
Guid subject = Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc");

await Expect.That(subject).IsOneOf(
  Guid.Parse("5c01d9d2-66f7-4782-8c14-e54eae9aaacc"),
  Guid.Parse("cdd7a485-40a1-4bba-bb8b-d0e903704b02"));
await Expect.That(subject).IsNotOneOf(
  Guid.Parse("cdd7a485-40a1-4bba-bb8b-d0e903704b02"),
  Guid.Parse("f1a0a5f4-1f63-4dd4-8d42-6d2f7a0a1c11"));
```

## Empty

You can verify that the `Guid` is (null or) empty or not:

```csharp
await Expect.That(Guid.Empty).IsEmpty();
await Expect.That(Guid.NewGuid()).IsNotEmpty();

Guid? guid1 = Guid.Empty;
await Expect.That(guid1).IsNullOrEmpty();
Guid? guid2 = Guid.NewGuid();
await Expect.That(guid2).IsNotNullOrEmpty();
```
