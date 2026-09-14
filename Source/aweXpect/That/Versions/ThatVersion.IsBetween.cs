using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the subject is between the <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<AndOrResult<Version?, IThat<Version?>>, Version?> IsBetween(
		this IThat<Version?> source,
		Version? minimum)
		=> new(maximum
			=> new AndOrResult<Version?, IThat<Version?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum)),
				source));

	/// <summary>
	///     Verifies that the subject is not between the <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<AndOrResult<Version?, IThat<Version?>>, Version?> IsNotBetween(
		this IThat<Version?> source,
		Version? minimum)
		=> new(maximum
			=> new AndOrResult<Version?, IThat<Version?>>(
				source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
					new IsBetweenConstraint(it, grammars, minimum, maximum).Invert()),
				source));

	private sealed class IsBetweenConstraint(
		string it,
		ExpectationGrammars grammars,
		Version? minimum,
		Version? maximum)
		: ConstraintResult.WithNotNullValue<Version?>(it, grammars),
			IValueConstraint<Version?>
	{
		public ConstraintResult IsMetBy(Version? actual)
		{
			Actual = actual;
			if (minimum is null || maximum is null)
			{
				Outcome = IsNegated ? Outcome.Success : Outcome.Failure;
				return this;
			}

			Outcome = actual?.CompareTo(minimum) >= 0 && actual?.CompareTo(maximum) <= 0
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is between ");
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not between ");
			Formatter.Format(stringBuilder, minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, maximum);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
