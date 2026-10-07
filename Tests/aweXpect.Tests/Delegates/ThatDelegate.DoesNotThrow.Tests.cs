namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
		public sealed class ActionTests
		{
			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrows_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw any exception,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrows_ShouldForwardExceptionAsInnerException(string message)
			{
				Exception exception = new CustomException(message);
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).Throws()
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw any exception,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncTaskTests
		{
			[Test]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<Task> @delegate = () => null!;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw any exception,
					             but it returned <null> instead of a task
					             """).And
					.Whose(e => e.InnerException, i => i.IsNull())
					.Because("a null task is not an exception thrown by the delegate");
			}
		}

		public sealed class FuncTaskValueTests
		{
			[Test]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<Task<int>> @delegate = () => null!;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw any exception,
					             but it returned <null> instead of a task
					             """).And
					.Whose(e => e.InnerException, i => i.IsNull())
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Test]
			public async Task WhenDelegateReturnsNullTask_WhoseResult_ShouldFail()
			{
				Func<Task<int>> @delegate = () => null!;

				async Task Act()
					=> await That(@delegate).DoesNotThrow().WhoseResult.IsEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             does not throw any exception and its result is equal to 1,
					             but it returned <null> instead of a task
					             """);
			}
		}

		public sealed class FuncValueTests
		{
			[Test]
			[AutoArguments]
			public async Task WhenAwaited_ShouldReturnResultFromDelegate(int value)
			{
				Func<int> @delegate = () => value;

				int result = await That(@delegate).DoesNotThrow();

				await That(result).IsEqualTo(value);
			}

			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldSucceed()
			{
				Func<int> @delegate = () => 1;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrows_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Func<int> @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).DoesNotThrow();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that @delegate
					              does not throw any exception,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw any exception,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WhoseResult_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).DoesNotThrow().WhoseResult.IsEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not throw any exception and its result is equal to 1,
					             but it was <null>
					             """);
			}
		}
	}
}
