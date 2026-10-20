namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Maps the <see cref="Operator" /> enum to the compiler-emitted <c>op_*</c> metadata name.
/// </summary>
internal static class OperatorNames
{
	/// <summary>
	///     Gets the compiler-emitted <c>op_*</c> metadata name for the <paramref name="operator" />
	///     (e.g. <see cref="Operator.Addition" /> maps to <c>op_Addition</c>).
	/// </summary>
	/// <remarks>
	///     Uses an explicit switch instead of <c>"op_" + @operator</c> so that the (reflection-based) enum
	///     <see cref="System.Enum.ToString()" /> is never invoked on the hot path.
	/// </remarks>
	public static string Of(Operator @operator)
		=> @operator switch
		{
			Operator.Implicit => "op_Implicit",
			Operator.Explicit => "op_Explicit",
			Operator.CheckedExplicit => "op_CheckedExplicit",
			Operator.UnaryPlus => "op_UnaryPlus",
			Operator.UnaryNegation => "op_UnaryNegation",
			Operator.LogicalNot => "op_LogicalNot",
			Operator.OnesComplement => "op_OnesComplement",
			Operator.Increment => "op_Increment",
			Operator.Decrement => "op_Decrement",
			Operator.True => "op_True",
			Operator.False => "op_False",
			Operator.Addition => "op_Addition",
			Operator.Subtraction => "op_Subtraction",
			Operator.Multiply => "op_Multiply",
			Operator.Division => "op_Division",
			Operator.Modulus => "op_Modulus",
			Operator.BitwiseAnd => "op_BitwiseAnd",
			Operator.BitwiseOr => "op_BitwiseOr",
			Operator.ExclusiveOr => "op_ExclusiveOr",
			Operator.LeftShift => "op_LeftShift",
			Operator.RightShift => "op_RightShift",
			Operator.UnsignedRightShift => "op_UnsignedRightShift",
			Operator.Equality => "op_Equality",
			Operator.Inequality => "op_Inequality",
			Operator.LessThan => "op_LessThan",
			Operator.GreaterThan => "op_GreaterThan",
			Operator.LessThanOrEqual => "op_LessThanOrEqual",
			Operator.GreaterThanOrEqual => "op_GreaterThanOrEqual",
			Operator.CheckedAddition => "op_CheckedAddition",
			Operator.CheckedSubtraction => "op_CheckedSubtraction",
			Operator.CheckedMultiply => "op_CheckedMultiply",
			Operator.CheckedDivision => "op_CheckedDivision",
			Operator.CheckedIncrement => "op_CheckedIncrement",
			Operator.CheckedDecrement => "op_CheckedDecrement",
			Operator.CheckedUnaryNegation => "op_CheckedUnaryNegation",
			_ => "op_" + @operator,
		};

	/// <summary>
	///     Gets a human-readable name for the <paramref name="operator" /> for use in assertion messages and
	///     descriptions (e.g. <see cref="Operator.Addition" /> is displayed as <c>Addition</c> rather than its
	///     <c>op_Addition</c> metadata name).
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="Of" />, this is only used while building messages (on assertion failure or when describing a
	///     filter), never per candidate member, so the (reflection-based) enum <see cref="System.Enum.ToString()" /> is
	///     acceptable here.
	/// </remarks>
	public static string Display(Operator @operator)
		=> @operator.ToString();

	/// <summary>
	///     Gets the compiler-emitted metadata name of an implicit (<c>op_Implicit</c>) or explicit
	///     (<c>op_Explicit</c>) conversion operator.
	/// </summary>
	public static string Conversion(bool isImplicit)
		=> isImplicit ? "op_Implicit" : "op_Explicit";
}
