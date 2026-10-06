using System.Runtime.CompilerServices;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
		public sealed class ActionGenericTests
		{
			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsOtherException_ShouldSucceed(string message)
			{
				Exception exception = new OtherException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSuperTypeOfException_ShouldSucceed(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<SubCustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow<CustomException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncValueGenericTests
		{
			[Test]
			public async Task WhenAwaited_ShouldNotReturnAValue()
			{
				Func<int> @delegate = () => throw new OtherException();

				Type awaiter = That(@delegate).DoesNotThrow<CustomException>().GetAwaiter().GetType();

				await That(awaiter).IsEqualTo(typeof(TaskAwaiter))
					.Because("the delegate may throw another exception instead of returning a value");
			}

			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsOtherException_ShouldSucceed(string message)
			{
				Exception exception = new OtherException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSuperTypeOfException_ShouldSucceed(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow<SubCustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow<CustomException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}
		}
	}
}
