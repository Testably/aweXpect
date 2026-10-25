using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class DecimalTests
	{
		[Test]
		public async Task Between_ShouldTriggerValidationForMinimumAndMaximum()
		{
			Signaler<decimal?> signal = new();
			PropertyResult.Decimal<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(4.2m).And(4.3m);

			await That(signal).Signaled(2.Times());
		}

		[Test]
		public async Task Between_ShouldVerifyThatActualIsInTheRange()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			MyClass? result = await sut.Between(4.2m).And(4.3m);

			await That(result?.DecimalValue).IsEqualTo(4.2m);
		}

		[Test]
		public async Task Between_WhenActualIsOutsideTheRange_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.Between(4.3m).And(4.4m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value between 4.3 and 4.4,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.Between(4.4m).And(4.3m);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Test]
		public async Task Between_WhenMinimumIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.DecimalValueOf(s, ExpectationGrammars.None)
					.Between(null).And(1m));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value between <null> and 1.0,
				             but it had decimal value 0.0
				             """)
				.Because("nothing can be ordered against a null bound, so the negation fails as well");
		}

		[Test]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<decimal?> signal = new();
			PropertyResult.Decimal<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo(4.2m);

			await That(signal).Signaled().With(e => e == 4.2m);
		}

		[Test]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			MyClass? result = await sut.EqualTo(4.2m);

			await That(result?.DecimalValue).IsEqualTo(4.2m);
		}

		[Test]
		public async Task EqualTo_WhenActualDiffers_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.EqualTo(4.3m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value equal to 4.3,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task EqualTo_WhenDifferenceOverflows_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(decimal.MaxValue);

			async Task Act()
				=> await sut.EqualTo(decimal.MinValue).Within(decimal.MaxValue);

			await That(Act).Throws<FailException>()
				.Because("a difference beyond the range of decimal is not within any tolerance, like for IsEqualTo")
				.WithMessage("""
				             Expected that subject
				             has decimal value equal to decimal.MinValue ± decimal.MaxValue,
				             but it had decimal value decimal.MaxValue
				             """);
		}

		[Test]
		public async Task EqualTo_WhenPropertyIsNull_ShouldOnlyBeEqualToNull()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasNullDecimalValue();

			async Task Act()
				=> await sut.EqualTo(null);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task EqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValueOfNullSubject();

			async Task Act()
				=> await sut.EqualTo(4.2m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value equal to 4.2,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task EqualTo_WithinPercent_ShouldApplyTheRelativeTolerance()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(98m);

			async Task Act()
				=> await sut.EqualTo(100m).Within(1).Percent();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value equal to 100.0 ± 1.0%,
				             but it had decimal value 98.0
				             """);
		}

		[Test]
		public async Task EqualTo_WithinTolerance_ShouldApplyTheTolerance()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.EqualTo(4.25m).Within(0.1m);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task GreaterThan_WhenActualIsNotGreater_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.GreaterThan(4.2m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value greater than 4.2,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task GreaterThanOrEqualTo_WhenActualIsLess_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.GreaterThanOrEqualTo(4.3m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value greater than or equal to 4.3,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task LessThan_WhenActualIsNotLess_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.LessThan(4.2m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value less than 4.2,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task LessThanOrEqualTo_WhenActualIsGreater_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.LessThanOrEqualTo(4.1m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has decimal value less than or equal to 4.1,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task NotBetween_WhenActualIsInTheRange_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.NotBetween(4.1m).And(4.3m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value between 4.1 and 4.3,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task NotEqualTo_WhenActualIsEqual_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.NotEqualTo(4.2m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value equal to 4.2,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task NotGreaterThan_WhenActualIsGreater_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.NotGreaterThan(4.1m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value greater than 4.1,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenActualIsEqual_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(4.2m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value greater than or equal to 4.2,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task NotLessThan_WhenActualIsLess_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.NotLessThan(4.3m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value less than 4.3,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenActualIsEqual_ShouldFail()
		{
			PropertyResult.Decimal<MyClass?> sut = MyClass.HasDecimalValue(4.2m);

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(4.2m);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have decimal value less than or equal to 4.2,
				             but it had decimal value 4.2
				             """);
		}

		[Test]
		public async Task Operators_WhenMet_ShouldSucceed()
		{
			async Task Act()
			{
				await MyClass.HasDecimalValue(4.2m).GreaterThan(4.1m);
				await MyClass.HasDecimalValue(4.2m).GreaterThanOrEqualTo(4.2m);
				await MyClass.HasDecimalValue(4.2m).LessThan(4.3m);
				await MyClass.HasDecimalValue(4.2m).LessThanOrEqualTo(4.2m);
				await MyClass.HasDecimalValue(4.2m).NotBetween(4.3m).And(4.4m);
				await MyClass.HasDecimalValue(4.2m).NotEqualTo(4.3m);
				await MyClass.HasDecimalValue(4.2m).NotGreaterThan(4.2m);
				await MyClass.HasDecimalValue(4.2m).NotGreaterThanOrEqualTo(4.3m);
				await MyClass.HasDecimalValue(4.2m).NotLessThan(4.2m);
				await MyClass.HasDecimalValue(4.2m).NotLessThanOrEqualTo(4.1m);
			}

			await That(Act).DoesNotThrow();
		}
	}
}
