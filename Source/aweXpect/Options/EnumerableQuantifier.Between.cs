using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Options;

public abstract partial class EnumerableQuantifier
{
	/// <summary>
	///     Matches between <paramref name="minimum" /> and <paramref name="maximum" /> items.
	/// </summary>
	public static EnumerableQuantifier Between(int minimum, int maximum,
		ExpectationGrammars expectationGrammars = ExpectationGrammars.None)
		=> new BetweenQuantifier(minimum, maximum);

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
			int matchingCount,
			int notMatchingCount,
			int? totalCount,
			string? verb = null)
		{
			if (matchingCount > maximum)
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
			else if (totalCount.HasValue)
			{
				if (verb != null)
				{
					stringBuilder.Append("only ").Append(matchingCount).Append(" of ").Append(totalCount.Value)
						.Append(' ').Append(verb);
				}
				else
				{
					stringBuilder.Append("found only ").Append(matchingCount);
				}
			}
		}
	}
}
