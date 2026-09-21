namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed partial class WithInner
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenAwaited_WithExpectations_ShouldReturnThrownException()
				{
					Exception exception = new OuterException(innerException: new Exception("inner"));
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithInner(e => e.HasMessage("inner"));

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenAwaited_WithoutExpectations_ShouldReturnThrownException()
				{
					Exception exception = new OuterException(innerException: new Exception("inner"));
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithInner();

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws().WithInner(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but Message was "bar" which differs at index 0:
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
						=> await That(action).Throws().WithInner();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception,
						             but it had no inner exception
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotPresent_WithExpectations_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithInner(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it had no inner exception
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsPresent_ShouldSucceed()
				{
					Action action = () => throw new OuterException(innerException: new Exception());

					async Task Act()
						=> await That(action).Throws().WithInner();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
