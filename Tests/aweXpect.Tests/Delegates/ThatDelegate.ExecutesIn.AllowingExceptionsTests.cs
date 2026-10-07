using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class AllowingExceptionsTests
		{
			[Test]
			public async Task WhenDelegateIsCanceled_ShouldSucceed()
			{
				CancellationToken canceledToken = new(true);
				Func<Task> @delegate = () => Task.FromCanceled(canceledToken);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("a delegate that cancels itself for its own reasons throws an ordinary exception, which is allowed");
			}

			[Test]
			public async Task WhenDelegateThrowsAfterExceedingTheMaximum_ShouldFail()
			{
				Action @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(10.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.010 allowing exceptions,
					              but it took 0:* and did throw a MyException:
					                {nameof(WhenDelegateThrowsAfterExceedingTheMaximum_ShouldFail)}
					              """).AsWildcard()
					.Because("the exception is still the most useful context, even when the duration decided");
			}

			[Test]
			public async Task WhenDelegateThrowsBeforeReachingTheMinimum_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtLeast(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at least 0:05 allowing exceptions,
					              but it took only 0:* and did throw a MyException:
					                {nameof(WhenDelegateThrowsBeforeReachingTheMinimum_ShouldFail)}
					              """).AsWildcard()
					.Because("allowing exceptions lets the duration decide, and here it was too short");
			}

			[Test]
			public async Task WhenDelegateThrowsOperationCanceledExceptionBeforeReachingTheMinimum_ShouldFail()
			{
				Action @delegate = () => throw new OperationCanceledException("my own reason");

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtLeast(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at least 0:05 allowing exceptions,
					             but it took only 0:* and did throw an OperationCanceledException:
					               my own reason
					             """).AsWildcard()
					.Because("a cancellation that neither the timeout nor the caller requested is an ordinary exception");
			}

			[Test]
			public async Task WhenDelegateThrowsWithinTheLimit_ShouldSucceed()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("the duration alone decides the outcome once exceptions are allowed");
			}

			[Test]
			public async Task WhenDelegateWithValueThrowsAfterExceedingTheMaximum_ShouldFail()
			{
				Func<int> @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(10.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.010 allowing exceptions,
					              but it took 0:* and did throw a MyException:
					                {nameof(WhenDelegateWithValueThrowsAfterExceedingTheMaximum_ShouldFail)}
					              """).AsWildcard()
					.Because("the constraint for delegates with a return value must render the same message");
			}

			[Test]
			public async Task WhenDelegateWithValueThrowsWithinTheLimit_ShouldSucceed()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("the constraint for delegates with a return value must decide the same way");
			}

			[Test]
			public async Task WhenExceptionIsAllowed_ShouldNotForwardItAsInnerException()
			{
				Exception exception = new MyException();
				Action @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					throw exception;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(10.Milliseconds());

				await That(Act).Throws<FailException>()
					.Whose(e => e.InnerException, i => i.IsNull())
					.Because("the exceeded duration, not the allowed exception, caused the failure");
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldFailForAThrownException()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions(false).AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:05,
					              but it did throw a MyException:
					                {nameof(WhenFalseIsSpecified_ShouldFailForAThrownException)}
					              """)
					.Because("an exception fails the expectation, as without the option");
			}

			[Test]
			[Arguments(true, true)]
			[Arguments(true, false)]
			[Arguments(false, true)]
			[Arguments(false, false)]
			public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException(bool first, bool second)
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions(first).AllowingExceptions(second)
						.AtMost(5000.Milliseconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AllowingExceptions cannot be specified more than once.")
					.Because("the second value would silently replace the first one");
			}

			[Test]
			public async Task WhenTheUpperBoundCancelsTheDelegate_ShouldFail()
			{
				Func<CancellationToken, Task> @delegate = token => Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050 allowing exceptions,
					             but it did not finish within 0:00.050
					             """)
					.Because("allowing exceptions must not let a delegate pass by being cancelled at the maximum");
			}

			[Test]
			public async Task WhenToleranceIsGivenAndDelegateThrowsWithinIt_ShouldSucceed()
			{
				Action @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn(50.Milliseconds()).AllowingExceptions().Within(5.Seconds());

				await That(Act).DoesNotThrow()
					.Because("the option must also be available on the tolerance overload");
			}

			[Test]
			public async Task WhenToleranceIsGivenAndFalseIsSpecified_ShouldFailForAThrownException()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn(50.Milliseconds()).AllowingExceptions(false).Within(5.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in approximately 0:00.050 ± 0:05,
					              but it did throw a MyException:
					                {nameof(WhenToleranceIsGivenAndFalseIsSpecified_ShouldFailForAThrownException)}
					              """)
					.Because("an exception fails the expectation, as without the option");
			}

			[Test]
			[Arguments(true, true)]
			[Arguments(true, false)]
			[Arguments(false, true)]
			[Arguments(false, false)]
			public async Task WhenToleranceIsGivenAndSpecifiedTwice_ShouldThrowInvalidOperationException(
				bool first, bool second)
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesIn(50.Milliseconds())
						.AllowingExceptions(first).AllowingExceptions(second).Within(5.Seconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AllowingExceptions cannot be specified more than once.")
					.Because("the second value would silently replace the first one");
			}
		}
	}
}
