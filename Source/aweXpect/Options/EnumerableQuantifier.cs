using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;

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

	/// <summary>
	///     Checks for each iteration, if the result is determinable by the <paramref name="matchingCount" /> and
	///     <paramref name="notMatchingCount" />.
	/// </summary>
	public abstract bool IsDeterminable(int matchingCount, int notMatchingCount);

	/// <summary>
	///     Returns true, if the quantifier should be treated as containing a single item.
	/// </summary>
	/// <remarks>
	///     This means, that the expectation text can be written in singular.
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
	///     Appends the result text to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendResult(StringBuilder stringBuilder,
		ExpectationGrammars grammars,
		int matchingCount,
		int notMatchingCount,
		int? totalCount,
		string? verb = null);

	/// <summary>
	///     Appends the negated quantifier together with the item noun, e.g. <c>not all items</c>.
	/// </summary>
	internal virtual void AppendNegated(StringBuilder stringBuilder)
		=> stringBuilder.Append("not ").Append(this).Append(' ').Append(this.GetItemString());

	/// <summary>
	///     Appends the <paramref name="matchingCount" /> relative to the <paramref name="totalCount" />,
	///     e.g. <c>only 2 of 3 were</c>, or <c>at least 4 of at least 7 were</c> when the enumeration stopped early.
	/// </summary>
	/// <remarks>
	///     Without a <paramref name="verb" />, the items themselves are counted (e.g. <c>HasCount</c>), so the matching count
	///     is the total and only the number of found items is appended.
	/// </remarks>
	private protected static void AppendCounts(StringBuilder stringBuilder,
		int matchingCount,
		int notMatchingCount,
		int? totalCount,
		string? verb,
		bool isTooFew)
	{
		if (verb is null)
		{
			stringBuilder.Append("found ");
			if (!totalCount.HasValue)
			{
				stringBuilder.Append("at least ");
			}
			else if (isTooFew)
			{
				stringBuilder.Append("only ");
			}

			stringBuilder.Append(matchingCount);
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
