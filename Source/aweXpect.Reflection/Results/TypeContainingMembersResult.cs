using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an assertion that a <see cref="System.Type" /> (or a collection of types) contains matching
///     members, allowing to specify a quantifier for the number of matching members.
/// </summary>
public class TypeContainingMembersResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	Quantifier quantifier)
	: AndOrResult<TType, IThat<TThat>>(expectationBuilder, subject)
{
	/// <summary>
	///     Verifies, that the matching members occur at least…
	/// </summary>
	public CountTimesResult<TypeContainingMembersResult<TType, TThat>> AtLeast()
		=> new(value =>
		{
			quantifier.AtLeast(value);
			return this;
		});

	/// <summary>
	///     Verifies, that the matching members occur at least <paramref name="minimum" /> times.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> AtLeast(Times minimum)
	{
		quantifier.AtLeast(minimum.Value);
		return this;
	}

	/// <summary>
	///     Verifies, that the matching members occur at most…
	/// </summary>
	public CountTimesResult<TypeContainingMembersResult<TType, TThat>> AtMost()
		=> new(value =>
		{
			quantifier.AtMost(value);
			return this;
		});

	/// <summary>
	///     Verifies, that the matching members occur at most <paramref name="maximum" /> times.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> AtMost(Times maximum)
	{
		quantifier.AtMost(maximum.Value);
		return this;
	}

	/// <summary>
	///     Verifies, that the matching members occur between <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<TypeContainingMembersResult<TType, TThat>> Between(int minimum)
		=> new(maximum =>
		{
			quantifier.Between(minimum, maximum);
			return this;
		});

	/// <summary>
	///     Verifies, that the matching members occur exactly <paramref name="expected" /> times.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> Exactly(Times expected)
	{
		quantifier.Exactly(expected.Value);
		return this;
	}

	/// <summary>
	///     Verifies, that the matching members occur less than…
	/// </summary>
	public CountTimesResult<TypeContainingMembersResult<TType, TThat>> LessThan()
		=> new(value =>
		{
			quantifier.LessThan(value);
			return this;
		});

	/// <summary>
	///     Verifies, that the matching members occur less than <paramref name="maximum" /> times.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> LessThan(Times maximum)
	{
		quantifier.LessThan(maximum.Value);
		return this;
	}

	/// <summary>
	///     Verifies, that the matching members occur more than…
	/// </summary>
	public CountTimesResult<TypeContainingMembersResult<TType, TThat>> MoreThan()
		=> new(value =>
		{
			quantifier.MoreThan(value);
			return this;
		});

	/// <summary>
	///     Verifies, that the matching members occur more than <paramref name="minimum" /> times.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> MoreThan(Times minimum)
	{
		quantifier.MoreThan(minimum.Value);
		return this;
	}

	/// <summary>
	///     Verifies, that no member matches the filter.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> Never()
	{
		quantifier.Exactly(0);
		return this;
	}

	/// <summary>
	///     Verifies, that exactly one member matches the filter.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> Once()
	{
		quantifier.Exactly(1);
		return this;
	}

	/// <summary>
	///     Verifies, that exactly two members match the filter.
	/// </summary>
	public TypeContainingMembersResult<TType, TThat> Twice()
	{
		quantifier.Exactly(2);
		return this;
	}
}
