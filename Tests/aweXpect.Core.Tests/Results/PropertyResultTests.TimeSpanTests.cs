using aweXpect.Chronology;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Results;

public sealed partial class PropertyResultTests
{
	public sealed class TimeSpanTests
	{
		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task Between_ShouldVerifyThatActualIsBetweenMinimumAndMaximum()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.Between(41.Seconds()).And(43.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Fact]
		public async Task Between_WhenActualIsOutsideTheRange_ShouldFail()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			async Task Act()
				=> await sut.Between(43.Seconds()).And(44.Seconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has TimeSpan value between 0:43 and 0:44,
				             but it had TimeSpan value 0:42
				             """);
		}

		[Fact]
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

		[Fact]
		public async Task EqualTo_ShouldVerifyThatActualIsEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.EqualTo(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Fact]
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

		[Fact]
		public async Task GreaterThan_ShouldVerifyThatActualIsGreaterThanExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.GreaterThan(41.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Fact]
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

		[Fact]
		public async Task GreaterThanOrEqualTo_ShouldVerifyThatActualIsGreaterThanOrEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.GreaterThanOrEqualTo(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Fact]
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

		[Fact]
		public async Task LessThan_ShouldVerifyThatActualIsLessThanExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.LessThan(43.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Fact]
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

		[Fact]
		public async Task LessThanOrEqualTo_ShouldVerifyThatActualIsLessThanOrEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.LessThanOrEqualTo(42.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		[Fact]
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

		[Fact]
		public async Task NotEqualTo_ShouldVerifyThatActualIsNotEqualToExpected()
		{
			PropertyResult.TimeSpan<MyClass?> sut = MyClass.HasTimeSpanValue(42.Seconds());

			MyClass? result = await sut.NotEqualTo(41.Seconds());

			await That(result?.TimeSpanValue).IsEqualTo(42.Seconds());
		}

		public sealed class GrammarTests
		{
			[Fact]
			public async Task WhenActive_ShouldUseTheActiveVoice()
			{
				PropertyResult.TimeSpan<MyClass?, MyClass?, IThat<MyClass?>> sut =
					MyClass.HasTimeSpanValue(42.Seconds(), ExpectationGrammars.Active);

				async Task Act()
					=> await sut.GreaterThan(43.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             with TimeSpan value greater than 0:43,
					             but it had TimeSpan value 0:42
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldUseDoesNotHave()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
						.EqualTo(TimeSpan.Zero));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have TimeSpan value equal to 0:00,
					             but it had TimeSpan value 0:00
					             """);
			}

			[Fact]
			public async Task WhenNegated_WithNotEqualTo_ShouldExpectEquality()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.None)
						.NotEqualTo(1.Seconds()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has TimeSpan value equal to 0:01,
					             but it had TimeSpan value 0:00
					             """);
			}

			[Fact]
			public async Task WhenPluralAndNegated_ShouldUseThePluralVerb()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(s => MyClass.TimeSpanValueOf(s, ExpectationGrammars.Plural)
						.EqualTo(TimeSpan.Zero));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             do not have TimeSpan value equal to 0:00,
					             but it had TimeSpan value 0:00
					             """);
			}
		}
	}
}
