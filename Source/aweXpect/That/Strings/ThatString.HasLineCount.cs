using System;
using System.Linq;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the number of lines of the <see langword="string" /> subject…
	/// </summary>
	/// <remarks>
	///     Lines are separated by <c>\r\n</c>, <c>\n</c> or <c>\r</c>.<br />
	///     A single trailing line terminator does not start a new line, so <c>"a\nb\n"</c> has two lines.
	/// </remarks>
	public static PropertyResult.Int<string?> HasLineCount(this IThat<string?> source)
		=> new(source, a => a?.GetLines().Count(), "line count", (value, paramName) =>
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(paramName, value,
					// ReSharper disable once LocalizableElement
					$"The {paramName} line count must be greater than or equal to zero.");
			}
		});
}
