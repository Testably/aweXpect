using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	private static readonly EnumerableQuantifier AllInstance = new AllQuantifier();

	/// <summary>
	///     Matches all items.
	/// </summary>
	public static EnumerableQuantifier All()
		=> AllInstance;

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
		///     The negated expectation only fails when every item matched, which the collection context already lists.
		/// </remarks>
		internal override QuantifierContexts GetNegatedQuantifierContext()
			=> QuantifierContexts.None;

		/// <inheritdoc />
		/// <remarks>
		///     <c>not for all items</c> keeps the negation in front of the connector, because <c>for not all items</c>
		///     reads as a statement about the items rather than about the quantifier.
		/// </remarks>
		private protected override void AppendNegated(StringBuilder stringBuilder)
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
