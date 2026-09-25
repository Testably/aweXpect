using System.Collections.Generic;
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

	[Fact]
	public async Task Using_WithDefaultTolerance_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => 5);

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).DoesNotThrow()
			.Because("a default tolerance is not an explicit option");
	}

	[Fact]
	public async Task WhenTimeSpanToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<DateTime, TimeSpan> sut =
			new((a, e, t) => a - e <= t);

		void Act() => sut.Within(TimeSpan.FromSeconds(-1));

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Fact]
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

	[Fact]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(-1.0);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Fact]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<double, double> sut =
			new((a, e, t) => Math.Abs(a - e) <= t);

		void Act() => sut.Within(0.0);

		await That(Act).DoesNotThrow();
	}

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task Within_WithDefaultTolerance_ShouldNotThrow()
	{
		ObjectEqualityWithToleranceOptions<int, int> sut =
			new ObjectEqualityWithToleranceOptions<int, int>((a, e, t) => Math.Abs(a - e) <= t)
				.WithDefaultTolerance(() => 5);

		void Act() => sut.Within(1);

		await That(Act).DoesNotThrow()
			.Because("a default tolerance is not an explicit option");
	}

	private sealed class AllEqualComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => true;

		public int GetHashCode(object obj) => 0;
	}
}
