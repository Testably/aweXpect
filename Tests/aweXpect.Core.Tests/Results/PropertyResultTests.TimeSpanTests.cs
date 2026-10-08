using aweXpect.Chronology;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class TimeSpanTests
	{
		[Test]
		public async Task Between_ShouldTriggerValidationForMaximum()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(42.Seconds()).And(43.Seconds());

			await That(signal).Signaled().With(e => e == 43.Seconds());
		}

		[Test]
		public async Task Between_ShouldTriggerValidationForMinimum()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.Between(42.Seconds()).And(43.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task Between_ShouldVerifyThatActualIsBetweenMinimumAndMaximum()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.Between(41.Seconds()).And(43.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task Between_WhenActualIsAboveMaximum_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.Between(40.Seconds()).And(41.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value between 0:40 and 0:41,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		[Arguments(42, 43)]
		[Arguments(41, 42)]
		[Arguments(42, 42)]
		public async Task Between_WhenActualIsEqualToMinimumOrMaximum_ShouldSucceed(int minimumSeconds,
			int maximumSeconds)
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.Between(minimumSeconds.Seconds()).And(maximumSeconds.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task Between_WhenActualIsOutsideTheRange_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.Between(43.Seconds()).And(44.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value between 0:43 and 0:44,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.Between(44.Seconds()).And(43.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Test]
		[Arguments(null, 1)]
		[Arguments(1, null)]
		public async Task Between_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(int? minimumSeconds,
			int? maximumSeconds)
		{
			MyClass subject = new();
			TimeSpan? minimum = minimumSeconds?.Seconds();
			TimeSpan? maximum = maximumSeconds?.Seconds();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.Between(minimum).And(maximum));

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              does not have TimeSpan value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had TimeSpan value 0:00
				              """)
				.Because("nothing can be ordered against a null bound, so the negation fails as well");
		}

		[Test]
		[Arguments(null, 43)]
		[Arguments(41, null)]
		public async Task Between_WhenMinimumOrMaximumIsNull_ShouldFail(int? minimumSeconds, int? maximumSeconds)
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());
			TimeSpan? minimum = minimumSeconds?.Seconds();
			TimeSpan? maximum = maximumSeconds?.Seconds();

			async Task Act()
				=> await sut.Between(minimum).And(maximum);

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              has TimeSpan value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had TimeSpan value 0:42
				              """)
				.Because("nothing can be ordered against a null bound");
		}

		[Test]
		public async Task EqualTo_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.EqualTo(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.EqualTo(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task EqualTo_WhenValueIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasNullTimeSpanValue();

			async Task Act()
				=> await sut.EqualTo(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value equal to 0:42,
				             but it had TimeSpan value <null>
				             """);
		}

		[Test]
		public async Task EqualTo_WhenValueIsNullAndExpectedIsNull_ShouldSucceed()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasNullTimeSpanValue();

			async Task Act()
				=> await sut.EqualTo(null);

			await That(Act).DoesNotThrow()
				.Because("null is equal to null");
		}

		[Test]
		public async Task GreaterThan_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.GreaterThan(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task GreaterThan_ShouldVerifyThatActualIsGreaterThanExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.GreaterThan(41.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task GreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.GreaterThan(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task GreaterThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.GreaterThanOrEqualTo(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task GreaterThanOrEqualTo_ShouldVerifyThatActualIsGreaterThanOrEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.GreaterThanOrEqualTo(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task GreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.GreaterThanOrEqualTo(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than or equal to <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task LessThan_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.LessThan(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task LessThan_ShouldVerifyThatActualIsLessThanExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.LessThan(43.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task LessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.LessThan(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task LessThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.LessThanOrEqualTo(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task LessThanOrEqualTo_ShouldVerifyThatActualIsLessThanOrEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.LessThanOrEqualTo(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task LessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.LessThanOrEqualTo(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than or equal to <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task NotBetween_ShouldTriggerValidationForMaximum()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotBetween(42.Seconds()).And(43.Seconds());

			await That(signal).Signaled().With(e => e == 43.Seconds());
		}

		[Test]
		public async Task NotBetween_ShouldTriggerValidationForMinimum()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotBetween(42.Seconds()).And(43.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task NotBetween_ShouldVerifyThatActualIsNotBetweenMinimumAndMaximum()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotBetween(43.Seconds()).And(44.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotBetween_WhenActualIsAboveMaximum_ShouldSucceed()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotBetween(40.Seconds()).And(41.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotBetween_WhenActualIsEqualToMaximum_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotBetween(41.Seconds()).And(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value between 0:41 and 0:42,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task NotBetween_WhenActualIsInsideTheRange_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotBetween(42.Seconds()).And(43.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value between 0:42 and 0:43,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task NotBetween_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotBetween(44.Seconds()).And(43.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Test]
		[Arguments(null, 1)]
		[Arguments(1, null)]
		public async Task NotBetween_WhenMinimumOrMaximumIsNull_AndNegated_ShouldFail(int? minimumSeconds,
			int? maximumSeconds)
		{
			MyClass subject = new();
			TimeSpan? minimum = minimumSeconds?.Seconds();
			TimeSpan? maximum = maximumSeconds?.Seconds();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotBetween(minimum).And(maximum));

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              has TimeSpan value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had TimeSpan value 0:00
				              """)
				.Because("nothing can be ordered against a null bound, so the negation fails as well");
		}

		[Test]
		[Arguments(null, 1)]
		[Arguments(1, null)]
		public async Task NotBetween_WhenMinimumOrMaximumIsNull_ShouldFail(int? minimumSeconds, int? maximumSeconds)
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());
			TimeSpan? minimum = minimumSeconds?.Seconds();
			TimeSpan? maximum = maximumSeconds?.Seconds();

			async Task Act()
				=> await sut.NotBetween(minimum).And(maximum);

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              does not have TimeSpan value between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
				              but it had TimeSpan value 0:42
				              """)
				.Because("nothing can be ordered against a null bound");
		}

		[Test]
		public async Task NotBetween_WhenNegated_ShouldExpectTheRange()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotBetween(1.Seconds()).And(2.Seconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value between 0:01 and 0:02,
				             but it had TimeSpan value 0:00
				             """);
		}

		[Test]
		public async Task NotBetween_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValueOfNullSubject();

			async Task Act()
				=> await sut.NotBetween(41.Seconds()).And(43.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value between 0:41 and 0:43,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task NotEqualTo_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotEqualTo(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task NotEqualTo_ShouldVerifyThatActualIsNotEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotEqualTo(41.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotEqualTo_WhenValueIsNull_ShouldSucceed()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasNullTimeSpanValue();

			async Task Act()
				=> await sut.NotEqualTo(42.Seconds());

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task NotEqualTo_WhenValueIsNullAndUnexpectedIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasNullTimeSpanValue();

			async Task Act()
				=> await sut.NotEqualTo(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value equal to <null>,
				             but it had TimeSpan value <null>
				             """)
				.Because("null is equal to null");
		}

		[Test]
		public async Task NotGreaterThan_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotGreaterThan(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task NotGreaterThan_ShouldValidateTheUnexpectedParameter()
		{
			string? parameterName = null;
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (_, name) =>
			{
				parameterName = name;
			});

			_ = sut.NotGreaterThan(unexpected: 42.Seconds());

			await That(parameterName).IsEqualTo("unexpected");
		}

		[Test]
		public async Task NotGreaterThan_ShouldVerifyThatActualIsNotGreaterThanExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotGreaterThan(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotGreaterThan_WhenActualIsGreaterThanExpected_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotGreaterThan(41.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than 0:41,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task NotGreaterThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotGreaterThan(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value greater than <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task NotGreaterThan_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotGreaterThan(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than <null>,
				             but it had TimeSpan value 0:42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Test]
		public async Task NotGreaterThan_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotGreaterThan(1.Seconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value greater than 0:01,
				             but it had TimeSpan value 0:00
				             """);
		}

		[Test]
		public async Task NotGreaterThan_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValueOfNullSubject();

			async Task Act()
				=> await sut.NotGreaterThan(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than 0:42,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotGreaterThanOrEqualTo(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_ShouldValidateTheUnexpectedParameter()
		{
			string? parameterName = null;
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (_, name) =>
			{
				parameterName = name;
			});

			_ = sut.NotGreaterThanOrEqualTo(unexpected: 42.Seconds());

			await That(parameterName).IsEqualTo("unexpected");
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_ShouldVerifyThatActualIsNotGreaterThanOrEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotGreaterThanOrEqualTo(43.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenActualIsEqualToExpected_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than or equal to 0:42,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotGreaterThanOrEqualTo(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value greater than or equal to <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than or equal to <null>,
				             but it had TimeSpan value 0:42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotGreaterThanOrEqualTo(1.Seconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value greater than or equal to 0:01,
				             but it had TimeSpan value 0:00
				             """);
		}

		[Test]
		public async Task NotGreaterThanOrEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValueOfNullSubject();

			async Task Act()
				=> await sut.NotGreaterThanOrEqualTo(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value greater than or equal to 0:42,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task NotLessThan_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotLessThan(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task NotLessThan_ShouldValidateTheUnexpectedParameter()
		{
			string? parameterName = null;
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (_, name) =>
			{
				parameterName = name;
			});

			_ = sut.NotLessThan(unexpected: 42.Seconds());

			await That(parameterName).IsEqualTo("unexpected");
		}

		[Test]
		public async Task NotLessThan_ShouldVerifyThatActualIsNotLessThanExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotLessThan(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotLessThan_WhenActualIsLessThanExpected_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotLessThan(43.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than 0:43,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task NotLessThan_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotLessThan(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value less than <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task NotLessThan_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotLessThan(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than <null>,
				             but it had TimeSpan value 0:42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Test]
		public async Task NotLessThan_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotLessThan(-1.Seconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value less than -0:01,
				             but it had TimeSpan value 0:00
				             """);
		}

		[Test]
		public async Task NotLessThan_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValueOfNullSubject();

			async Task Act()
				=> await sut.NotLessThan(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than 0:42,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task NotLessThanOrEqualTo_ShouldTriggerValidation()
		{
			Signaler<TimeSpan?> signal = new();
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (e, _) =>
			{
				signal.Signal(e);
			});

			_ = sut.NotLessThanOrEqualTo(42.Seconds());

			await That(signal).Signaled().With(e => e == 42.Seconds());
		}

		[Test]
		public async Task NotLessThanOrEqualTo_ShouldValidateTheUnexpectedParameter()
		{
			string? parameterName = null;
			PropertyResult.TimeSpan<string> sut = new(new Dummy(), _ => TimeSpan.Zero, "foo", (_, name) =>
			{
				parameterName = name;
			});

			_ = sut.NotLessThanOrEqualTo(unexpected: 42.Seconds());

			await That(parameterName).IsEqualTo("unexpected");
		}

		[Test]
		public async Task NotLessThanOrEqualTo_ShouldVerifyThatActualIsNotLessThanOrEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotLessThanOrEqualTo(41.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenActualIsEqualToExpected_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than or equal to 0:42,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenExpectedIsNull_AndNegated_ShouldFail()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotLessThanOrEqualTo(null));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value less than or equal to <null>,
				             but it had TimeSpan value 0:00
				             """)
				.Because("nothing can be ordered against null, so the negation fails as well");
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenExpectedIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(null);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than or equal to <null>,
				             but it had TimeSpan value 0:42
				             """)
				.Because("nothing can be ordered against null");
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenNegated_ShouldExpectTheComparison()
		{
			MyClass subject = new();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
					.NotLessThanOrEqualTo(-1.Seconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value less than or equal to -0:01,
				             but it had TimeSpan value 0:00
				             """);
		}

		[Test]
		public async Task NotLessThanOrEqualTo_WhenSubjectIsNull_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValueOfNullSubject();

			async Task Act()
				=> await sut.NotLessThanOrEqualTo(42.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have TimeSpan value less than or equal to 0:42,
				             but it was <null>
				             """);
		}

		public sealed class GrammarTests
		{
			[Test]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.TimeSpan<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasTimeSpanValue(42.Seconds(), ExpectationGrammars.Active);

				async Task Act()
					=> await sut.GreaterThan(43.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             with TimeSpan value greater than 0:43,
					             but it had TimeSpan value 0:42
					             """);
			}

			[Test]
			public async Task WhenNegated_ShouldUseDoesNotHave()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
						.EqualTo(TimeSpan.Zero));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have TimeSpan value equal to 0:00,
					             but it had TimeSpan value 0:00
					             """);
			}

			[Test]
			public async Task WhenNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
						.NotEqualTo(1.Seconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has TimeSpan value equal to 0:01,
					             but it had TimeSpan value 0:00
					             """);
			}

			[Test]
			public async Task WhenNested_ShouldNameTheProperty()
			{
				PropertyResult.TimeSpan<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasTimeSpanValue(42.Seconds(), ExpectationGrammars.Nested);

				async Task Act()
					=> await sut.GreaterThan(43.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose TimeSpan value is greater than 0:43,
					             but TimeSpan value was 0:42
					             """);
			}

			[Test]
			public async Task WhenPluralAndNegated_ShouldUseThePluralVerb()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.Plural)
						.EqualTo(TimeSpan.Zero));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             do not have TimeSpan value equal to 0:00,
					             but it had TimeSpan value 0:00
					             """);
			}
		}
	}
}
