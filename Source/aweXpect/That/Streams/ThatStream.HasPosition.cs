using System;
using System.IO;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatStream
{
	/// <summary>
	///     Verifies that the position of the <see cref="Stream" /> subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Long<Stream?, TStream, IThat<TStream?>> HasPosition<TStream>(
		this IThat<TStream?> subject)
		where TStream : Stream
		=> new(subject, a => a?.Position, "position", (value, paramName) =>
		{
			if (value < 0)
			{
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(paramName, value,
						// ReSharper disable once LocalizableElement
						$"The {paramName} position must not be negative."));
			}
		});

	/// <summary>
	///     Verifies that the position of the <see cref="Stream" /> subject is equal to the <paramref name="expected" />
	///     value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TStream, IThat<TStream?>> HasPosition<TStream>(
		this IThat<TStream?> subject,
		long? expected)
		where TStream : Stream
		=> subject.HasPosition().EqualTo(expected);
}
