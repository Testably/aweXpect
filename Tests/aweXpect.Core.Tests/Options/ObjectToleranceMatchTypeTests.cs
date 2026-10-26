using System.Collections.Generic;
using System.Threading;
using aweXpect.Options;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Options;

public class ObjectToleranceMatchTypeTests
{
	[Test]
	public async Task AreConsideredEqual_WhenBothAreNull_ShouldReturnTrue()
	{
		OptionsHolder<ObjectMatchType<int?>> sut =
			new(ObjectMatchType<int?>.WithTolerance<int>((a, e, t) => Math.Abs(a!.Value - e!.Value) <= t));
		sut.Within(1);

		bool result = await Evaluator(sut).AreConsideredEqual(null, (int?)null);

		await That(result).IsTrue();
	}

	[Test]
	public async Task AreConsideredEqual_WhenExpectedHasAnotherType_ShouldReturnFalse()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(1);

		bool result = await Evaluator(sut).AreConsideredEqual(1, "1");

		await That(result).IsFalse();
	}

	[Test]
	public async Task AreConsideredEqual_WithDefaultTolerance_ShouldReadItForEachComparison()
	{
		int tolerance = 1;
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => tolerance);
		IEqualityEvaluator<int> evaluator = Evaluator(sut);

		bool first = await evaluator.AreConsideredEqual(1, 2);
		tolerance = 0;
		bool second = await evaluator.AreConsideredEqual(1, 2);

		await That(first).IsTrue();
		await That(second).IsFalse()
			.Because("a comparison outside an evaluation has no evaluation to read the default tolerance once for");
	}

	[Test]
	public async Task ForEvaluation_ShouldReadTheDefaultToleranceOnce()
	{
		int reads = 0;
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => ++reads);

		IEqualityEvaluator<int> evaluation =
			((IEqualityOptions<int>)sut.Value).ForEvaluation(new Context(), CancellationToken.None);
		bool first = await evaluation.AreConsideredEqual(1, 2);
		bool second = await evaluation.AreConsideredEqual(1, 3);

		await That(first).IsTrue();
		await That(second).IsFalse()
			.Because("the tolerance of 1 read for the evaluation must not be read again for the second comparison");
		await That(reads).IsEqualTo(1);
	}

	[Test]
	public async Task ForEvaluation_WithExplicitTolerance_ShouldReturnTheSameMatcher()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => 5);
		sut.Within(1);

		ObjectMatcher<int> first = sut.Value.ForEvaluation(new MatchEnvironment(new Context(), CancellationToken.None));
		ObjectMatcher<int> second = sut.Value.ForEvaluation(new MatchEnvironment(new Context(), CancellationToken.None));

		await That(second).IsSameAs(first)
			.Because("an explicit tolerance does not depend on a setting");
	}

	[Test]
	public async Task ForEvaluation_WithoutDefaultTolerance_ShouldCompareLikeTheDefault()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();

		ObjectMatcher<int> result = sut.Value.ForEvaluation(default);

		await That(result).IsSameAs(ObjectMatchType<int>.Default.ForEvaluation(default));
	}

	[Test]
	public async Task ForEvaluation_WithoutEvaluation_ShouldReturnTheSameMatcher()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => 5);

		ObjectMatcher<int> first = sut.Value.ForEvaluation(default);
		ObjectMatcher<int> second = sut.Value.ForEvaluation(default);

		await That(second).IsSameAs(first)
			.Because("the matcher outside an evaluation reads the default tolerance for each comparison itself");
	}

	[Test]
	[Arguments(false, "is equal to 3 ± 2")]
	[Arguments(true, "is not equal to 3 ± 2")]
	public async Task GetExpectation_ShouldNameTheTolerance(bool isNegated, string expected)
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(2);

		string result =
			sut.Value.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task GetItemExpectation_ShouldNameTheTolerance()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(2);

		string result = sut.Value.GetItemExpectation("3", "an item", "equal to");

		await That(result).IsEqualTo("an item equal to 3 ± 2");
	}

	[Test]
	[Arguments(3, true)]
	[Arguments(4, false)]
	public async Task MatchWithExplanation_ShouldDecideWithTheTolerance(int expected, bool expectMatch)
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(2);
		ObjectMatcher<int> matcher = sut.Value.ForEvaluation(default);

		MatchResult result = await matcher.CompareWithExplanation(1, expected);

		await That(result.IsMatch).IsEqualTo(expectMatch);
		await That(matcher.GetFailure("it", ExpectationGrammars.None, 1, expected, result, null))
			.IsEqualTo("it was 1");
	}

	[Test]
	[Arguments(0, "")]
	[Arguments(5, " ± 5")]
	public async Task ToString_WithDefaultTolerance_ShouldOnlyNameAToleranceOtherThanZero(int defaultTolerance,
		string expected)
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => defaultTolerance);

		string? result = sut.Value.ToString();

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task Using_WhenToleranceIsSpecified_ShouldThrowInvalidOperationException()
	{
		OptionsHolder<ObjectMatchType<double>> sut =
			new(ObjectMatchType<double>.WithTolerance<double>((a, e, t) => Math.Abs(a - e) <= t));
		sut.Within(0.1);

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with Within.")
			.Because("the comparer would silently replace the tolerance");
	}

	[Test]
	public async Task Using_WithDefaultTolerance_ShouldNotThrow()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => 5);

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).DoesNotThrow()
			.Because("a default tolerance is not an explicit option");
	}

	[Test]
	public async Task WhenTimeSpanToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		OptionsHolder<ObjectMatchType<DateTime>> sut =
			new(ObjectMatchType<DateTime>.WithTolerance<TimeSpan>((a, e, t) => a - e <= t));

		void Act() => sut.Within(TimeSpan.FromSeconds(-1));

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		OptionsHolder<ObjectMatchType<double>> sut =
			new(ObjectMatchType<double>.WithTolerance<double>((a, e, t) => Math.Abs(a - e) <= t));

		void Act() => sut.Within(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be NaN.").AsPrefix()
			.Because("NaN is neither negative nor a usable tolerance");
	}

	[Test]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		OptionsHolder<ObjectMatchType<double>> sut =
			new(ObjectMatchType<double>.WithTolerance<double>((a, e, t) => Math.Abs(a - e) <= t));

		void Act() => sut.Within(-1.0);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		OptionsHolder<ObjectMatchType<double>> sut =
			new(ObjectMatchType<double>.WithTolerance<double>((a, e, t) => Math.Abs(a - e) <= t));

		void Act() => sut.Within(0.0);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithRelativeTolerance_ShouldAppendTheUnitToTheTolerance()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(5).WithRelativeTolerance<int, int>((t, e) => t * e / 100, "%");

		string result = sut.Value.GetExpectation("100", ExpectationGrammars.None);

		await That(result).IsEqualTo("is equal to 100 ± 5%");
	}

	[Test]
	[Arguments(-105, -100, true)]
	[Arguments(105, 100, true)]
	[Arguments(106, 100, false)]
	public async Task WithRelativeTolerance_ShouldApplyTheToleranceRelativeToTheExpectedValue(int actual,
		int expected, bool expectMatch)
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(5).WithRelativeTolerance<int, int>((t, e) => t * Math.Abs(e) / 100, "%");

		bool result = await Evaluator(sut).AreConsideredEqual(actual, expected);

		await That(result).IsEqualTo(expectMatch);
	}

	[Test]
	public async Task WithRelativeTolerance_ShouldNotChangeTheMatchTypeItIsCalledOn()
	{
		ObjectToleranceMatchType<int, int> within = ObjectMatchType<int>
			.WithTolerance<int>((a, e, t) => Math.Abs(a - e) <= t)
			.Within(5);

		ObjectToleranceMatchType<int, int> result = within.WithRelativeTolerance((t, e) => t * e / 100, "%");

		await That(result).IsNotSameAs(within);
		await That(within.ToString()).IsEqualTo(" ± 5");
		await That(result.ToString()).IsEqualTo(" ± 5%");
	}

	[Test]
	public async Task WithRelativeTolerance_WhenEvaluated_ShouldKeepTheRelativeTolerance()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(5).WithRelativeTolerance<int, int>((t, e) => t * e / 100, "%");

		IEqualityEvaluator<int> evaluation =
			((IEqualityOptions<int>)sut.Value).ForEvaluation(new Context(), CancellationToken.None);
		bool result = await evaluation.AreConsideredEqual(210, 200);

		await That(result).IsTrue();
	}

	[Test]
	public async Task WithRelativeTolerance_WhenSubjectIsNullable_ShouldApplyItToTheExpectedValue()
	{
		OptionsHolder<ObjectMatchType<int?>> sut = new(ObjectMatchType<int?>.WithTolerance<int>((a, e, t)
			=> a is not null && e is not null && Math.Abs(a.Value - e.Value) <= t));
		sut.Within(5).WithRelativeTolerance<int?, int>((t, e) => t * e / 100, "%");

		bool result = await Evaluator(sut).AreConsideredEqual((int?)210, (int?)200);

		await That(result).IsTrue()
			.Because("the nullable expected value 200 makes 5% a tolerance of 10");
	}

	[Test]
	public async Task WithRelativeTolerance_WithoutWithin_ShouldNotChangeTheComparison()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.WithRelativeTolerance<int, int>((t, e) => t * e / 100, "%");

		bool result = await Evaluator(sut).AreConsideredEqual(201, 200);
		string expectation = sut.Value.GetExpectation("200", ExpectationGrammars.None);

		await That(result).IsFalse();
		await That(expectation).IsEqualTo("is equal to 200");
	}

	[Test]
	public async Task Within_ShouldNameTheOption()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();

		sut.Within(5);

		await That(sut.Value.OptionName).IsEqualTo("Within");
	}

	[Test]
	public async Task Within_ShouldNotChangeTheStartingValue()
	{
		ObjectToleranceMatchType<int, int> start = ObjectMatchType<int>
			.WithTolerance<int>((a, e, t) => Math.Abs(a - e) <= t);
		OptionsHolder<ObjectMatchType<int>> first = new(start);
		OptionsHolder<ObjectMatchType<int>> second = new(start);

		first.Within(5);

		await That(second.Value).IsSameAs(start);
		await That(start.OptionName).IsNull();
		await That(await Evaluator(second).AreConsideredEqual(1, 6)).IsFalse()
			.Because("a shared starting value must not be changed by another expectation");
	}

	[Test]
	public async Task Within_WhenComparerIsSpecified_ShouldThrowInvalidOperationException()
	{
		OptionsHolder<ObjectMatchType<double>> sut =
			new(ObjectMatchType<double>.WithTolerance<double>((a, e, t) => Math.Abs(a - e) <= t));
		sut.Using(new AllEqualComparer());

		void Act() => sut.Within(0.1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be combined with Using.")
			.Because("the tolerance would silently replace the comparer");
	}

	[Test]
	public async Task Within_WhenTheComparisonHasNoTolerance_ShouldThrowInvalidOperationException()
	{
		OptionsHolder<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);

		void Act() => sut.Within(0.1);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within requires a comparison within a tolerance of type double.");
	}

	[Test]
	public async Task Within_WhenToleranceIsRejectedByTheValidation_ShouldThrowItsException()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(RequireEven);

		void Act() => sut.Within(3);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("t").And
			.WithMessage("The tolerance must be even.").AsPrefix();
	}

	[Test]
	public async Task Within_WhenToleranceIsSpecified_ShouldUseTheLastTolerance()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut();
		sut.Within(1).WithRelativeTolerance<int, int>((t, e) => t * e / 100, "%");

		sut.Within(5);

		await That(sut.Value.GetExpectation("100", ExpectationGrammars.None)).IsEqualTo("is equal to 100 ± 5")
			.Because("the later tolerance also replaces the earlier relative tolerance");
		await That(await Evaluator(sut).AreConsideredEqual(1, 6)).IsTrue();
	}

	[Test]
	public async Task Within_WithDefaultTolerance_ShouldNotThrow()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(defaultTolerance: () => 5);

		void Act() => sut.Within(1);

		await That(Act).DoesNotThrow()
			.Because("a default tolerance is not an explicit option");
	}

	[Test]
	public async Task Within_WithToleranceValidation_WhenToleranceIsAccepted_ShouldApplyIt()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(_ => { });
		sut.Within(2);

		bool result = await Evaluator(sut).AreConsideredEqual(1, 3);

		await That(result).IsTrue();
	}

	[Test]
	public async Task Within_WithToleranceValidation_WhenToleranceIsSpecifiedTwice_ShouldValidateTheLaterTolerance()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(RequireEven);
		sut.Within(2);

		void Act() => sut.Within(3);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must be even.").AsPrefix();
		await That(await Evaluator(sut).AreConsideredEqual(1, 3)).IsTrue()
			.Because("the rejected tolerance must leave the earlier one in place");
	}

	[Test]
	public async Task Within_WithToleranceValidation_WhenToleranceWasRejected_ShouldAcceptALaterTolerance()
	{
		OptionsHolder<ObjectMatchType<int>> sut = CreateSut(RequireEven);
		await That(() => sut.Within(3)).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must be even.").AsPrefix();

		void Act() => sut.Within(2);

		await That(Act).DoesNotThrow()
			.Because("the rejected tolerance must leave the options unchanged");
		await That(await Evaluator(sut).AreConsideredEqual(1, 3)).IsTrue();
	}

	private static OptionsHolder<ObjectMatchType<int>> CreateSut(Action<int>? validation = null,
		Func<int>? defaultTolerance = null)
		=> new(ObjectMatchType<int>.WithTolerance<int>((a, e, t) => Math.Abs(a - e) <= t,
			validation: validation, defaultTolerance: defaultTolerance));

#pragma warning disable CA1859 // The ObjectMatcher implements AreConsideredEqual explicitly, so the tests need the interface
	private static IEqualityEvaluator<T> Evaluator<T>(OptionsHolder<ObjectMatchType<T>> sut)
		=> sut.Value.ForEvaluation(default);
#pragma warning restore CA1859

	private static void RequireEven(int t)
	{
		if (t % 2 != 0)
		{
			throw new ArgumentOutOfRangeException(nameof(t), "The tolerance must be even.");
		}
	}

	private sealed class AllEqualComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => true;

		public int GetHashCode(object obj) => 0;
	}
}
