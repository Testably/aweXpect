using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches all items.
	/// </summary>
	public static EnumerableQuantifier All()
		=> new AllQuantifier();

	private sealed class AllQuantifier : EnumerableQuantifier
	{
		public override string ToString() => "all";

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> notMatchingCount > 0;

		/// <inheritdoc />
		public override bool IsSingle() => false;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (notMatchingCount > 0)
			{
				return Outcome.Failure;
			}

			if (matchingCount == totalCount)
			{
				return Outcome.Success;
			}

			return Outcome.Undecided;
		}

		/// <inheritdoc />
		public override QuantifierContexts GetQuantifierContext()
			=> QuantifierContexts.NotMatchingItems;

		/// <inheritdoc />
		/// <remarks>
		///     <c>not for all items</c> keeps the negation in front of the connector, because <c>for not all items</c>
		///     reads as a statement about the items rather than about the quantifier.
		/// </remarks>
		internal override void AppendNegated(StringBuilder stringBuilder)
			=> stringBuilder.Append(" not for all items");

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			string it,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			verb ??= "were";
			if (grammars.IsNegated())
			{
				if (totalCount == 0)
				{
					stringBuilder.Append(it).Append(grammars.SubjectVerb(it, " was", " were")).Append(" empty");
				}
				else
				{
					stringBuilder.Append("all ").Append(matchingCount).Append(' ').Append(verb);
				}
			}
			else
			{
				AppendCounts(stringBuilder, it, matchingCount, notMatchingCount, totalCount, verb, true);
			}
		}
	}
}
