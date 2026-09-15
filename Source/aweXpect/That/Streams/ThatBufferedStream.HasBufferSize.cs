#if NET8_0_OR_GREATER
using System;
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
		=> new(subject, a => a?.BufferSize, "buffer size", (value, paramName) =>
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(paramName, value,
					// ReSharper disable once LocalizableElement
					$"The {paramName} buffer size must be greater than or equal to zero.");
			}
		});
}
#endif
