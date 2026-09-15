namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithInnerExceptionTests
		{
			[Fact]
			public async Task WhenAwaited_WithExpectations_ShouldReturnThrownException()
			{
				Exception exception = new OuterException(innerException: new Exception("inner"));
				void Delegate() => throw exception;

				Exception result = await That(Delegate)
					.Throws().WithInnerException(e => e.HasMessage("inner"));

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenAwaited_WithoutExpectations_ShouldReturnThrownException()
			{
				Exception exception = new OuterException(innerException: new Exception("inner"));
				void Delegate() => throw exception;

				Exception result = await That(Delegate)
					.Throws().WithInnerException();

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
			{
				string message = "bar";
				Action action = ()
					=> throw new OuterException(innerException: new CustomException(message));

				async Task Act()
					=> await That(action).Throws().WithInnerException(x => x.HasMessage("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with an inner exception whose Message is equal to "foo",
					             but it was "bar" which differs at index 0:
					                ↓ (actual)
					               "bar"
					               "foo"
					                ↑ (expected)

					             Message:
					             bar
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionIsNotPresent_ShouldFail()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithInnerException();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with an inner exception,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionIsPresent_ShouldSucceed()
			{
				Action action = () => throw new OuterException(innerException: new Exception());

				async Task Act()
					=> await That(action).Throws().WithInnerException();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_ShouldFail()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithInnerException();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with an inner exception,
					             but it was <null>
					             """);
			}
		}
	}
}
