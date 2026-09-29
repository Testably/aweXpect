using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Quantifier for evaluating collections.
/// </summary>
public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Specifies which context is helpful for the <see cref="EnumerableQuantifier" />.
	/// </summary>
	[Flags]
	public enum QuantifierContexts
	{
		/// <summary>
		///     Include the matching items in the context.
		/// </summary>
		MatchingItems = 1 << 1,

		/// <summary>
		///     Include the not matching items in the context.
		/// </summary>
		NotMatchingItems = 1 << 2,

		/// <summary>
		///     Include nothing in the context.
		/// </summary>
		None = 0,
	}

	/// <remarks>
	///     Only the built-in quantifiers can derive, because the negated texts rely on members that are not public.
	/// </remarks>
	private protected EnumerableQuantifier()
	{
	}

	/// <summary>
	///     Checks for each iteration if the result is determinable by the <paramref name="matchingCount" /> and
	///     <paramref name="notMatchingCount" />.
	/// </summary>
	public abstract bool IsDeterminable(int matchingCount, int notMatchingCount);

	/// <summary>
	///     Returns <see langword="true" /> if the quantifier should be treated as containing a single item.
	/// </summary>
	/// <remarks>
	///     This means that the expectation text can be written in singular.
	/// </remarks>
	public abstract bool IsSingle();

	/// <summary>
	///     Returns the outcome.
	/// </summary>
	public abstract Outcome GetOutcome(
		int matchingCount,
		int notMatchingCount,
		int? totalCount);

	/// <summary>
	///     Returns the <see cref="QuantifierContexts" /> which specifies which context values are helpful.
	/// </summary>
	public virtual QuantifierContexts GetQuantifierContext()
		=> QuantifierContexts.None;

	/// <summary>
	///     Returns the <see cref="QuantifierContexts" /> which are helpful when the expectation is negated.
	/// </summary>
	/// <remarks>
	///     The negated expectation reads as its complement, so it shows the same items as the complement would. Without a
	///     complement, the negated expectation fails when the items are on the other side, so these items explain the
	///     failure.
	/// </remarks>
	internal virtual QuantifierContexts GetNegatedQuantifierContext()
	{
		if (GetComplement(ExpectationGrammars.None) is { } complement)
		{
			return complement.GetQuantifierContext();
		}

		QuantifierContexts contexts = GetQuantifierContext();
		QuantifierContexts negatedContexts = QuantifierContexts.None;
		if (contexts.HasFlag(QuantifierContexts.MatchingItems))
		{
			negatedContexts |= QuantifierContexts.NotMatchingItems;
		}

		if (contexts.HasFlag(QuantifierContexts.NotMatchingItems))
		{
			negatedContexts |= QuantifierContexts.MatchingItems;
		}

		return negatedContexts;
	}

	/// <summary>
	///     Appends the result text to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendResult(StringBuilder stringBuilder,
		ExpectationGrammars grammars,
		string it,
		int matchingCount,
		int notMatchingCount,
		int? totalCount,
		string? verb = null);

	/// <summary>
	///     Appends the connector together with the complement of the quantifier and the item noun,
	///     e.g. <c> for no items</c>.
	/// </summary>
	/// <remarks>
	///     The connector belongs to the negation, because the complement of <c>for all items</c> negates the connector
	///     itself (<c>not for all items</c>).
	/// </remarks>
	internal virtual void AppendNegated(StringBuilder stringBuilder)
	{
		EnumerableQuantifier? complement = GetComplement(ExpectationGrammars.None);
		if (complement is null)
		{
			stringBuilder.Append(" for not ").Append(this).Append(' ').Append(this.GetItemString());
		}
		else
		{
			stringBuilder.Append(" for ").Append(complement).Append(' ').Append(complement.GetItemString());
		}
	}

	/// <summary>
	///     Appends the complement of the quantifier in a nested expectation, e.g. <c>fewer than 2</c> in
	///     <c>has lines of which fewer than 2 are …</c>.
	/// </summary>
	internal void AppendNestedNegated(StringBuilder stringBuilder)
	{
		EnumerableQuantifier? complement = GetComplement(ExpectationGrammars.Nested);
		if (complement is null)
		{
			stringBuilder.Append("not ").Append(this);
		}
		else
		{
			stringBuilder.Append(complement);
		}
	}

	/// <summary>
	///     Returns <see langword="true" /> if the quantifier, or its complement when <paramref name="isNegated" />,
	///     names a single item (e.g. <c>at least one</c>).
	/// </summary>
	internal bool IsRenderedSingle(bool isNegated)
		=> isNegated
			? (GetComplement(ExpectationGrammars.Nested) ?? this).IsSingle()
			: IsSingle();

	/// <summary>
	///     Returns the quantifier that matches exactly when this one does not, or <see langword="null" /> when the
	///     negation is written as <c>not</c> in front of this quantifier.
	/// </summary>
	/// <remarks>
	///     A named complement avoids texts like <c>not no items</c> or <c>not at least 2</c>. The
	///     <paramref name="grammars" /> select its wording, e.g. <c>none</c> instead of <c>no</c> in a nested expectation.
	/// </remarks>
	private protected virtual EnumerableQuantifier? GetComplement(ExpectationGrammars grammars)
		=> null;

	/// <summary>
	///     Appends the <paramref name="matchingCount" /> relative to the <paramref name="totalCount" />,
	///     e.g. <c>only 2 of 3 were</c>, or <c>at least 4 of at least 7 were</c> when the enumeration stopped early.
	/// </summary>
	/// <remarks>
	///     Without a <paramref name="verb" />, the items themselves are counted (e.g. <c>HasCount</c>), so the matching count
	///     is the total and the result names <paramref name="it" /> as the subject that had them.
	/// </remarks>
	private protected static void AppendCounts(StringBuilder stringBuilder,
		string it,
		int matchingCount,
		int notMatchingCount,
		int? totalCount,
		string? verb,
		bool isTooFew)
	{
		if (verb is null)
		{
			stringBuilder.Append(it).Append(" had ");
			if (!totalCount.HasValue)
			{
				stringBuilder.Append("at least ");
			}
			else if (isTooFew)
			{
				stringBuilder.Append("only ");
			}

			stringBuilder.AppendItemCount(matchingCount);
			return;
		}

		if (matchingCount == 0)
		{
			stringBuilder.Append("none");
		}
		else if (isTooFew)
		{
			stringBuilder.Append("only ").Append(matchingCount);
		}
		else if (!totalCount.HasValue)
		{
			stringBuilder.Append("at least ").Append(matchingCount);
		}
		else
		{
			stringBuilder.Append(matchingCount);
		}

		stringBuilder.Append(" of ");
		if (totalCount.HasValue)
		{
			stringBuilder.Append(totalCount.Value);
		}
		else
		{
			stringBuilder.Append("at least ").Append(matchingCount + notMatchingCount);
		}

		stringBuilder.Append(' ').Append(verb);
	}
}
