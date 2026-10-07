using System.Runtime.CompilerServices;
using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrowExactly
	{
		public sealed class ActionGenericTests
		{
			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<Task> @delegate = () => null!;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<NullReferenceException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw exactly a NullReferenceException,
					             but it returned <null> instead of a task
					             """)
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldForwardExceptionAsInnerException(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).Throws()
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsOtherException_ShouldSucceed(string message)
			{
				Exception exception = new OtherException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldSucceed(string message)
			{
				Exception exception = new SubCustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSuperTypeOfException_ShouldSucceed(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<SubCustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrowExactly<CustomException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw exactly a ThatDelegate.CustomException,
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

				Type awaiter = That(@delegate).DoesNotThrowExactly<CustomException>().GetAwaiter().GetType();

				await That(awaiter).IsEqualTo(typeof(TaskAwaiter))
					.Because("the delegate may throw another exception instead of returning a value");
			}

			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<CancellationToken, Task<int>> @delegate = _ => null!;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<NullReferenceException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw exactly a NullReferenceException,
					             but it returned <null> instead of a task
					             """)
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw exactly a ThatDelegate.CustomException,
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
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldSucceed(string message)
			{
				Exception exception = new SubCustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsSuperTypeOfException_ShouldSucceed(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrowExactly<SubCustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrowExactly<CustomException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw exactly a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}
		}
	}
}
