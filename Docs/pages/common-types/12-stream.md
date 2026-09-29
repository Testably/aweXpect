import PropertyComparisons from '../_property-comparisons.md';

# Stream

Describes the possible expectations for `Stream` and `BufferedStream`.

Every expectation has a negated counterpart (`IsNot…`/`DoesNot…`), except the `Has…` properties, which take a negated
comparison instead (e.g. `HasLength().NotEqualTo(4)`).

## Properties

You can verify the properties of the `Stream`:

```csharp
Stream subject = new MemoryStream();

await Expect.That(subject).IsReadable();
await Expect.That(subject).IsSeekable();
await Expect.That(subject).IsWritable();
await Expect.That(File.Open("read-only.txt", FileMode.OpenOrCreate, FileAccess.Read)).IsReadOnly()
  .Because("the file was opened with Read access");
await Expect.That(File.Open("write-only.txt", FileMode.OpenOrCreate, FileAccess.Write)).IsWriteOnly()
  .Because("the file was opened with Write access");
```

## Length

You can verify the length of the `Stream`:

```csharp
Stream subject = new MemoryStream("foo"u8.ToArray());

await Expect.That(subject).HasLength(3);
// or more explicit
await Expect.That(subject).HasLength().EqualTo(3);

await Expect.That(subject).HasLength().Between(2).And(4);
```

## Position

You can verify the position of the `Stream`:

```csharp
Stream subject = new MemoryStream("foo"u8.ToArray());
subject.Seek(2, SeekOrigin.Current);

await Expect.That(subject).HasPosition(2);
// or more explicit
await Expect.That(subject).HasPosition().EqualTo(2);

await Expect.That(subject).HasPosition().GreaterThan(1);
```

## Buffer size

You can verify the buffer size of the `BufferedStream`:

```csharp
BufferedStream subject = new(new MemoryStream("foo"u8.ToArray()), 2);

await Expect.That(subject).HasBufferSize(2);
// or more explicit
await Expect.That(subject).HasBufferSize().EqualTo(2);

await Expect.That(subject).HasBufferSize().NotEqualTo(3);
```

:::note
The buffer size expectations are only available on .NET 8 or later.
:::

## Comparisons

<PropertyComparisons />
