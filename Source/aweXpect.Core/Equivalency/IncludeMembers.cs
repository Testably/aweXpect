using System;

namespace aweXpect.Equivalency;

/// <summary>
///     Specifies which members to include in the object comparison.
/// </summary>
/// <remarks>
///     Protected and private members are never compared, because they are implementation details of a type, except
///     for <c>protected internal</c> members, which <see cref="Internal" /> includes.
/// </remarks>
[Flags]
public enum IncludeMembers
{
	/// <summary>
	///     No members should be included in the object comparison.
	/// </summary>
	None = 0,

	/// <summary>
	///     Public members should be included in the object comparison.
	/// </summary>
	Public = 1 << 1,

	/// <summary>
	///     Internal members should be included in the object comparison.
	/// </summary>
	/// <remarks>
	///     This includes <c>protected internal</c> members, because the whole assembly can access them, but not
	///     <c>private protected</c> members.
	/// </remarks>
	Internal = 1 << 2,
}
