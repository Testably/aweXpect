namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
		public sealed class ActionTests
		{
			[Fact]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenDelegateThrows_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw any exception,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenDelegateThrows_ShouldForwardExceptionAsInnerException(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).ThrowsException()
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not throw any exception,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncValueTests
		{
			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnResultFromDelegate(int value)
			{
				Func<int> @delegate = () => value;

				int result = await That(@delegate).DoesNotThrow();

				await That(result).IsEqualTo(value);
			}

			[Fact]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenDelegateThrows_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw any exception,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not throw any exception,
					             but it was <null>
					             """);
			}
		}
	}
}
