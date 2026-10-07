using System.Runtime.CompilerServices;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class ActionTypeTests
		{
			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			public async Task WhenDelegateThrowsOpenGenericTypeException_ShouldFail()
			{
				Action @delegate = () => throw new GenericException<int>("foo");

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(GenericException<>));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw a ThatDelegate.GenericException<>,
					             but it did throw a ThatDelegate.GenericException<int>:
					               foo
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsOtherException_ShouldSucceed(string message)
			{
				Exception exception = new OtherException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

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
					=> await That(@delegate).DoesNotThrow(typeof(SubCustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow(typeof(CustomException));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(string));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("type").And
					.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix()
					.Because("no exception could ever be a string");
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrow(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}
		}

		public sealed class FuncValueTypeTests
		{
			[Test]
			public async Task WhenAwaited_ShouldNotReturnAValue()
			{
				Func<int> @delegate = () => throw new OtherException();

				Type awaiter = That(@delegate).DoesNotThrow(typeof(CustomException)).GetAwaiter().GetType();

				await That(awaiter).IsEqualTo(typeof(TaskAwaiter))
					.Because("the delegate may throw another exception instead of returning a value");
			}

			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			public async Task WhenDelegateThrowsOpenGenericTypeException_ShouldFail()
			{
				Func<int> @delegate = () => throw new GenericException<int>("foo");

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(GenericException<>));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw a ThatDelegate.GenericException<>,
					             but it did throw a ThatDelegate.GenericException<int>:
					               foo
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsOtherException_ShouldSucceed(string message)
			{
				Exception exception = new OtherException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(CustomException));

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
					=> await That(@delegate).DoesNotThrow(typeof(SubCustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow(typeof(CustomException));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(typeof(string));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("type").And
					.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix()
					.Because("no exception could ever be a string");
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrow(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}
		}
#pragma warning restore CA2263
	}
}
