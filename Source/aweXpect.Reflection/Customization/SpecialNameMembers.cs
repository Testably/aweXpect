using System;

namespace aweXpect.Customization;

/// <summary>
///     The kinds of special-name methods that are included when reflecting over methods.
/// </summary>
/// <remarks>
///     By default, special-name methods (operators and property/event accessors) are excluded. Use these flags to opt
///     specific kinds back in via <see cref="ReflectionCustomizationExtensions.IncludedSpecialNameMembers" />.
///     <para />
///     Unlike <see cref="CompilerGeneratedMembers" />, these members are user-written but are nonetheless hidden by
///     default because they are not normally reflected over directly.
/// </remarks>
[Flags]
public enum SpecialNameMembers
{
	/// <summary>
	///     No special-name methods are included (the default).
	/// </summary>
	None = 0,

	/// <summary>
	///     Include operator methods (e.g. <c>op_Equality</c>, <c>op_Addition</c>, …).
	/// </summary>
	Operators = 1 << 0,

	/// <summary>
	///     Include property and event accessor methods (<c>get_</c>, <c>set_</c>, <c>add_</c>, <c>remove_</c>).
	/// </summary>
	Accessors = 1 << 1,

	/// <summary>
	///     Include all special-name methods.
	/// </summary>
	All = Operators | Accessors,
}
