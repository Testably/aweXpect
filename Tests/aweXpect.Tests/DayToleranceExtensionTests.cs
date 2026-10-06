#if NET8_0_OR_GREATER
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed class DayToleranceExtensionTests
{
	[Test]
	public async Task WhenDefaultToleranceIsCustomized_ShouldApplyItLikeTheBuiltInExpectation()
	{
		DateOnly subject = new(2020, 1, 10);
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		async Task Act() => await That(subject).IsOnSameDayAs(subject.AddDays(1));

		await That(Act).DoesNotThrow()
			.Because("an extension using GetToleranceOrDefault applies the whole days of the default tolerance");
	}

	[Test]
	public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowLikeTheBuiltInExpectation()
	{
		DateOnly subject = new(2020, 1, 10);

		object ActOnExtension() => That(subject).IsOnSameDayAs(subject).Within(12.Hours());
		object ActOnBuiltIn() => That(subject).IsEqualTo(subject).Within(12.Hours());

		await That(ActOnExtension).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
			.Because("an extension using DayTolerance rejects a sub-day tolerance as soon as it is specified");
		await That(Catch.Exception(ActOnExtension)?.Message).IsEqualTo(Catch.Exception(ActOnBuiltIn)?.Message)
			.Because("the extension must reject the tolerance exactly like the built-in expectations");
	}

	[Test]
	public async Task Within_WhenToleranceIsWholeDays_ShouldApplyIt()
	{
		DateOnly subject = new(2020, 1, 10);

		async Task Act() => await That(subject).IsOnSameDayAs(subject.AddDays(1)).Within(1.Days());

		await That(Act).DoesNotThrow();
	}
}

file static class DayToleranceExtensions
{
	public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsOnSameDayAs(
		this IThat<DateOnly> subject,
		DateOnly expected)
	{
		TimeTolerance tolerance = new DayTolerance();
		return new TimeToleranceResult<DateOnly, IThat<DateOnly>>(
			((IExpectThat<DateOnly>)subject).ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsOnSameDayAsConstraint(it, grammars, expected, tolerance)),
			subject,
			tolerance);
	}

	private sealed class IsOnSameDayAsConstraint(
		string it,
		ExpectationGrammars grammars,
		DateOnly expected,
		TimeTolerance tolerance)
		: ConstraintResult.WithValue<DateOnly>(it, grammars),
			IValueConstraint<DateOnly>
	{
		public ConstraintResult IsMetBy(DateOnly actual)
		{
			Actual = actual;
			int days = (int)tolerance.GetToleranceOrDefault().TotalDays;
			Outcome = Math.Abs(actual.DayNumber - expected.DayNumber) <= days ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is on the same day as ").Append(expected).Append(tolerance.ToDayString());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was ").Append(Actual);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not on the same day as ").Append(expected).Append(tolerance.ToDayString());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
#endif
