using System;

namespace aweXpect.Options;

internal sealed class CharEqualityOptions
{
	public bool IgnoreCase { get; set; }

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
