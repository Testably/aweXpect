using aweXpect.Chronology;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class LongTests
	{
		[Fact]
		public async Task Between_ShouldTriggerValidationForMaximum()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(42L).And(43L);

			await That(signal).Signaled().With(e => e == 43L);
		}

		[Fact]
		public async Task Between_ShouldTriggerValidationForMinimum()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(42L).And(43L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task Between_ShouldVerifyThatActualIsBetweenMinimumAndMaximum()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.Between(41L).And(43L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task Between_WhenActualIsOutsideTheRange_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			async Task Act()
				=> await sut.Between(43L).And(44L);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value between 43 and 44,
				             but it had long value 42
				             """);
		}

		[Fact]
		public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			async Task Act()
				=> await sut.Between(44L).And(43L);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Theory]
		[InlineData(null, 1L)]
		[InlineData(1L, null)]
		public async Task Between_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(long? minimum, long? maximum)
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.Between(minimum).And(maximum));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              does not have long value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had long value 0
				              """)
				.Because("nothing can be ordered against a null bound, so the negation fails as well");
		}

		[Fact]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo(42L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.EqualTo(42L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task EqualTo_WhenAnEarlierEvaluationThrew_ShouldDescribeTheLastValue()
		{
			int calls = 0;
			Func<int> subject = () => 1;
			PropertyResult.Long<int> sut = new(
				That(subject).Eventually().Within(1.Seconds()).CheckEvery(10.Milliseconds()),
				_ => calls++ == 0 ? throw new InvalidOperationException("not ready") : 41L,
				"long value");

			async Task Act()
				=> await sut.EqualTo(42L);

			XunitException exception = await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             eventually has long value equal to 42 within 0:01,
				             but it had long value 41
				             """);
			await That(exception.InnerException).IsNull()
				.Because("only the first evaluation could not read the property");
		}

		[Fact]
		public async Task EqualTo_WhenReadingThePropertyThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("foo");
			PropertyResult.Long<MyClass?> sut = MyClass.HasThrowingLongValue(exception);

			async Task Act()
				=> await sut.EqualTo(42L);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value equal to 42,
				             but long value did throw an InvalidOperationException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception))
				.Because("a property that cannot be read fails the expectation, whatever it throws");
		}

		[Fact]
		public async Task EqualTo_WhenValueIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasNullLongValue();

			async Task Act()
				=> await sut.EqualTo(42L);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value equal to 42,
				             but it had long value <null>
				             """);
		}

		[Fact]
		public async Task EqualTo_WhenValueIsNullAndExpectedIsNull_ShouldSucceed()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasNullLongValue();

			async Task Act()
				=> await sut.EqualTo(null);

			await That(Act).DoesNotThrow()
				.Because("null is equal to null");
		}

		[Fact]
		public async Task GreaterThan_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.GreaterThan(42L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task GreaterThan_ShouldVerifyThatActualIsGreaterThanExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.GreaterThan(41L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task GreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.GreaterThan(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task GreaterThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.GreaterThanOrEqualTo(42L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task GreaterThanOrEqualTo_ShouldVerifyThatActualIsGreaterThanOrEqualToExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.GreaterThanOrEqualTo(42L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task GreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.GreaterThanOrEqualTo(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than or equal to <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task LessThan_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.LessThan(42L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task LessThan_ShouldVerifyThatActualIsLessThanExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.LessThan(43L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task LessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.LessThan(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task LessThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.LessThanOrEqualTo(42L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task LessThanOrEqualTo_ShouldVerifyThatActualIsLessThanOrEqualToExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.LessThanOrEqualTo(42L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task LessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.LessThanOrEqualTo(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than or equal to <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task NotBetween_ShouldTriggerValidationForMaximum()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotBetween(42).And(43);

			await That(signal).Signaled().With(e => e == 43);
		}

		[Fact]
		public async Task NotBetween_ShouldTriggerValidationForMinimum()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotBetween(42).And(43);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task NotBetween_ShouldVerifyThatActualIsNotBetweenMinimumAndMaximum()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			MyClass? result = await sut.NotBetween(43).And(44);

			await That(result?.LongValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotBetween_WhenActualIsInsideTheRange_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotBetween(42).And(43);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value between 42 and 43,
				             but it had long value 42
				             """);
		}

		[Fact]
		public async Task NotBetween_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotBetween(44).And(43);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Theory]
		[InlineData(null, 1L)]
		[InlineData(1L, null)]
		public async Task NotBetween_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(long? minimum, long? maximum)
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotBetween(minimum).And(maximum));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has long value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had long value 0
				              """)
				.Because("nothing can be ordered against a null bound, so the negation fails as well");
		}

		[Theory]
		[InlineData(null, 1L)]
		[InlineData(1L, null)]
		public async Task NotBetween_WhenMinimumOrMaximumIsNull_ShouldFail(long? minimum, long? maximum)
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotBetween(minimum).And(maximum);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              does not have long value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had long value 42
				              """)
				.Because("nothing can be ordered against a null bound");
		}

		[Fact]
		public async Task NotBetween_WhenNegated_ShouldExpectTheRange()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotBetween(1).And(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value between 1 and 2,
				             but it had long value 0
				             """);
		}

		[Fact]
		public async Task NotBetween_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValueOfNullSubject();

			async Task Act()
				=> await sut.NotBetween(41).And(43);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value between 41 and 43,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task NotEqualTo_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0L, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotEqualTo(42L);

			await That(signal).Signaled().With(e => e == 42L);
		}

		[Fact]
		public async Task NotEqualTo_ShouldVerifyThatActualIsNotEqualToExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42L);

			MyClass? result = await sut.NotEqualTo(41L);

			await That(result?.LongValue).IsEqualTo(42L);
		}

		[Fact]
		public async Task NotEqualTo_WhenReadingThePropertyThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("foo");
			PropertyResult.Long<MyClass?> sut = MyClass.HasThrowingLongValue(exception);

			async Task Act()
				=> await sut.NotEqualTo(42L);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value equal to 42,
				             but long value did throw an InvalidOperationException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception))
				.Because("a property that was never read cannot prove inequality either");
		}

		[Fact]
		public async Task NotEqualTo_WhenValueIsNull_ShouldSucceed()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasNullLongValue();

			async Task Act()
				=> await sut.NotEqualTo(42L);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task NotEqualTo_WhenValueIsNullAndUnexpectedIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasNullLongValue();

			async Task Act()
				=> await sut.NotEqualTo(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value equal to <null>,
				             but it had long value <null>
				             """)
				.Because("null is equal to null");
		}

		[Fact]
		public async Task NotGreaterThan_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotGreaterThan(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task NotGreaterThan_ShouldVerifyThatActualIsNotGreaterThanExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			MyClass? result = await sut.NotGreaterThan(42);

			await That(result?.LongValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotGreaterThan_WhenActualIsGreaterThanExpected_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotGreaterThan(41);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than 41,
				             but it had long value 42
				             """);
		}

		[Fact]
		public async Task NotGreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotGreaterThan(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value greater than <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task NotGreaterThan_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotGreaterThan(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than <null>,
				             but it had long value 42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Fact]
		public async Task NotGreaterThan_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotGreaterThan(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value greater than 1,
				             but it had long value 0
				             """);
		}

		[Fact]
		public async Task NotGreaterThan_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValueOfNullSubject();

			async Task Act()
				=> await sut.NotGreaterThan(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than 42,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotGreaterThanOrEqualTo(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_ShouldVerifyThatActualIsNotGreaterThanOrEqualToExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			MyClass? result = await sut.NotGreaterThanOrEqualTo(43);

			await That(result?.LongValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_WhenActualIsEqualToExpected_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than or equal to 42,
				             but it had long value 42
				             """);
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotGreaterThanOrEqualTo(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value greater than or equal to <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than or equal to <null>,
				             but it had long value 42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotGreaterThanOrEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value greater than or equal to 1,
				             but it had long value 0
				             """);
		}

		[Fact]
		public async Task NotGreaterThanOrEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValueOfNullSubject();

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value greater than or equal to 42,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task NotLessThan_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotLessThan(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task NotLessThan_ShouldVerifyThatActualIsNotLessThanExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			MyClass? result = await sut.NotLessThan(42);

			await That(result?.LongValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotLessThan_WhenActualIsLessThanExpected_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotLessThan(43);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than 43,
				             but it had long value 42
				             """);
		}

		[Fact]
		public async Task NotLessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotLessThan(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value less than <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task NotLessThan_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotLessThan(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than <null>,
				             but it had long value 42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Fact]
		public async Task NotLessThan_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotLessThan(-1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value less than -1,
				             but it had long value 0
				             """);
		}

		[Fact]
		public async Task NotLessThan_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValueOfNullSubject();

			async Task Act()
				=> await sut.NotLessThan(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than 42,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<long?> signal = new();
			PropertyResult.Long<string> sut = new(new Dummy(), _ => 0, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotLessThanOrEqualTo(42);

			await That(signal).Signaled().With(e => e == 42);
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_ShouldVerifyThatActualIsNotLessThanOrEqualToExpected()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			MyClass? result = await sut.NotLessThanOrEqualTo(41);

			await That(result?.LongValue).IsEqualTo(42);
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_WhenActualIsEqualToExpected_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than or equal to 42,
				             but it had long value 42
				             """);
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotLessThanOrEqualTo(null));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value less than or equal to <null>,
				             but it had long value 0
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValue(42);

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(null);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than or equal to <null>,
				             but it had long value 42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
					.NotLessThanOrEqualTo(-1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value less than or equal to -1,
				             but it had long value 0
				             """);
		}

		[Fact]
		public async Task NotLessThanOrEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut = MyClass.HasLongValueOfNullSubject();

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(42);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have long value less than or equal to 42,
				             but it was <null>
				             """);
		}

		public sealed class GrammarTests
		{
			[Fact]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.Long<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasLongValue(42L, ExpectationGrammars.Active);

				async Task Act()
					=> await sut.GreaterThan(43L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with long value greater than 43,
					             but it had long value 42
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldUseDoesNotHave()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
						.EqualTo(0L));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have long value equal to 0,
					             but it had long value 0
					             """);
			}

			[Fact]
			public async Task WhenNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.None)
						.NotEqualTo(1L));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has long value equal to 1,
					             but it had long value 0
					             """);
			}

			[Fact]
			public async Task WhenNested_ShouldNameTheProperty()
			{
				PropertyResult.Long<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasLongValue(42L, ExpectationGrammars.Nested);

				async Task Act()
					=> await sut.GreaterThan(43L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose long value is greater than 43,
					             but long value was 42
					             """);
			}

			[Fact]
			public async Task WhenPluralAndNegated_ShouldUseThePluralVerb()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.LongValueOf(s, ExpectationGrammars.Plural)
						.EqualTo(0L));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             do not have long value equal to 0,
					             but it had long value 0
					             """);
			}
		}
	}
}
