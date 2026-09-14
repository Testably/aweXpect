# Stream

Describes the possible expectations for `Stream` and `BufferedStream`.

## Properties

You can verify, the properties of the `Stream`:

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

You can verify, the length of the `Stream`:

```csharp
Stream subject = new MemoryStream("foo"u8.ToArray());

await Expect.That(subject).HasLength().EqualTo(3);
await Expect.That(subject).HasLength().NotEqualTo(4);

await Expect.That(subject).HasLength().GreaterThan(2);
await Expect.That(subject).HasLength().GreaterThanOrEqualTo(3);
await Expect.That(subject).HasLength().LessThanOrEqualTo(3);
await Expect.That(subject).HasLength().LessThan(4);
```

## Position

You can verify, the position of the `Stream`:

```csharp
Stream subject = new MemoryStream("foo"u8.ToArray());
subject.Seek(2, SeekOrigin.Current);

await Expect.That(subject).HasPosition().EqualTo(2);
await Expect.That(subject).HasPosition().NotEqualTo(0);

await Expect.That(subject).HasPosition().GreaterThan(1);
await Expect.That(subject).HasPosition().GreaterThanOrEqualTo(2);
await Expect.That(subject).HasPosition().LessThanOrEqualTo(2);
await Expect.That(subject).HasPosition().LessThan(3);
```

## Buffer size

You can verify, the buffer size of the `BufferedStream`:

```csharp
BufferedStream subject = new(new MemoryStream("foo"u8.ToArray()), 2);

await Expect.That(subject).HasBufferSize().EqualTo(2);
await Expect.That(subject).HasBufferSize().NotEqualTo(3);

await Expect.That(subject).HasBufferSize().GreaterThan(1);
await Expect.That(subject).HasBufferSize().GreaterThanOrEqualTo(2);
await Expect.That(subject).HasBufferSize().LessThanOrEqualTo(2);
await Expect.That(subject).HasBufferSize().LessThan(3);
```
