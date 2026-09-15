namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ThrowsExactly
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldSupportChainedConstraints()
			{
				Action action = () => { };

				async Task Act()
					=> await That(action).ThrowsExactly<Exception>().WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly an Exception with Message equal to "foo",
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnThrownException(string value)
			{
				Exception exception = new CustomException
				{
					Value = value,
				};
				Action action = () => throw exception;

				CustomException result =
					await That(action).ThrowsExactly<CustomException>();

				await That(result.Value).IsEqualTo(value);
				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenCorrectExceptionTypeIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException,
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new OtherException(message);
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubCustomExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ThrowsExactly<CustomException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws exactly a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldSupportChainedConstraints()
			{
				Action action = () => { };

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(Exception)).WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly an Exception with Message equal to "foo",
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnThrownException(string value)
			{
				Exception exception = new CustomException
				{
					Value = value,
				};
				Action action = () => throw exception;

				Exception result =
					await That(action).ThrowsExactly(typeof(CustomException));

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenCorrectExceptionTypeIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException,
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new OtherException(message);
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubCustomExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws exactly a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}
		}
	}
}
