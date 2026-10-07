using System.Collections.Generic;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     Compares strings as versions, and throws for a string that is not a complete version, like a part of one.
/// </summary>
public sealed class VersionStringComparer : IEqualityComparer<string>
{
	public bool Equals(string? x, string? y) => Version.Parse(x!).Equals(Version.Parse(y!));

	public int GetHashCode(string obj) => Version.Parse(obj).GetHashCode();
}
