using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for results that compare objects with <see cref="ObjectEqualityOptions{TSubject}" />.
/// </summary>
/// <remarks>
///     These options all specify how two objects are compared, so only one of them can be specified.
/// </remarks>
public static class ObjectEqualityOptionsExtensions
{
	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="object" />s.
	/// </summary>
	public static TSelf Using<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result,
		IEqualityComparer<object> comparer)
		where TSelf : IObjectEqualityResult<TSelf, TElement>
	{
		result.Options.Using(comparer);
		return (TSelf)result;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <typeparamref name="TElement" /> values.
	/// </summary>
	public static TSelf Using<TSelf, TElement>(this IObjectEqualityResult<TSelf, TElement> result,
		IEqualityComparer<TElement> comparer)
		where TSelf : IObjectEqualityResult<TSelf, TElement>
	{
		result.Options.Using(comparer);
		return (TSelf)result;
	}

	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the comparison.
	/// </summary>
	public static TSelf Within<TSelf, TElement, TTolerance>(
		this IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance> result,
		TTolerance tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>
	{
		((IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>)result).Options.Within(tolerance);
		return (TSelf)result;
	}

	/// <inheritdoc cref="Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	/// <remarks>
	///     An <see langword="int" /> literal, e.g. in <c>Within(1)</c>, cannot be inferred as a
	///     <see langword="byte" /> tolerance, so this overload fixes the type.
	/// </remarks>
	public static TSelf Within<TSelf, TElement>(this IObjectEqualityWithToleranceResult<TSelf, TElement, byte> result,
		byte tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, byte>
		=> result.Within<TSelf, TElement, byte>(tolerance);

	/// <inheritdoc cref="Within{TSelf, TElement}(IObjectEqualityWithToleranceResult{TSelf, TElement, byte}, byte)" />
	public static TSelf Within<TSelf, TElement>(this IObjectEqualityWithToleranceResult<TSelf, TElement, sbyte> result,
		sbyte tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, sbyte>
		=> result.Within<TSelf, TElement, sbyte>(tolerance);

	/// <inheritdoc cref="Within{TSelf, TElement}(IObjectEqualityWithToleranceResult{TSelf, TElement, byte}, byte)" />
	public static TSelf Within<TSelf, TElement>(this IObjectEqualityWithToleranceResult<TSelf, TElement, short> result,
		short tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, short>
		=> result.Within<TSelf, TElement, short>(tolerance);

	/// <inheritdoc cref="Within{TSelf, TElement}(IObjectEqualityWithToleranceResult{TSelf, TElement, byte}, byte)" />
	public static TSelf Within<TSelf, TElement>(this IObjectEqualityWithToleranceResult<TSelf, TElement, ushort> result,
		ushort tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, ushort>
		=> result.Within<TSelf, TElement, ushort>(tolerance);

	/// <inheritdoc cref="Within{TSelf, TElement}(IObjectEqualityWithToleranceResult{TSelf, TElement, byte}, byte)" />
	public static TSelf Within<TSelf, TElement>(this IObjectEqualityWithToleranceResult<TSelf, TElement, uint> result,
		uint tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, uint>
		=> result.Within<TSelf, TElement, uint>(tolerance);

	/// <inheritdoc cref="Within{TSelf, TElement}(IObjectEqualityWithToleranceResult{TSelf, TElement, byte}, byte)" />
	public static TSelf Within<TSelf, TElement>(this IObjectEqualityWithToleranceResult<TSelf, TElement, ulong> result,
		ulong tolerance)
		where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, ulong>
		=> result.Within<TSelf, TElement, ulong>(tolerance);
}
