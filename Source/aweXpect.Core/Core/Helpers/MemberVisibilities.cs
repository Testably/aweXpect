#nullable disable
using System;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Determines which members are included in the equivalency constraint.
/// </summary>
[Flags]
public enum MemberVisibilities
{
	/// <summary>
	///     No visibilities.
	/// </summary>
	None = 0,

	/// <summary>
	///     Internal.
	/// </summary>
	Internal = 1,

	/// <summary>
	///     Public.
	/// </summary>
	Public = 2,

	/// <summary>
	///     Explicitly implemented.
	/// </summary>
	ExplicitlyImplemented = 4,
}
