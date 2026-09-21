using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches none items.
	/// </summary>
	public static EnumerableQuantifier None(ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
		=> new NoneQuantifier(expectationGrammars);

	private sealed class NoneQuantifier(ExpectationGrammars expectationGrammars) : EnumerableQuantifier
	{
		public override string ToString()
			=> expectationGrammars.IsNested() ? "none" : "no";

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount > 0;

		/// <inheritdoc />
		public override bool IsSingle() => false;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount > 0)
			{
				return Outcome.Failure;
			}

			if (totalCount.HasValue)
			{
				return Outcome.Success;
			}

			return Outcome.Undecided;
		}

		/// <inheritdoc />
		public override QuantifierContexts GetQuantifierContext()
			=> QuantifierContexts.MatchingItems;

		/// <inheritdoc />
		/// <remarks>
		///     <c>not no items</c> is not grammatical, so this renders the exact complement.
		/// </remarks>
		internal override void AppendNegated(StringBuilder stringBuilder)
			=> stringBuilder.Append(" for at least one item");

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			string it,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
			=> AppendCounts(stringBuilder, it, matchingCount, notMatchingCount, totalCount, verb, false);
	}
}
