using System;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Specifies which members are included when navigating from types to their members.
/// </summary>
/// <remarks>
///     Inherited members never include the <see langword="private" /> members of base types, nor the
///     <see langword="internal" /> members of base types in another assembly, as a derived type cannot access them.
/// </remarks>
[Flags]
internal enum MemberScope
{
	/// <summary>
	///     Include the inherited members, except those declared on <see cref="object" />, <see cref="ValueType" /> and
	///     <see cref="Enum" />.
	/// </summary>
	Inherited = 0,

	/// <summary>
	///     Only include members declared directly on the type.
	/// </summary>
	DeclaredOnly = 1,

	/// <summary>
	///     Also include the inherited members declared on <see cref="object" />, <see cref="ValueType" /> and
	///     <see cref="Enum" />.
	/// </summary>
	IncludingObjectMembers = 2,

	/// <summary>
	///     Also include the <see langword="private" /> fields and properties of base types and those a derived type
	///     cannot access, which the immutability check needs, as they still hold the state of an instance.
	/// </summary>
	IncludingPrivateBaseMembers = 4,
}
