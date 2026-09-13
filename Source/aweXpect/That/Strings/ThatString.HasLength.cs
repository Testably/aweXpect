using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the length of the <see langword="string" /> subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<string?> HasLength(this IThat<string?> source)
		=> new(source, a => a?.Length, "length", (value, paramName) =>
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(paramName, value,
					// ReSharper disable once LocalizableElement
					$"The {paramName} length must be greater than or equal to zero.");
			}
		});
}
