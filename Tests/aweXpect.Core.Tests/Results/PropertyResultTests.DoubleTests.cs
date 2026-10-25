using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class DoubleTests
	{
		[Test]
		public async Task Between_ShouldTriggerValidationForMaximum()
		{
			Signaler<double?> signal = new();
			PropertyResult.Double<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(4.2).And(4.3);

			await That(signal).Signaled().With(e => e == 4.3);
		}

		[Test]
		public async Task Between_ShouldTriggerValidationForMinimum()
		{
			Signaler<double?> signal = new();
			PropertyResult.Double<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(4.2).And(4.3);

			await That(signal).Signaled().With(e => e == 4.2);
		}

		[Test]
		[Arguments(4.1, 4.3)]
		[Arguments(4.2, 4.2)]
		public async Task Between_WhenActualIsInTheRange_ShouldSucceed(double minimum, double maximum)
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.Between(minimum).And(maximum);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task Between_WhenActualIsOutsideTheRange_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.Between(4.3).And(4.4);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value between 4.3 and 4.4,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.Between(4.4).And(4.3);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Test]
		public async Task Between_WhenMaximumIsNaN_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.Between(4.1).And(double.NaN);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be NaN.").AsPrefix();
		}

		[Test]
		public async Task Between_WhenMinimumIsNaN_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.Between(double.NaN).And(4.3);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("minimum").And
				.WithMessage("The minimum must not be NaN.").AsPrefix();
		}

		[Test]
		[Arguments(null, 1.0)]
		[Arguments(1.0, null)]
		public async Task Between_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(double? minimum, double? maximum)
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.DoubleValueOf(s, ExpectationGrammars.None)
					.Between(minimum).And(maximum));

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              does not have double value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had double value 0.0
				              """)
				.Because("nothing can be ordered against a null bound, so the negation fails as well");
		}

		[Test]
		public async Task Between_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValueOfNullSubject();

			async Task Act()
				=> await sut.Between(4.1).And(4.3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value between 4.1 and 4.3,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<double?> signal = new();
			PropertyResult.Double<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo(4.2);

			await That(signal).Signaled().With(e => e == 4.2);
		}

		[Test]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.EqualTo(4.2);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task EqualTo_WhenActualDiffers_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.EqualTo(4.3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value equal to 4.3,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task EqualTo_WhenActualIsInfinity_ShouldOnlyBeEqualToTheSameInfinity()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(double.PositiveInfinity);

			async Task Act()
				=> await sut.EqualTo(double.MaxValue).Within(double.PositiveInfinity);

			await That(Act).Throws<FailException>()
				.Because("an infinite value has no finite distance to any other value, like for IsEqualTo")
				.WithMessage("""
				             Expected that subject
				             has double value equal to double.MaxValue ± +∞,
				             but it had double value +∞
				             """);
		}

		[Test]
		public async Task EqualTo_WhenActualIsNaN_ShouldFailForANumber()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(double.NaN);

			async Task Act()
				=> await sut.EqualTo(4.2).Within(1000);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value equal to 4.2 ± 1000.0,
				             but it had double value NaN
				             """);
		}

		[Test]
		public async Task EqualTo_WhenBothAreNaN_ShouldSucceed()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(double.NaN);

			async Task Act()
				=> await sut.EqualTo(double.NaN);

			await That(Act).DoesNotThrow()
				.Because("NaN is equal to NaN, like for IsEqualTo");
		}

		[Test]
		public async Task EqualTo_WhenPropertyIsNull_ShouldOnlyBeEqualToNull()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasNullDoubleValue();

			async Task Act()
				=> await sut.EqualTo(null);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task EqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValueOfNullSubject();

			async Task Act()
				=> await sut.EqualTo(4.2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value equal to 4.2,
				             but it was <null>
				             """);
		}

		[Test]
		[Arguments(4.15, true)]
		[Arguments(4.3, false)]
		public async Task EqualTo_WithinPercent_ShouldApplyTheRelativeTolerance(double expected, bool isWithin)
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.EqualTo(expected).Within(2).Percent();

			await That(Act).Throws<FailException>().OnlyIf(!isWithin)
				.WithMessage($"""
				              Expected that subject
				              has double value equal to {Formatter.Format(expected)} ± 2.0%,
				              but it had double value 4.2
				              """);
		}

		[Test]
		[Arguments(4.25, true)]
		[Arguments(4.4, false)]
		public async Task EqualTo_WithinTolerance_ShouldApplyTheTolerance(double expected, bool isWithin)
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.EqualTo(expected).Within(0.1);

			await That(Act).Throws<FailException>().OnlyIf(!isWithin)
				.WithMessage($"""
				              Expected that subject
				              has double value equal to {Formatter.Format(expected)} ± 0.1,
				              but it had double value 4.2
				              """);
		}

		[Test]
		public async Task GreaterThan_ShouldVerifyThatActualIsGreaterThanExpected()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.GreaterThan(4.1);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task GreaterThan_WhenActualIsNaN_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(double.NaN);

			async Task Act()
				=> await sut.GreaterThan(4.2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value greater than 4.2,
				             but it had double value NaN
				             """);
		}

		[Test]
		[Arguments(4.2)]
		[Arguments(4.3)]
		public async Task GreaterThan_WhenActualIsNotGreater_ShouldFail(double expected)
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.GreaterThan(expected);

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              has double value greater than {Formatter.Format(expected)},
				              but it had double value 4.2
				              """);
		}

		[Test]
		public async Task GreaterThan_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.GreaterThan(double.NaN);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("expected").And
				.WithMessage("The expected value must not be NaN.").AsPrefix();
		}

		[Test]
		public async Task GreaterThan_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.GreaterThan(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value greater than <null>,
				             but it had double value 4.2
				             """);
		}

		[Test]
		[Arguments(4.1)]
		[Arguments(4.2)]
		public async Task GreaterThanOrEqualTo_ShouldVerifyThatActualIsGreaterThanOrEqualToExpected(double expected)
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.GreaterThanOrEqualTo(expected);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task GreaterThanOrEqualTo_WhenActualIsLess_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.GreaterThanOrEqualTo(4.3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value greater than or equal to 4.3,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task LessThan_ShouldVerifyThatActualIsLessThanExpected()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.LessThan(4.3);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task LessThan_WhenActualIsNotLess_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.LessThan(4.2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value less than 4.2,
				             but it had double value 4.2
				             """);
		}

		[Test]
		[Arguments(4.2)]
		[Arguments(4.3)]
		public async Task LessThanOrEqualTo_ShouldVerifyThatActualIsLessThanOrEqualToExpected(double expected)
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.LessThanOrEqualTo(expected);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task LessThanOrEqualTo_WhenActualIsGreater_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.LessThanOrEqualTo(4.1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has double value less than or equal to 4.1,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotBetween_ShouldVerifyThatActualIsOutsideTheRange()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.NotBetween(4.3).And(4.4);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task NotBetween_WhenActualIsInTheRange_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotBetween(4.1).And(4.3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value between 4.1 and 4.3,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotBetween_WhenActualIsNaN_ShouldSucceed()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(double.NaN);

			async Task Act()
				=> await sut.NotBetween(4.1).And(4.3);

			await That(Act).DoesNotThrow()
				.Because("NaN is not between any bounds");
		}

		[Test]
		public async Task NotEqualTo_ShouldVerifyThatActualIsNotEqualToUnexpected()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.NotEqualTo(4.3);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task NotEqualTo_WhenActualIsEqual_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotEqualTo(4.2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value equal to 4.2,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotEqualTo_WithinTolerance_ShouldFailWithinTheTolerance()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotEqualTo(4.25).Within(0.1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value equal to 4.25 ± 0.1,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotGreaterThan_ShouldVerifyThatActualIsNotGreaterThanUnexpected()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.NotGreaterThan(4.2);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task NotGreaterThan_WhenActualIsGreater_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotGreaterThan(4.1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value greater than 4.1,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotGreaterThan_WhenActualIsNaN_ShouldSucceed()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(double.NaN);

			async Task Act()
				=> await sut.NotGreaterThan(4.2);

			await That(Act).DoesNotThrow()
				.Because("NaN is not greater than any value, like for IsNotGreaterThan");
		}

		[Test]
		public async Task NotGreaterThan_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotGreaterThan(double.NaN);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("unexpected").And
				.WithMessage("The unexpected value must not be NaN.").AsPrefix();
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_ShouldVerifyThatActualIsLess()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.NotGreaterThanOrEqualTo(4.3);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenActualIsEqual_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(4.2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value greater than or equal to 4.2,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotLessThan_ShouldVerifyThatActualIsNotLessThanUnexpected()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.NotLessThan(4.2);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task NotLessThan_WhenActualIsLess_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotLessThan(4.3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value less than 4.3,
				             but it had double value 4.2
				             """);
		}

		[Test]
		public async Task NotLessThanOrEqualTo_ShouldVerifyThatActualIsGreater()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			MyClass? result = await sut.NotLessThanOrEqualTo(4.1);

			await That(result?.DoubleValue).IsEqualTo(4.2);
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenActualIsEqual_ShouldFail()
		{
			PropertyResult.Double<MyClass?> sut = MyClass.HasDoubleValue(4.2);

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(4.2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have double value less than or equal to 4.2,
				             but it had double value 4.2
				             """);
		}
	}
}
