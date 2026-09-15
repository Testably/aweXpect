namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ThrowsExactly
	{
		public sealed class OnlyIf
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					await That(action).ThrowsExactly<Exception>()
						.OnlyIf(false)
						.WithMessage("foo");
				}

				[Fact]
				public async Task WhenAwaited_OnlyIfFalse_ShouldReturnNull()
				{
					Action action = () => { };

					CustomException? result =
						await That(action).ThrowsExactly<CustomException>().OnlyIf(false);

					await That(result).IsNull();
				}

				[Fact]
				public async Task WhenAwaited_OnlyIfTrue_ShouldReturnThrownException()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					CustomException? result =
						await That(action).ThrowsExactly<CustomException>().OnlyIf(true);

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenFalse_ShouldFailWhenAnExceptionWasThrown()
				{
					Exception exception = new("");
					Action action = () => throw exception;

					async Task Act()
						=> await That(action).ThrowsExactly<Exception>().OnlyIf(false);

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             does not throw any exception,
						             but it did throw an Exception
						             """);
				}

				[Fact]
				public async Task WhenFalse_ShouldSucceedWhenNoExceptionWasThrown()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly<Exception>().OnlyIf(false);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTrue_ShouldFailWhenNoExceptionWasThrow()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly<ArgumentException>().OnlyIf(true);

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an ArgumentException,
						             but it did not throw any exception
						             """);
				}

				[Fact]
				public async Task WhenTrue_ShouldSucceedWhenAnExceptionWasThrow()
				{
					Exception exception = new("");
					Action action = () => throw exception;

					async Task Act()
						=> await That(action).ThrowsExactly<Exception>().OnlyIf(true);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TypeTests
			{
				[Fact]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					await That(action).ThrowsExactly(typeof(Exception))
						.OnlyIf(false)
						.WithMessage("foo");
				}

				[Fact]
				public async Task WhenAwaited_OnlyIfFalse_ShouldReturnNull()
				{
					Action action = () => { };

					Exception? result =
						await That(action).ThrowsExactly(typeof(CustomException)).OnlyIf(false);

					await That(result).IsNull();
				}

				[Fact]
				public async Task WhenAwaited_OnlyIfTrue_ShouldReturnThrownException()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					Exception? result =
						await That(action).ThrowsExactly(typeof(CustomException)).OnlyIf(true);

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenFalse_ShouldFailWhenAnExceptionWasThrown()
				{
					Exception exception = new("");
					Action action = () => throw exception;

					async Task Act()
						=> await That(action).ThrowsExactly(typeof(Exception)).OnlyIf(false);

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             does not throw any exception,
						             but it did throw an Exception
						             """);
				}

				[Fact]
				public async Task WhenFalse_ShouldSucceedWhenNoExceptionWasThrown()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly(typeof(Exception)).OnlyIf(false);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTrue_ShouldFailWhenNoExceptionWasThrow()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly(typeof(ArgumentException)).OnlyIf(true);

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an ArgumentException,
						             but it did not throw any exception
						             """);
				}

				[Fact]
				public async Task WhenTrue_ShouldSucceedWhenAnExceptionWasThrow()
				{
					Exception exception = new("");
					Action action = () => throw exception;

					async Task Act()
						=> await That(action).ThrowsExactly(typeof(Exception)).OnlyIf(true);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
