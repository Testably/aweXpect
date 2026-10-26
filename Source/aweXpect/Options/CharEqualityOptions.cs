using System;

namespace aweXpect.Options;

internal sealed class CharEqualityOptions
{
	public bool IgnoreCase { get; private set; }

	/// <summary>
	///     Ignores casing when comparing the <see langword="char" />s.
	/// </summary>
	public void IgnoringCase(bool ignoreCase)
		=> IgnoreCase = ignoreCase;

	public bool AreConsideredEqual(char? actual, char? expected)
	{
		if (actual is null || expected is null)
		{
			return actual == expected;
		}

		return IgnoreCase
			? string.Equals(actual.Value.ToString(), expected.Value.ToString(), StringComparison.OrdinalIgnoreCase)
			: actual.Value == expected.Value;
	}

	public override string ToString()
		=> IgnoreCase ? " ignoring case" : "";
}
