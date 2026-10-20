using System.Collections.Generic;

namespace aweXpect.Reflection.Tests.TestHelpers;

/// <summary>
///     A test <see cref="IEqualityComparer{T}" /> for <see langword="string" />s that
///     ignores case differences only in vocals.
/// </summary>
public sealed class IgnoreCaseForVocalsComparer : IEqualityComparer<string>
{
	private static string? LowercaseVocals(string? input)
		=> input?.Replace('A', 'a')
			.Replace('E', 'e')
			.Replace('I', 'i')
			.Replace('O', 'o')
			.Replace('U', 'u');

	#region IEqualityComparer<string> Members

	public bool Equals(string? x, string? y)
	{
		string? adjustedX = LowercaseVocals(x);
		string? adjustedY = LowercaseVocals(y);

		return adjustedX?.Equals(adjustedY, StringComparison.Ordinal) == true;
	}

	public int GetHashCode(string obj) => obj.GetHashCode();

	#endregion
}
