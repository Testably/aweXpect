using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches less than <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier LessThan(int maximum,
		ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
		=> new LessThanQuantifier(maximum);

	private sealed class LessThanQuantifier(int maximum) : EnumerableQuantifier
	{
		public override string ToString()
			=> maximum switch
			{
				1 => "less than one",
				_ => $"less than {maximum}",
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
		public override void AppendResult(StringBuilder stringBuilder,
			ExpectationGrammars grammars,
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			if (totalCount.HasValue)
			{
				if (verb != null)
				{
					stringBuilder.Append(matchingCount).Append(" of ").Append(totalCount.Value)
						.Append(' ').Append(verb);
				}
				else
				{
					stringBuilder.Append("found ").Append(matchingCount);
				}
			}
			else
			{
				if (verb != null)
				{
					stringBuilder.Append("at least ").Append(matchingCount)
						.Append(' ').Append(verb);
				}
				else
				{
					stringBuilder.Append("found at least ").Append(matchingCount);
				}
			}
		}
	}
}
