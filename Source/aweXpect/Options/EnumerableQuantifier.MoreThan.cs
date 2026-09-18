using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches more than <paramref name="minimum" /> items.
	/// </summary>
	public static EnumerableQuantifier MoreThan(int minimum,
		ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
		=> new MoreThanQuantifier(minimum);

	private sealed class MoreThanQuantifier(int minimum) : EnumerableQuantifier
	{
		public override string ToString()
			=> minimum switch
			{
				1 => "more than one",
				_ => $"more than {minimum}",
			};

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount > minimum;

		/// <inheritdoc />
		public override bool IsSingle() => minimum == 1;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount > minimum)
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
		public override QuantifierContexts GetQuantifierContext()
			=> QuantifierContexts.NotMatchingItems;

		/// <inheritdoc />
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
			=> AppendCounts(stringBuilder, matchingCount, notMatchingCount, totalCount, verb,
				!grammars.IsNegated());
	}
}
