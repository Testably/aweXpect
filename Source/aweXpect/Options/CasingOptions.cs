using System;
using aweXpect.Helpers;

namespace aweXpect.Options;

internal sealed class CasingOptions
{
	private bool _isIncludingUncasedLettersSpecified;

	public bool IncludesUncasedLetters { get; private set; }

	/// <summary>
	///     Also rejects letters without an upper-case (lower-case) form and titlecase letters,
	///     according to the <paramref name="includeUncasedLetters" /> parameter.
	/// </summary>
	/// <exception cref="InvalidOperationException">The uncased letters are already specified.</exception>
	public void IncludingUncasedLetters(bool includeUncasedLetters)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIncludingUncasedLettersSpecified,
			nameof(IncludingUncasedLetters));
		_isIncludingUncasedLettersSpecified = true;
		IncludesUncasedLetters = includeUncasedLetters;
	}

	public override string ToString()
		=> IncludesUncasedLetters ? " including uncased letters" : "";
}
