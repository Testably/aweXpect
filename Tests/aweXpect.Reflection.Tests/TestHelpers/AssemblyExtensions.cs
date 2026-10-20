using System.Reflection;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class AssemblyExtensions
{
	/// <summary>
	///     Checks if the <paramref name="assembly" /> is strong named.
	/// </summary>
	/// <param name="assembly">The <see cref="Assembly" /> to check.</param>
	public static bool IsStrongNamed(this Assembly? assembly)
		=> assembly?.GetName().GetPublicKeyToken() is { Length: > 0, };
}
