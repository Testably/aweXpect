namespace aweXpect.Options;

internal sealed class CasingOptions
{
	public bool IncludesUncasedLetters { get; set; }

	public override string ToString()
		=> IncludesUncasedLetters ? " including uncased letters" : "";
}
