using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches between <paramref name="minimum" /> and <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier Between(int minimum, int maximum,
		ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<int>(minimum, maximum);
		return new BetweenQuantifier(minimum, maximum);
	}

	private sealed class BetweenQuantifier(int minimum, int maximum)
		: EnumerableQuantifier
	{
		public override string ToString() => $"between {minimum} and {maximum}";

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount > maximum;

		/// <inheritdoc />
		public override bool IsSingle() => false;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount > maximum)
			{
				return Outcome.Failure;
			}

			if (matchingCount >= minimum)
			{
				return Outcome.Success;
			}

			if (totalCount.HasValue)
			{
				return Outcome.Failure;
			}

			return Outcome.Undecided;
		}

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			string it,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
			=> AppendCounts(stringBuilder, it, matchingCount, notMatchingCount, totalCount, verb,
				matchingCount <= maximum && !grammars.IsNegated());
	}
}
