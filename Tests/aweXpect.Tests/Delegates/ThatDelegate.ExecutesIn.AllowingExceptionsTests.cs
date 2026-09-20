using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class AllowingExceptionsTests
		{
			[Fact]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);
				Func<Task> @delegate = () => Task.FromCanceled(canceledToken);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05 allowing exceptions,
					             but it was canceled after 0:*
					             """).AsWildcard()
					.Because("a cancellation aborts the execution instead of timing it");
			}

			[Fact]
			public async Task WhenDelegateThrowsAfterExceedingTheMaximum_ShouldFail()
			{
				Action @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.010 allowing exceptions,
					              but it took 0:* and did throw a MyException:
					                {nameof(WhenDelegateThrowsAfterExceedingTheMaximum_ShouldFail)}
					              """).AsWildcard()
					.Because("the exception is still the most useful context, even when the duration decided");
			}

			[Fact]
			public async Task WhenDelegateThrowsBeforeReachingTheMinimum_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtLeast(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at least 0:05 allowing exceptions,
					              but it took only 0:* and did throw a MyException:
					                {nameof(WhenDelegateThrowsBeforeReachingTheMinimum_ShouldFail)}
					              """).AsWildcard()
					.Because("allowing exceptions lets the duration decide, and here it was too short");
			}

			[Fact]
			public async Task WhenDelegateThrowsOperationCanceledException_ShouldForwardItAsInnerException()
			{
				Exception exception = new OperationCanceledException();
				Action @delegate = () => throw exception;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).Throws<XunitException>()
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a cancellation is never allowed, so it remains the cause of the failure");
			}

			[Fact]
			public async Task WhenDelegateThrowsWithinTheLimit_ShouldSucceed()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("the duration alone decides the outcome once exceptions are allowed");
			}

			[Fact]
			public async Task WhenDelegateWithValueThrowsAfterExceedingTheMaximum_ShouldFail()
			{
				Func<int> @delegate = () =>
				{
					Task.Delay(50.Milliseconds()).Wait();
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.010 allowing exceptions,
					              but it took 0:* and did throw a MyException:
					                {nameof(WhenDelegateWithValueThrowsAfterExceedingTheMaximum_ShouldFail)}
					              """).AsWildcard()
					.Because("the constraint for delegates with a return value must render the same message");
			}

			[Fact]
			public async Task WhenDelegateWithValueThrowsWithinTheLimit_ShouldSucceed()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("the constraint for delegates with a return value must decide the same way");
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.Whose(e => e.InnerException, i => i.IsNull())
					.Because("the exceeded duration, not the allowed exception, caused the failure");
			}

			[Fact]
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
		}
	}
}
