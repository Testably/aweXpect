using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches fewer than <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier LessThan(int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		return new LessThanQuantifier(maximum);
	}

	private sealed class LessThanQuantifier(int maximum) : EnumerableQuantifier
	{
		public override string ToString()
			=> maximum switch
			{
				1 => "fewer than one",
				_ => $"fewer than {maximum}",
			};

		/// <inheritdoc />
		public override bool IsDeterminable(int matchingCount, int notMatchingCount)
			=> matchingCount >= maximum;

		/// <inheritdoc />
		public override bool IsSingle() => maximum == 1;

		/// <inheritdoc />
		public override Outcome GetOutcome(int matchingCount, int notMatchingCount, int? totalCount)
		{
			if (matchingCount >= maximum)
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
		internal override void AppendNegated(StringBuilder stringBuilder)
			=> stringBuilder.Append(maximum switch
			{
				1 => " for at least one item",
				_ => $" for at least {maximum} items",
			});

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
