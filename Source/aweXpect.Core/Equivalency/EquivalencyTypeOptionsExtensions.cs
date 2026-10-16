using System;
using System.Linq.Expressions;
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

	/// <summary>
	///     Compares the <paramref name="expectedMember" /> of an expected <typeparamref name="TExpected" /> with the
	///     differently named <paramref name="actualMember" /> of an actual <typeparamref name="TActual" />, wherever they
	///     meet in the compared objects.
	/// </summary>
	/// <remarks>
	///     Either member can be nested, e.g. <c>x => x.Artist.Name</c>. A <see langword="null" /> value on the way to the
	///     actual member reads as <see langword="null" />. When the actual object has no member for an expected member
	///     that a nested <paramref name="expectedMember" /> runs through, every member below it has to be mapped as well,
	///     and the others are reported as missing. The expressions are only read for the names of their members and
	///     never compiled.<br />
	///     Members to ignore and failures name the expected member. When several mappings apply to an expected member,
	///     the one that was added last decides.
	/// </remarks>
	/// <exception cref="ArgumentException">An expression does not select a member of its parameter.</exception>
	public static EquivalencyOptions Mapping<TActual, TExpected>(
		this EquivalencyOptions options,
		Expression<Func<TActual, object?>> actualMember,
		Expression<Func<TExpected, object?>> expectedMember)
		=> AddMapping(options, actualMember, expectedMember);

	/// <inheritdoc
	///     cref="Mapping{TActual, TExpected}(EquivalencyOptions, Expression{Func{TActual, object}}, Expression{Func{TExpected, object}})" />
	public static EquivalencyTypeOptions Mapping<TActual, TExpected>(
		this EquivalencyTypeOptions options,
		Expression<Func<TActual, object?>> actualMember,
		Expression<Func<TExpected, object?>> expectedMember)
		=> AddMapping(options, actualMember, expectedMember);

	/// <summary>
	///     Compares the <paramref name="expectedMember" /> with the differently named <paramref name="actualMember" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectedMember" /> matches like a member to ignore by name: case-insensitively and by whole
	///     segments at the end of the member path, so <c>"Key"</c> maps every member called <c>Key</c> at any depth. The
	///     <paramref name="actualMember" /> is read on the actual object that is compared with the object which has the
	///     first member of the <paramref name="expectedMember" />. Either member can be a dot-separated path, e.g.
	///     <c>"Artist.Name"</c>, which behaves like a nested member of the expression overload.
	/// </remarks>
	/// <exception cref="ArgumentException">A member is empty or contains an empty or indexed segment.</exception>
	public static EquivalencyOptions Mapping(
		this EquivalencyOptions options,
		string actualMember,
		string expectedMember)
		=> AddMapping(options, actualMember, expectedMember);

	/// <inheritdoc cref="Mapping(EquivalencyOptions, string, string)" />
	public static EquivalencyTypeOptions Mapping(
		this EquivalencyTypeOptions options,
		string actualMember,
		string expectedMember)
		=> AddMapping(options, actualMember, expectedMember);

	private static TEquivalencyOptions AddMapping<TActual, TExpected, TEquivalencyOptions>(
		TEquivalencyOptions options,
		Expression<Func<TActual, object?>> actualMember,
		Expression<Func<TExpected, object?>> expectedMember)
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		actualMember.ThrowIfNull();
		expectedMember.ThrowIfNull();
		return options with
		{
			MemberMappings =
			[
				..options.MemberMappings,
				new MemberMapping.ByType<TActual, TExpected>(actualMember, expectedMember),
			],
		};
	}

	private static TEquivalencyOptions AddMapping<TEquivalencyOptions>(
		TEquivalencyOptions options,
		string actualMember,
		string expectedMember)
		where TEquivalencyOptions : EquivalencyTypeOptions
	{
		actualMember.ThrowIfNull();
		expectedMember.ThrowIfNull();
		return options with
		{
			MemberMappings =
			[
				..options.MemberMappings,
				new MemberMapping.ByName(actualMember, expectedMember),
			],
		};
	}
}
