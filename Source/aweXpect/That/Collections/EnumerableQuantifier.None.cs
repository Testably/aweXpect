using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect;

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
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			if (verb != null)
			{
				if (totalCount.HasValue)
				{
					stringBuilder.Append(matchingCount).Append(" of ").Append(totalCount.Value)
						.Append(' ').Append(verb);
				}
				else
				{
					stringBuilder.Append("at least one ").Append(verb == "were" ? "was" : verb);
				}
			}
			else
			{
				stringBuilder.Append("found ").Append(matchingCount);
			}
		}
	}
}
