using System.Collections.Generic;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ObjectEqualityWithToleranceOptionsTests
{
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

		IOptionsEquality<int> evaluation = sut.ForEvaluation();
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

		IOptionsEquality<int> evaluation = sut.ForEvaluation();

		await That(evaluation).IsSameAs(sut)
			.Because("an explicit tolerance does not depend on a setting");
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
