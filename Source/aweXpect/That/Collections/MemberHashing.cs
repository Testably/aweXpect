using System;
using System.Collections.Generic;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Hash codes for the members that uniqueness expectations count, which agree with the comparison of their
///     options, so that only members with the same hash code have to be compared.
/// </summary>
/// <remarks>
///     A hash code is only offered when the comparison cannot consider two members equal that have different hash
///     codes, and when computing it calls no code of the caller, which could disagree with its equality or throw.
/// </remarks>
internal static class MemberHashing
{
	/// <summary>
	///     The hash code for strings that the <paramref name="options" /> compare with plain ordinal equality, or
	///     <see langword="null" /> when they were changed.
	/// </summary>
	public static Func<string?, int>? For(StringEqualityOptions options)
		=> options.ComparesByOrdinalEquality
			? static value => value is null ? 0 : StringComparer.Ordinal.GetHashCode(value)
			: null;

	/// <summary>
	///     The hash code for members of a framework type that the <paramref name="options" /> compare with their default
	///     equality, or <see langword="null" /> otherwise.
	/// </summary>
	public static Func<TMember, int>? For<TMember>(ItemEqualityOptions<TMember> options)
		=> options.HasDefaultMatchType && HasFrameworkHashCode(typeof(TMember))
			? static value => value is null ? 0 : EqualityComparer<TMember>.Default.GetHashCode(value)
			: null;

	/// <remarks>
	///     Floating-point numbers and decimals are left out, because values that are equal can have different hash codes
	///     on some target frameworks, e.g. <c>0.0</c> and <c>-0.0</c>.
	/// </remarks>
	private static bool HasFrameworkHashCode(Type type)
	{
		type = Nullable.GetUnderlyingType(type) ?? type;
		return type.IsEnum ||
		       type == typeof(string) ||
		       type == typeof(bool) ||
		       type == typeof(char) ||
		       type == typeof(byte) ||
		       type == typeof(sbyte) ||
		       type == typeof(short) ||
		       type == typeof(ushort) ||
		       type == typeof(int) ||
		       type == typeof(uint) ||
		       type == typeof(long) ||
		       type == typeof(ulong) ||
		       type == typeof(Guid) ||
		       type == typeof(DateTime) ||
		       type == typeof(DateTimeOffset) ||
		       type == typeof(TimeSpan);
	}
}
