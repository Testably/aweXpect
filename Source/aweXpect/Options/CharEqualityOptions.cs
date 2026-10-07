using System;
using aweXpect.Helpers;

namespace aweXpect.Options;

internal sealed class CharEqualityOptions
{
	private bool _isIgnoreCaseSpecified;

	public bool IgnoreCase { get; private set; }

	/// <summary>
	///     Ignores casing when comparing the <see langword="char" />s.
	/// </summary>
	/// <exception cref="InvalidOperationException">The casing is already specified.</exception>
	public void IgnoringCase(bool ignoreCase)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIgnoreCaseSpecified, nameof(IgnoringCase));
		_isIgnoreCaseSpecified = true;
		IgnoreCase = ignoreCase;
	}

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
