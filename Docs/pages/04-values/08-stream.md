import PropertyComparisons from '../_property-comparisons.md';

# Stream

Describes the possible expectations for `Stream` and `BufferedStream`.

| Expectation                     | Negated            | Summary                                          |
|---------------------------------|--------------------|--------------------------------------------------|
| [`IsReadable`](#capabilities)   | `IsNotReadable`    | supports reading                                 |
| [`IsWritable`](#capabilities)   | `IsNotWritable`    | supports writing                                 |
| [`IsSeekable`](#capabilities)   | `IsNotSeekable`    | supports seeking                                 |
| [`IsReadOnly`](#capabilities)   | `IsNotReadOnly`    | supports reading, but not writing                |
| [`IsWriteOnly`](#capabilities)  | `IsNotWriteOnly`   | supports writing, but not reading                |
| [`HasLength`](#length)          | negated comparison | has the expected length in bytes                 |
| [`HasPosition`](#position)      | negated comparison | is at the expected position                      |
| [`HasBufferSize`](#buffer-size) | negated comparison | a `BufferedStream` with the expected buffer size |

## Capabilities

You can verify what the `Stream` supports, or that it does not support it:

```csharp
Stream playlist = new MemoryStream();

await Expect.That(playlist).IsReadable();
await Expect.That(playlist).IsSeekable();
await Expect.That(playlist).IsWritable();
await Expect.That(playlist).IsNotReadOnly();
await Expect.That(File.Open("album.txt", FileMode.OpenOrCreate, FileAccess.Read)).IsReadOnly()
  .Because("the file was opened with Read access");
await Expect.That(File.Open("album.log", FileMode.OpenOrCreate, FileAccess.Write)).IsWriteOnly()
  .Because("the file was opened with Write access");
```

## Length

You can verify the length of the `Stream`:

```csharp
Stream playlist = new MemoryStream("foo"u8.ToArray());

await Expect.That(playlist).HasLength(3);
await Expect.That(playlist).HasLength().Between(2).And(4);
```

## Position

You can verify the position of the `Stream`:

```csharp
Stream playlist = new MemoryStream("foo"u8.ToArray());
playlist.Seek(2, SeekOrigin.Current);

await Expect.That(playlist).HasPosition(2);
await Expect.That(playlist).HasPosition().GreaterThan(1);
```

## Buffer size

:::note[.NET 8 or later]
The buffer size expectations are only available on .NET 8 or later.
:::

You can verify the buffer size of the `BufferedStream`:

```csharp
BufferedStream playlist = new(new MemoryStream("foo"u8.ToArray()), 2);

await Expect.That(playlist).HasBufferSize(2);
await Expect.That(playlist).HasBufferSize().NotEqualTo(3);
```

## Comparisons

<PropertyComparisons />
