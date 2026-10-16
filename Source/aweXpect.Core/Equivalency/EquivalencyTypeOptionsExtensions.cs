using System;
using System.Runtime.CompilerServices;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;

namespace aweXpect.Equivalency;

/// <summary>
///     Extension methods for <see cref="EquivalencyTypeOptions" />.
/// </summary>
/// <remarks>
///     Each method has an overload for <see cref="EquivalencyOptions" /> and one for
///     <see cref="EquivalencyTypeOptions" /> instead of a type parameter for the options, so that the types of the
///     compared values can be specified without also specifying the type of the options.
/// </remarks>
public static class EquivalencyTypeOptionsExtensions
{
	/// <summary>
	///     Compares an actual <typeparamref name="TActual" /> with an expected <typeparamref name="TExpected" /> by the
	///     <paramref name="predicate" /> instead of by their members, wherever they meet in the compared objects.
	/// </summary>
	/// <remarks>
	///     The predicate decides before the values are compared by value or by members, also for derived or implementing
	///     types, and a <see langword="null" /> value is never passed to it. When several predicates apply to a pair, the
	///     one that was added last decides.
	/// </remarks>
	public static EquivalencyOptions Comparing<TActual, TExpected>(
		this EquivalencyOptions options,
		Func<TActual, TExpected, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> AddComparison(options, predicate, doNotPopulateThisValue);

	/// <inheritdoc cref="Comparing{TActual, TExpected}(EquivalencyOptions, Func{TActual, TExpected, bool}, string)" />
	public static EquivalencyTypeOptions Comparing<TActual, TExpected>(
		this EquivalencyTypeOptions options,
		Func<TActual, TExpected, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> AddComparison(options, predicate, doNotPopulateThisValue);

	private static TEquivalencyOptions AddComparison<TActual, TExpected, TEquivalencyOptions>(
		TEquivalencyOptions options,
		Func<TActual, TExpected, bool> predicate,
		string description)
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		predicate.ThrowIfNull();
		return options with
		{
			TypePairComparisons =
			[
				..options.TypePairComparisons,
				new TypePairComparison<TActual, TExpected>(predicate, description.TrimCommonWhiteSpace()),
			],
		};
	}
}
