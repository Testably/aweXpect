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
	public static PropertyResult.Long<Stream?> HasPosition(this IThat<Stream?> subject)
		=> new(subject, a => a?.Position, "position", (value, paramName) =>
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(paramName, value,
					// ReSharper disable once LocalizableElement
					$"The {paramName} position must be greater than or equal to zero.");
			}
		});

	/// <summary>
	///     Verifies that the position of the <see cref="Stream" /> subject is equal to the <paramref name="expected" />
	///     value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Stream?, IThat<Stream?>> HasPosition(
		this IThat<Stream?> subject,
		long expected)
		=> subject.HasPosition().EqualTo(expected);
}
