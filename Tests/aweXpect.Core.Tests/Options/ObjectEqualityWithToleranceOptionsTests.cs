using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ObjectEqualityWithToleranceOptionsTests
{
	[Fact]
	public async Task ForEvaluation_ShouldReadTheDefaultToleranceOnce()
	{
		int reads = 0;
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => ++reads);

		IOptionsEquality<int> evaluation = sut.ForEvaluation();
		bool first = await evaluation.AreConsideredEqual(1, 2);
		bool second = await evaluation.AreConsideredEqual(1, 3);

		await That(first).IsTrue();
		await That(second).IsFalse()
			.Because("the tolerance of 1 read for the evaluation must not be read again for the second comparison");
		await That(reads).IsEqualTo(1);
	}

	[Fact]
	public async Task ForEvaluation_WithExplicitTolerance_ShouldReturnTheSameOptions()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => 5);
		sut.Within(1);

		IOptionsEquality<int> evaluation = sut.ForEvaluation();

		await That(evaluation).IsSameAs(sut)
			.Because("an explicit tolerance does not depend on a setting");
	}

	[Fact]
	public async Task WhenTimeSpanToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> sut =
			new((a, e, t) => a - e <= t);

		void Act() => sut.Within(TimeSpan.FromSeconds(-1));

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("Tolerance must be non-negative").AsPrefix();
	}

	[Fact]
	public async Task WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("Tolerance must not be NaN").AsPrefix()
			.Because("NaN is neither negative nor a usable tolerance");
	}

	[Fact]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(-1.0);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("Tolerance must be non-negative").AsPrefix();
	}

	[Fact]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(0.0);

		await That(Act).DoesNotThrow();
	}
}
