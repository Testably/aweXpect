using System;

namespace aweXpect.Customization;

/// <summary>
///     The kinds of compiler-generated members that are included when reflecting over types and members.
/// </summary>
/// <remarks>
///     By default, compiler-generated members (e.g. closures, state machines, record members, backing fields) are
///     excluded. Use these flags to opt specific kinds back in via
///     <see cref="ReflectionCustomizationExtensions.IncludedCompilerGeneratedMembers" />.
/// </remarks>
[Flags]
public enum CompilerGeneratedMembers
{
	/// <summary>
	///     No compiler-generated members are included (the default).
	/// </summary>
	None = 0,

	/// <summary>
	///     Include compiler-generated types (e.g. closures, async/iterator state machines, anonymous types).
	/// </summary>
	Types = 1 << 0,

	/// <summary>
	///     Include compiler-generated constructors (e.g. the copy-constructor of a record).
	/// </summary>
	Constructors = 1 << 1,

	/// <summary>
	///     Include compiler-generated methods (e.g. local functions, lambda bodies, record members such as
	///     <c>ToString</c>, <c>Equals</c>, <c>&lt;Clone&gt;$</c>, <c>Deconstruct</c> and <c>PrintMembers</c>).
	/// </summary>
	Methods = 1 << 2,

	/// <summary>
	///     Include compiler-generated properties (e.g. the <c>EqualityContract</c> of a record).
	/// </summary>
	Properties = 1 << 3,

	/// <summary>
	///     Include compiler-generated fields (e.g. auto-property backing fields or cached-delegate fields).
	/// </summary>
	Fields = 1 << 4,

	/// <summary>
	///     Include compiler-generated events.
	/// </summary>
	Events = 1 << 5,

	/// <summary>
	///     Include all compiler-generated members.
	/// </summary>
	All = Types | Constructors | Methods | Properties | Fields | Events,
}
