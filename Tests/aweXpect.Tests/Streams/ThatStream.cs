using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aweXpect.Tests;

public sealed partial class ThatStream
{
	/// <summary>
	///     A <see cref="Stream" /> that is also a collection, so that a member of this type reads as plural.
	/// </summary>
	public sealed class ChunkedStream(bool canRead = false, bool canWrite = false, bool canSeek = false)
		: MyStream(canRead: canRead, canWrite: canWrite, canSeek: canSeek), IEnumerable<byte[]>
	{
		/// <inheritdoc />
		public IEnumerator<byte[]> GetEnumerator()
			=> Enumerable.Empty<byte[]>().GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}

	public sealed class ChunkedStreamContainer(ChunkedStream chunks)
	{
		public ChunkedStream Chunks { get; } = chunks;
	}

	public class MyStream(
		byte[]? buffer = null,
		bool canRead = false,
		bool canWrite = false,
		bool canSeek = false,
		long position = 0,
		long length = 0)
		: Stream
	{
		private readonly byte[] _buffer = buffer ?? Array.Empty<byte>();

		/// <inheritdoc />
		public override bool CanRead { get; } = canRead;

		/// <inheritdoc />
		public override bool CanSeek { get; } = canSeek;

		/// <inheritdoc />
		public override bool CanWrite { get; } = canWrite;

		/// <inheritdoc />
		public override long Length { get; } = length;

		/// <inheritdoc />
		public override long Position { get; set; } = position;

		/// <inheritdoc />
		public override void Flush()
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override void SetLength(long value)
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
			=> throw new NotSupportedException();
	}

	/// <summary>
	///     A <see cref="Stream" /> which cannot tell its length or position, like a non-seekable or a broken stream.
	/// </summary>
	public sealed class UnreadableStream(Exception exception) : Stream
	{
		/// <inheritdoc />
		public override bool CanRead => false;

		/// <inheritdoc />
		public override bool CanSeek => false;

		/// <inheritdoc />
		public override bool CanWrite => false;

		/// <inheritdoc />
		public override long Length => throw exception;

		/// <inheritdoc />
		public override long Position
		{
			get => throw exception;
			set => throw exception;
		}

		/// <inheritdoc />
		public override void Flush()
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override void SetLength(long value)
			=> throw new NotSupportedException();

		/// <inheritdoc />
		public override void Write(byte[] buffer, int offset, int count)
			=> throw new NotSupportedException();
	}
}
