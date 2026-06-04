namespace aweXpect.Reflection;

/// <summary>
///     The C# operators that the compiler emits as <c>op_*</c> special-name methods.
/// </summary>
/// <remarks>
///     Each member maps to its compiler-emitted metadata name by prefixing <c>op_</c> (e.g.
///     <see cref="Addition" /> maps to <c>op_Addition</c>). This is a plain (non-<c>[Flags]</c>) enum because combining
///     operators would be ambiguous in an assertion.<br />
///     The conversion operators (<see cref="Implicit" />, <see cref="Explicit" />, <see cref="CheckedExplicit" />) are
///     keyed by their source → target signature; use the dedicated conversion operator assertions to match a specific
///     conversion.
/// </remarks>
public enum Operator
{
	/// <summary>
	///     An implicit conversion operator (<c>op_Implicit</c>).
	/// </summary>
	Implicit,

	/// <summary>
	///     An explicit conversion operator (<c>op_Explicit</c>).
	/// </summary>
	Explicit,

	/// <summary>
	///     A checked explicit conversion operator (<c>op_CheckedExplicit</c>).
	/// </summary>
	CheckedExplicit,

	/// <summary>
	///     The unary plus operator <c>+</c> (<c>op_UnaryPlus</c>).
	/// </summary>
	UnaryPlus,

	/// <summary>
	///     The unary negation operator <c>-</c> (<c>op_UnaryNegation</c>).
	/// </summary>
	UnaryNegation,

	/// <summary>
	///     The logical negation operator <c>!</c> (<c>op_LogicalNot</c>).
	/// </summary>
	LogicalNot,

	/// <summary>
	///     The ones-complement operator <c>~</c> (<c>op_OnesComplement</c>).
	/// </summary>
	OnesComplement,

	/// <summary>
	///     The increment operator <c>++</c> (<c>op_Increment</c>).
	/// </summary>
	Increment,

	/// <summary>
	///     The decrement operator <c>--</c> (<c>op_Decrement</c>).
	/// </summary>
	Decrement,

	/// <summary>
	///     The <c>true</c> operator (<c>op_True</c>).
	/// </summary>
	True,

	/// <summary>
	///     The <c>false</c> operator (<c>op_False</c>).
	/// </summary>
	False,

	/// <summary>
	///     The addition operator <c>+</c> (<c>op_Addition</c>).
	/// </summary>
	Addition,

	/// <summary>
	///     The subtraction operator <c>-</c> (<c>op_Subtraction</c>).
	/// </summary>
	Subtraction,

	/// <summary>
	///     The multiplication operator <c>*</c> (<c>op_Multiply</c>).
	/// </summary>
	Multiply,

	/// <summary>
	///     The division operator <c>/</c> (<c>op_Division</c>).
	/// </summary>
	Division,

	/// <summary>
	///     The modulus operator <c>%</c> (<c>op_Modulus</c>).
	/// </summary>
	Modulus,

	/// <summary>
	///     The bitwise-and operator <c>&amp;</c> (<c>op_BitwiseAnd</c>).
	/// </summary>
	BitwiseAnd,

	/// <summary>
	///     The bitwise-or operator <c>|</c> (<c>op_BitwiseOr</c>).
	/// </summary>
	BitwiseOr,

	/// <summary>
	///     The exclusive-or operator <c>^</c> (<c>op_ExclusiveOr</c>).
	/// </summary>
	ExclusiveOr,

	/// <summary>
	///     The left-shift operator <c>&lt;&lt;</c> (<c>op_LeftShift</c>).
	/// </summary>
	LeftShift,

	/// <summary>
	///     The right-shift operator <c>&gt;&gt;</c> (<c>op_RightShift</c>).
	/// </summary>
	RightShift,

	/// <summary>
	///     The unsigned right-shift operator <c>&gt;&gt;&gt;</c> (<c>op_UnsignedRightShift</c>).
	/// </summary>
	UnsignedRightShift,

	/// <summary>
	///     The equality operator <c>==</c> (<c>op_Equality</c>).
	/// </summary>
	Equality,

	/// <summary>
	///     The inequality operator <c>!=</c> (<c>op_Inequality</c>).
	/// </summary>
	Inequality,

	/// <summary>
	///     The less-than operator <c>&lt;</c> (<c>op_LessThan</c>).
	/// </summary>
	LessThan,

	/// <summary>
	///     The greater-than operator <c>&gt;</c> (<c>op_GreaterThan</c>).
	/// </summary>
	GreaterThan,

	/// <summary>
	///     The less-than-or-equal operator <c>&lt;=</c> (<c>op_LessThanOrEqual</c>).
	/// </summary>
	LessThanOrEqual,

	/// <summary>
	///     The greater-than-or-equal operator <c>&gt;=</c> (<c>op_GreaterThanOrEqual</c>).
	/// </summary>
	GreaterThanOrEqual,

	/// <summary>
	///     The checked addition operator <c>+</c> (<c>op_CheckedAddition</c>).
	/// </summary>
	CheckedAddition,

	/// <summary>
	///     The checked subtraction operator <c>-</c> (<c>op_CheckedSubtraction</c>).
	/// </summary>
	CheckedSubtraction,

	/// <summary>
	///     The checked multiplication operator <c>*</c> (<c>op_CheckedMultiply</c>).
	/// </summary>
	CheckedMultiply,

	/// <summary>
	///     The checked division operator <c>/</c> (<c>op_CheckedDivision</c>).
	/// </summary>
	CheckedDivision,

	/// <summary>
	///     The checked increment operator <c>++</c> (<c>op_CheckedIncrement</c>).
	/// </summary>
	CheckedIncrement,

	/// <summary>
	///     The checked decrement operator <c>--</c> (<c>op_CheckedDecrement</c>).
	/// </summary>
	CheckedDecrement,

	/// <summary>
	///     The checked unary negation operator <c>-</c> (<c>op_CheckedUnaryNegation</c>).
	/// </summary>
	CheckedUnaryNegation,
}
