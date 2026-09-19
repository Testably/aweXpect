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
		public async Task EqualTo_WhenReadingThePropertyThrowsAnExpectedException_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut =
				MyClass.HasThrowingLongValue(new InvalidOperationException("foo"), _ => true);

			async Task Act()
				=> await sut.EqualTo(42L);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value equal to 42,
				             but it could not read the long value, because it did throw an InvalidOperationException:
				               foo
				             """)
				.Because("a property that the caller declared unreadable fails the expectation");
		}

		[Fact]
		public async Task EqualTo_WhenReadingThePropertyThrowsAnUnexpectedException_ShouldThrowTheWrappedException()
		{
			PropertyResult.Long<MyClass?> sut =
				MyClass.HasThrowingLongValue(new InvalidOperationException("foo"), _ => false);

			async Task Act()
				=> await sut.EqualTo(42L);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Error evaluating*constraint with value*foo").AsWildcard()
				.Because("an undeclared exception remains a defect");
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
		public async Task NotEqualTo_WhenReadingThePropertyThrowsAnExpectedException_ShouldFail()
		{
			PropertyResult.Long<MyClass?> sut =
				MyClass.HasThrowingLongValue(new InvalidOperationException("foo"), _ => true);

			async Task Act()
				=> await sut.NotEqualTo(42L);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has long value not equal to 42,
				             but it could not read the long value, because it did throw an InvalidOperationException:
				               foo
				             """)
				.Because("a property that was never read cannot prove inequality either");
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
