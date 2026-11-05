using System.Threading;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class ActionTests
		{
			[Test]
			public async Task WhenBecauseIsSpecifiedBeforeAnd_ShouldIncludeTheReason()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AllowingExceptions().AtMost(10.Milliseconds())
						.Because("it must be fast").And.DoesNotThrow().WithTimeProvider(new MockTimeSystem().ToTimeProvider());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.010 allowing exceptions and does not throw any exception, because it must be fast,
					              but it did throw a MyException:
					                {nameof(WhenBecauseIsSpecifiedBeforeAnd_ShouldIncludeTheReason)}
					              """);
			}

			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:00.050
					             """);
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncTaskTests
		{
			[Test]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);
				Func<Task> @delegate = () => Task.FromCanceled(canceledToken);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it did throw a TaskCanceledException:
					               *
					             """).AsWildcard().And
					.WithInner<TaskCanceledException>()
					.Because("a delegate that cancels itself for its own reasons throws an ordinary exception");
			}

			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<Task> @delegate = () => Task.CompletedTask;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<Task> @delegate = () => null!;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it returned <null> instead of a task
					             """)
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
#if NET8_0_OR_GREATER
				Func<Task> @delegate = () => Task.Delay(30.Seconds(), time, CancellationToken.None);
#else
				Func<Task> @delegate = () => TimeProviderTaskExtensions.Delay(time, 30.Seconds(), CancellationToken.None);
#endif

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the maximum is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<Task> @delegate = () => Task.FromException(new MyException());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<Task>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class FuncTaskValueTests
		{
			[Test]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);
				Func<Task<int>> @delegate = () => Task.FromCanceled<int>(canceledToken);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it did throw a TaskCanceledException:
					               *
					             """).AsWildcard().And
					.WithInner<TaskCanceledException>()
					.Because("a delegate that cancels itself for its own reasons throws an ordinary exception");
			}

			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<Task<int>> @delegate = () => Task.FromResult(1);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<CancellationToken, Task<int>> @delegate = _ => null!;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it returned <null> instead of a task
					             """)
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<Task<int>> @delegate = () =>
				{
					timeSystem.TimeProvider.AdvanceBy(30.Seconds());
					return new TaskCompletionSource<int>().Task;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the maximum is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<Task<int>> @delegate = () => Task.FromException<int>(new MyException());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<Task<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class FuncValueTaskTests
		{
			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask Delegate() => new(Task.CompletedTask);

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
#if NET8_0_OR_GREATER
				ValueTask Delegate() => new(Task.Delay(30.Seconds(), time, CancellationToken.None));
#else
				ValueTask Delegate() => new(TimeProviderTaskExtensions.Delay(time, 30.Seconds(), CancellationToken.None));
#endif

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the maximum is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask Delegate() => new(Task.FromException(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<ValueTask>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

#if NET8_0_OR_GREATER
		public sealed class FuncCancellationTokenValueTaskTests
		{
			[Test]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);

				ValueTask Delegate(CancellationToken _)
					=> new(Task.FromCanceled(canceledToken));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:05,
					             but it did throw a TaskCanceledException:
					               *
					             """).AsWildcard().And
					.WithInner<TaskCanceledException>()
					.Because("a delegate that cancels itself for its own reasons throws an ordinary exception");
			}

			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask Delegate(CancellationToken _)
					=> new(Task.CompletedTask);

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				ValueTask Delegate(CancellationToken token) => new(timeSystem.Task.Delay(30.Seconds(), token));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the maximum is applied as timeout, so the token is cancelled once it elapsed");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask Delegate(CancellationToken _)
					=> new(Task.FromException(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<CancellationToken, ValueTask>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

#if NET8_0_OR_GREATER
		public sealed class FuncValueTaskValueTests
		{
			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask<int> Delegate() => new(Task.FromResult(1));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				ValueTask<int> Delegate()
				{
					timeSystem.TimeProvider.AdvanceBy(30.Seconds());
					return new(new TaskCompletionSource<int>().Task);
				}

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the maximum is applied as timeout, so the task is abandoned once it elapsed");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask<int> Delegate() => new(Task.FromException<int>(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<ValueTask<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

#if NET8_0_OR_GREATER
		public sealed class FuncCancellationTokenValueTaskValueTests
		{
			[Test]
			public async Task WhenDelegateIsCanceled_ShouldFail()
			{
				CancellationToken canceledToken = new(true);

				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromCanceled<int>(canceledToken));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:05,
					             but it did throw a TaskCanceledException:
					               *
					             """).AsWildcard().And
					.WithInner<TaskCanceledException>()
					.Because("a delegate that cancels itself for its own reasons throws an ordinary exception");
			}

			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromResult(1));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				ValueTask<int> Delegate(CancellationToken token)
					=> new(timeSystem.Task.Delay(30.Seconds(), token).ContinueWith(_ => 1, token));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the maximum is applied as timeout, so the token is cancelled once it elapsed");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				ValueTask<int> Delegate(CancellationToken _)
					=> new(Task.FromException<int>(new MyException()));

				async Task Act()
					=> await That(Delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that Delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<CancellationToken, ValueTask<int>>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}
#endif

		public sealed class FuncValueTests
		{
			[Test]
			public async Task WhenBecauseIsSpecifiedBeforeAnd_ShouldIncludeTheReason()
			{
				Func<int> @delegate = () => 0;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).Because("it must be fast")
						.And.DoesNotThrow().WhoseResult.IsEqualTo(1).WithTimeProvider(new MockTimeSystem().ToTimeProvider());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010 and does not throw any exception and its result is equal to 1, because it must be fast,
					             but it was 0, which differs by -1
					             """);
			}

			[Test]
			public async Task WhenDelegateIsFastEnough_ShouldSucceed()
			{
				Func<int> @delegate = () => 0;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5000.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateTakesLonger_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<int> @delegate = () =>
				{
					timeSystem.TimeProvider.AdvanceBy(50.Milliseconds());
					return 0;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:00.050
					             """);
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at most 0:00.500,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not execute within the expected time");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).ExecutesIn().AtMost(500.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             executes in at most 0:00.500,
					             but it was <null>
					             """);
			}
		}

		public sealed class CancellationTokenTests
		{
			[Test]
			public async Task AtLeast_WhenDelegateExceedsTheMinimum_ShouldNotCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(200.Milliseconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).DoesNotThrow()
					.Because("a minimum is no upper bound, so nothing may interrupt the delegate");
			}

			[Test]
			public async Task AtMost_WithoutReturnValue_WhenDelegateExceedsTheMaximum_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it did not finish within 0:00.050
					             """)
					.Because("the maximum must cancel the token instead of awaiting the delegate");
			}

			[Test]
			public async Task AtMost_WithReturnValue_WhenDelegateExceedsTheMaximum_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await timeSystem.Task.Delay(30.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it did not finish within 0:00.050
					             """)
					.Because("the maximum must cancel the token instead of awaiting the delegate");
			}

			[Test]
			public async Task Between_WithoutReturnValue_WhenDelegateExceedsTheMaximum_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(10.Milliseconds()).And(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in between 0:00.010 and 0:00.050,
					             but it did not finish within 0:00.050
					             """)
					.Because("the maximum of the range must cancel the token instead of awaiting the delegate");
			}

			[Test]
			public async Task Between_WithReturnValue_WhenDelegateExceedsTheMaximum_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await timeSystem.Task.Delay(30.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().Between(10.Milliseconds()).And(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in between 0:00.010 and 0:00.050,
					             but it did not finish within 0:00.050
					             """)
					.Because("the maximum of the range must cancel the token instead of awaiting the delegate");
			}

			[Test]
			public async Task Within_WithoutReturnValue_WhenDelegateExceedsTheTolerance_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn(10.Milliseconds()).Within(40.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in approximately 0:00.010 ± 0:00.040,
					             but it did not finish within 0:00.050
					             """)
					.Because("the expected time plus the tolerance must cancel the token");
			}

			[Test]
			public async Task Within_WithReturnValue_WhenDelegateExceedsTheTolerance_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await timeSystem.Task.Delay(30.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn(10.Milliseconds()).Within(40.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in approximately 0:00.010 ± 0:00.040,
					             but it did not finish within 0:00.050
					             """)
					.Because("the expected time plus the tolerance must cancel the token");
			}

			[Test]
			public async Task WithoutCancellationToken_WhenAsyncDelegateExceedsTheMaximum_ShouldAbandonIt()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
#if NET8_0_OR_GREATER
				Func<Task> @delegate = () => Task.Delay(30.Seconds(), time, CancellationToken.None);
#else
				Func<Task> @delegate = () => TimeProviderTaskExtensions.Delay(time, 30.Seconds(), CancellationToken.None);
#endif

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it did not finish within 0:00.010
					             """)
					.Because("the task of an asynchronous delegate is abandoned instead of awaited to completion");
			}

			[Test]
			public async Task WithoutCancellationToken_WhenSyncDelegateExceedsTheMaximum_ShouldAwaitItToCompletion()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				bool didComplete = false;
				Action @delegate = () =>
				{
					timeSystem.TimeProvider.AdvanceBy(100.Milliseconds());
					didComplete = true;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(10.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.010,
					             but it took 0:00.100
					             """);
				await That(didComplete).IsTrue()
					.Because("a delegate that cannot be interrupted is awaited instead of being abandoned");
			}
		}

		public sealed class WithTimeoutTests
		{
			[Test]
			public async Task WhenLaterTimeoutIsLonger_ShouldKeepTheUpperBound()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithTimeout(60.Seconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it did not finish within 0:00.050
					             """)
					.Because("the tighter limit wins, so a longer timeout must not loosen the upper bound");
			}

			[Test]
			public async Task WhenSyncDelegateExceedsTheTimeoutAndTheMaximum_ShouldReportTheDuration()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(200.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(100.Milliseconds()).WithTimeout(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.100,
					             but it took 0:00.200
					             """)
					.Because("the delegate violates the maximum on its own, which the measured duration shows best");
			}

			[Test]
			public async Task WhenSyncDelegateExceedsTheTimeoutButNotTheMaximum_ShouldFailWithTheTimeout()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(200.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5.Seconds()).WithTimeout(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
					.Because("the tighter timeout wins over the maximum");
			}

			[Test]
			public async Task WhenSyncDelegateExceedsTheTimeoutButReachesTheMinimum_ShouldFailWithTheTimeout()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Action @delegate = () => timeSystem.TimeProvider.AdvanceBy(200.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(100.Milliseconds()).WithTimeout(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at least 0:00.100,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
					.Because("a minimum has no upper bound, so any timeout is tighter");
			}

			[Test]
			public async Task WhenSyncDelegateWithValueExceedsTheTimeoutButNotTheMaximum_ShouldFailWithTheTimeout()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<int> @delegate = () =>
				{
					timeSystem.TimeProvider.AdvanceBy(200.Milliseconds());
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5.Seconds()).WithTimeout(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:05,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
					.Because("the tighter timeout wins over the maximum");
			}

			[Test]
			public async Task WithoutReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(30.Seconds(), token);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(4000.Milliseconds()).WithTimeout(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:04,
					             but it did not finish within 0:00.050
					             """);
			}

			[Test]
			public async Task WithReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new(o => o.EnableStrictTimerOrder());
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await timeSystem.Task.Delay(30.Seconds(), token);
					return 1;
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(4000.Milliseconds()).WithTimeout(50.Milliseconds()).WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:04,
					             but it did not finish within 0:00.050
					             """);
			}
		}

		public sealed class WithCancellationTests
		{
			[Test]
			public async Task WithoutReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task> @delegate = token => timeSystem.Task.Delay(30.Seconds(), token);
				CancellationToken cancelledToken = new(true);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithCancellation(cancelledToken).WithTimeProvider(time);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it could not be verified, because the evaluation was already canceled
					             """);
			}

			[Test]
			public async Task WithReturnValue_WhenTimeoutIsApplied_ShouldCancelTheCancellationToken()
			{
				MockTimeSystem timeSystem = new();
				TimeProvider time = timeSystem.ToTimeProvider();
				Func<CancellationToken, Task<int>> @delegate = async token =>
				{
					await timeSystem.Task.Delay(30.Seconds(), token);
					return 1;
				};
				CancellationToken cancelledToken = new(true);

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds()).WithCancellation(cancelledToken).WithTimeProvider(time);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at most 0:00.050,
					             but it could not be verified, because the evaluation was already canceled
					             """);
			}
		}
	}
}
