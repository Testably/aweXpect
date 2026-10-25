namespace aweXpect.Options;

internal sealed class CasingOptions
{
	public bool IncludesUncasedLetters { get; private set; }

	/// <summary>
	///     Also rejects letters without an upper-case (lower-case) form and titlecase letters,
	///     according to the <paramref name="includeUncasedLetters" /> parameter.
	/// </summary>
	public void IncludingUncasedLetters(bool includeUncasedLetters)
		=> IncludesUncasedLetters = includeUncasedLetters;

	public override string ToString()
		=> IncludesUncasedLetters ? " including uncased letters" : "";
}
