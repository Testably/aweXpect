#if NET8_0_OR_GREATER
using System.IO;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatBufferedStream
{
	/// <summary>
	///     Verifies that the buffer size of the <see cref="BufferedStream" /> subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<BufferedStream?> HasBufferSize(this IThat<BufferedStream?> subject)
		=> new(subject, a => a?.BufferSize, "buffer size");

	/// <summary>
	///     Verifies that the buffer size of the <see cref="BufferedStream" /> subject is equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<BufferedStream?, IThat<BufferedStream?>> HasBufferSize(
		this IThat<BufferedStream?> subject,
		int expected)
		=> subject.HasBufferSize().EqualTo(expected);
}
#endif
