using System.Collections.Generic;
using System.Threading;
using aweXpect.Options;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Options;

public class ObjectEqualityWithToleranceOptionsTests
{
	[Test]
	public async Task AreConsideredEqual_WhenBothAreNull_ShouldReturnTrue()
	{
		ObjectEqualityWithToleranceOptions<int?, int> sut =
			new((a, e, t) => Math.Abs(a!.Value - e!.Value) <= t);
		sut.Within(1);

		bool result = await sut.AreConsideredEqual(null, (int?)null);

		await That(result).IsTrue();
	}

	[Test]
	public async Task AreConsideredEqual_WhenExpectedHasAnotherType_ShouldReturnFalse()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(1);

		bool result = await sut.AreConsideredEqual(1, "1");

		await That(result).IsFalse();
	}

	[Test]
	[Arguments(3, true)]
	[Arguments(4, false)]
	public async Task AreConsideredEqualWithExplanation_ShouldDecideWithTheTolerance(int expected, bool expectMatch)
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(2);

		IObjectMatchResult result = await sut.AreConsideredEqualWithExplanation(1, expected);

		await That(result.IsMatch).IsEqualTo(expectMatch);
		await That(result.GetExtendedFailure("it", ExpectationGrammars.None, 1, expected)).IsEqualTo("it was 1");
	}

	[Test]
	public async Task ForEvaluation_ShouldReadTheDefaultToleranceOnce()
	{
		int reads = 0;
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => ++reads);

		ObjectEqualityOptions<int> evaluation = sut.ForEvaluation(new Context(), CancellationToken.None);
		bool first = await evaluation.AreConsideredEqual(1, 2);
		bool second = await evaluation.AreConsideredEqual(1, 3);

		await That(first).IsTrue();
		await That(second).IsFalse()
			.Because("the tolerance of 1 read for the evaluation must not be read again for the second comparison");
		await That(reads).IsEqualTo(1);
	}

	[Test]
	public async Task ForEvaluation_WithExplicitTolerance_ShouldReturnTheSameOptions()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => 5);
		sut.Within(1);

		IOptionsEquality<int> evaluation = sut.ForEvaluation(new Context(), CancellationToken.None);

		await That(evaluation).IsSameAs(sut)
			.Because("an explicit tolerance does not depend on a setting");
	}

	[Test]
	[Arguments(false, "is equal to 3 ± 2")]
	[Arguments(true, "is not equal to 3 ± 2")]
	public async Task GetExpectation_ShouldNameTheTolerance(bool isNegated, string expected)
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(2);

		string result = sut.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task GetItemExpectation_ShouldNameTheTolerance()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(2);

		string result = sut.GetItemExpectation("3", "an item", "equal to");

		await That(result).IsEqualTo("an item equal to 3 ± 2");
	}

	[Test]
	[Arguments(0, "")]
	[Arguments(5, " ± 5")]
	public async Task ToString_WithDefaultTolerance_ShouldOnlyNameAToleranceOtherThanZero(int defaultTolerance,
		string expected)
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => defaultTolerance);

		string? result = sut.ToString();

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task UseRelativeTolerance_ShouldAppendTheUnitToTheTolerance()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		string result = sut.GetExpectation("100", ExpectationGrammars.None);

		await That(result).IsEqualTo("is equal to 100 ± 5%");
	}

	[Test]
	[Arguments(-105, -100, true)]
	[Arguments(105, 100, true)]
	[Arguments(106, 100, false)]
	public async Task UseRelativeTolerance_ShouldApplyTheToleranceRelativeToTheExpectedValue(int actual,
		int expected, bool expectMatch)
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * Math.Abs(e) / 100, "%");

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsEqualTo(expectMatch);
	}

	[Test]
	public async Task UseRelativeTolerance_WhenEvaluated_ShouldKeepTheRelativeTolerance()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		ObjectEqualityOptions<int> evaluation = sut.ForEvaluation(new Context(), CancellationToken.None);
		bool result = await evaluation.AreConsideredEqual(210, 200);

		await That(result).IsTrue();
	}

	[Test]
	public async Task UseRelativeTolerance_WhenSubjectIsNullable_ShouldApplyItToTheExpectedValue()
	{
		ObjectEqualityWithToleranceOptions<int?, int> sut =
			new((a, e, t) => a is not null && e is not null && Math.Abs(a.Value - e.Value) <= t);
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		bool result = await sut.AreConsideredEqual((int?)210, (int?)200);

		await That(result).IsTrue()
			.Because("the nullable expected value 200 makes 5% a tolerance of 10");
	}

	[Test]
	public async Task UseRelativeTolerance_WithoutWithin_ShouldNotChangeTheComparison()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		bool result = await sut.AreConsideredEqual(201, 200);
		string expectation = sut.GetExpectation("200", ExpectationGrammars.None);

		await That(result).IsFalse();
		await That(expectation).IsEqualTo("is equal to 200");
	}

	[Test]
	public async Task Using_WhenToleranceIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(0.1);

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with Within.")
			.Because("the comparer would silently replace the tolerance");
	}

	[Test]
	public async Task Using_WithDefaultTolerance_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => 5);

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).DoesNotThrow()
			.Because("a default tolerance is not an explicit option");
	}

	[Test]
	public async Task WhenTimeSpanToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> sut =
			new((a, e, t) => a - e <= t);

		void Act() => sut.Within(TimeSpan.FromSeconds(-1));

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix()
			.Because("NaN is neither negative nor a usable tolerance");
	}

	[Test]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(-1.0);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(0.0);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Within_ShouldReturnTheSameOptionsWithTheirToleranceMembers()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		ObjectEqualityWithToleranceOptions<int, int> result = sut.Within(5);
		result.UseRelativeTolerance((t, e) => t * e / 100, "%");

		await That(result).IsSameAs(sut);
		await That(sut.GetExpectation("100", ExpectationGrammars.None)).IsEqualTo("is equal to 100 ± 5%")
			.Because("the relative tolerance can be chained directly after Within");
	}

	[Test]
	public async Task Within_WhenComparerIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Using(new AllEqualComparer());

		void Act() => sut.Within(0.1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be combined with Using.")
			.Because("the tolerance would silently replace the comparer");
	}

	[Test]
	public async Task Within_WhenToleranceIsRejectedByTheValidation_ShouldThrowItsException()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithToleranceValidation(t =>
				{
					if (t % 2 != 0)
					{
						throw new ArgumentOutOfRangeException(nameof(t), "The tolerance must be even.");
					}
				});

		void Act() => sut.Within(3);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("t").And
			.WithMessage("The tolerance must be even.").AsPrefix();
	}

	[Test]
	public async Task Within_WhenToleranceIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);
		sut.Within(0.1);

		void Act() => sut.Within(0.2);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the second tolerance would silently replace the first one");
	}

	[Test]
	public async Task Within_WithDefaultTolerance_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => 5);

		void Act() => sut.Within(1);

		await That(Act).DoesNotThrow()
			.Because("a default tolerance is not an explicit option");
	}

	[Test]
	public async Task Within_WithToleranceValidation_WhenToleranceIsAccepted_ShouldApplyIt()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithToleranceValidation(_ => { });
		sut.Within(2);

		bool result = await sut.AreConsideredEqual(1, 3);

		await That(result).IsTrue();
	}

	[Test]
	public async Task Within_WithToleranceValidation_WhenToleranceIsSpecifiedTwice_ShouldReportTheRepetition()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithToleranceValidation(t =>
				{
					if (t % 2 != 0)
					{
						throw new ArgumentOutOfRangeException(nameof(t), "The tolerance must be even.");
					}
				});
		sut.Within(2);

		void Act() => sut.Within(3);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the repetition is the misuse, whatever the second tolerance is");
	}

	[Test]
	public async Task Within_WithToleranceValidation_WhenToleranceWasRejected_ShouldAcceptALaterTolerance()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithToleranceValidation(t =>
				{
					if (t % 2 != 0)
					{
						throw new ArgumentOutOfRangeException(nameof(t), "The tolerance must be even.");
					}
				});
		await That(() => sut.Within(3)).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must be even.").AsPrefix();

		void Act() => sut.Within(2);

		await That(Act).DoesNotThrow()
			.Because("the rejected tolerance must leave the options unchanged");
		await That(await sut.AreConsideredEqual(1, 3)).IsTrue();
	}

	private sealed class AllEqualComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => true;

		public int GetHashCode(object obj) => 0;
	}
}
