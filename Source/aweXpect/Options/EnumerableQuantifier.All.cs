using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches all items.
	/// </summary>
	public static EnumerableQuantifier All(ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
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
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			verb ??= "were";
			if (grammars.IsNegated())
			{
				stringBuilder.Append("all ").Append(matchingCount).Append(' ').Append(verb);
			}
			else if (totalCount.HasValue)
			{
				if (matchingCount == 0)
				{
					stringBuilder.Append("none");
				}
				else
				{
					stringBuilder.Append("only ").Append(matchingCount);
				}

				stringBuilder.Append(" of ").Append(totalCount.Value).Append(' ')
					.Append(verb);
			}
			else
			{
				stringBuilder.Append("not all ").Append(verb);
			}
		}
	}
}
