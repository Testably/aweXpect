namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed partial class HasInner
	{
		public sealed class Tests
		{
			[Fact]
			public async Task AllowsNestedIs()
			{
				Exception subject = new InvalidOperationException(
					"outer",
					new InvalidCastException("inner"));

				async Task Act()
					=> await That(subject).HasInner(it => it.Is<InvalidCastException>()
						.Whose(e => e.Message, it => it.IsEqualTo("inner")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionHasCorrectMessage_ShouldSucceed()
			{
				Exception subject = new("outer",
					new CustomException("inner"));

				async Task Act()
					=> await That(subject).HasInner(e => e.HasMessage("inner"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionHasUnexpectedMessage_ShouldFail()
			{
				Exception subject = new("outer",
					new Exception("inner"));

				async Task Act()
					=> await That(subject).HasInner(e => e.HasMessage("some other message"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an inner exception whose Message is equal to "some other message",
					             but it was "inner" which differs at index 0:
					                ↓ (actual)
					               "inner"
					               "some other message"
					                ↑ (expected)

					             Message:
					             inner
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionIsNotSet_ShouldFail()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasInner();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an inner exception,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionIsSet_ShouldSucceed()
			{
				Exception subject = new("outer",
					new Exception("inner"));

				async Task Act()
					=> await That(subject).HasInner();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasInner();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an inner exception,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasInner());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsSet_ShouldFail()
			{
				Exception subject = new("outer",
					new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasInner());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner exception,
					             but it had an inner InvalidOperationException:
					               inner
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionMeetsExpectations_ShouldFail()
			{
				Exception subject = new("outer",
					new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasInner(e => e.HasMessage("inner")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner exception whose Message is equal to "inner",
					             but it had an inner InvalidOperationException:
					               inner

					             Message:
					             inner
					             """);
			}
		}
	}
}
